using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ReviewBooks.Books.Dto;
using ReviewBooks.Books.Models;
using ReviewBooks.Books.Repository;
using Shared;

namespace ReviewBooks.Books.Services
{
    public interface IBookService
    {
        Task<BookDto?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<PageResult<BookDto>> SearchAsync(Query query, CancellationToken ct = default);
    }

    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<BookService> _logger;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

        public BookService(IBookRepository repo, HttpClient http, IConfiguration config, ILogger<BookService> logger)
        {
            _repo = repo;
            _http = http;
            _config = config;
            _logger = logger;
        }

        public async Task<BookDto?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            // 1) Check cache
            var cache = await _repo.GetBookCacheAsync(id, ct);
            if (cache is not null && DateTime.UtcNow - cache.CachedAt < CacheTtl)
            {
                // Prefer Book entity if exists
                var existing = await _repo.GetBookByIdAsync(id, ct);
                if (existing is not null)
                {
                    return ToDto(existing);
                }
                // Fallback: parse from cached JSON
                var parsed = ParseFromGoogleJson(id, cache.JsonData);
                if (parsed is not null) return parsed;
            }

            // 2) Call Google Books API
            // Ensure base URL ends with /books/v1 to avoid 404 on /volumes
            var configuredBase = _config.GetValue<string>("GoogleBooks:BaseUrl");
            var baseUrl = string.IsNullOrWhiteSpace(configuredBase)
                ? "https://www.googleapis.com/books/v1"
                : NormalizeGoogleBooksBaseUrl(configuredBase);
            var apiKey = _config.GetValue<string>("GoogleBooks:ApiKey");
            var url = string.IsNullOrEmpty(apiKey)
                ? $"{baseUrl}/volumes/{id}"
                : $"{baseUrl}/volumes/{id}?key={apiKey}";

            _logger.LogInformation("Calling Google Books API: {Url}", url);
            using var resp = await _http.GetAsync(url, ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google Books API failed for ID {BookId}. Status: {StatusCode}, Reason: {Reason}",
                    id, resp.StatusCode, resp.ReasonPhrase);
                return null;
            }

            var json = await resp.Content.ReadAsStringAsync(ct);
            _logger.LogInformation("Google Books API response received for ID {BookId}, Length: {Length}", id, json.Length);
            var dto = ParseFromGoogleJson(id, json);
            if (dto is null) return null;

            // 3) Upsert Book + BookCache
            var book = await _repo.GetBookByIdAsync(id, ct) ?? new Book { Id = id };
            book.Title = dto.Title;
            book.Authors = dto.Authors;
            book.Publisher = dto.Publisher;
            book.Description = dto.Description;
            book.Thumbnail = dto.Thumbnail;
            book.AverageRating = dto.AverageRating;
            book.RatingsCount = dto.RatingsCount;
            book.PublishedDate = dto.PublishedDate;
            book.ISBN = dto.ISBN;
            book.Categories = dto.Categories;
            book.Price = dto.Price;
            book.Url_Buy = dto.Url_Buy;
            book.CachedAt = DateTime.UtcNow;

            await _repo.UpsertBookAsync(book, ct);

            var bookCache = new BookCache
            {
                BookId = id,
                JsonData = json,
                CachedAt = DateTime.UtcNow
            };
            await _repo.UpsertBookCacheAsync(bookCache, ct);
            return dto;
        }

        public async Task<PageResult<BookDto>> SearchAsync(Query query, CancellationToken ct = default)
        {
            var q = (query.search ?? string.Empty).Trim();
            var pageSize = query.pageSize <= 0 ? 10 : Math.Min(query.pageSize, 40); // Google max 40
            var pageNumber = query.pageNumber <= 0 ? 1 : query.pageNumber;
            var startIndex = (pageNumber - 1) * pageSize;

            var configuredBase = _config.GetValue<string>("GoogleBooks:BaseUrl");
            var baseUrl = string.IsNullOrWhiteSpace(configuredBase)
                ? "https://www.googleapis.com/books/v1"
                : NormalizeGoogleBooksBaseUrl(configuredBase);
            var apiKey = _config.GetValue<string>("GoogleBooks:ApiKey");

            // Map sortBy to orderBy (Google supports: relevance|newest)
            string? orderBy = null;
            if (!string.IsNullOrWhiteSpace(query.sortBy))
            {
                var s = query.sortBy.Trim().ToLowerInvariant();
                if (s == "newest" || s == "relevance") orderBy = s;
            }

            // Optional filter mapping: ebooks|free-ebooks|paid-ebooks|full|partial
            string? filter = null;
            if (!string.IsNullOrWhiteSpace(query.filterBy))
            {
                var f = query.filterBy.Trim().ToLowerInvariant();
                var allowed = new HashSet<string> { "ebooks", "free-ebooks", "paid-ebooks", "full", "partial" };
                if (allowed.Contains(f)) filter = f;
            }

            if (string.IsNullOrWhiteSpace(q))
            {
                // Without a query term, Google returns nothing meaningful
                return new PageResult<BookDto>
                {
                    Items = new List<BookDto>(),
                    TotalCount = 0,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                };
            }

            var url = $"{baseUrl}/volumes?q={Uri.EscapeDataString(q)}&startIndex={startIndex}&maxResults={pageSize}";
            if (!string.IsNullOrEmpty(apiKey)) url += $"&key={apiKey}";
            if (!string.IsNullOrEmpty(orderBy)) url += $"&orderBy={orderBy}";
            if (!string.IsNullOrEmpty(filter)) url += $"&filter={filter}";

            _logger.LogInformation("Calling Google Books Search API: {Url}", url);
            using var resp = await _http.GetAsync(url, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var errorContent = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Google Books Search API failed. Status: {StatusCode}, Reason: {Reason}, Response: {Response}",
                    resp.StatusCode, resp.ReasonPhrase, errorContent);
                return new PageResult<BookDto>
                {
                    Items = new List<BookDto>(),
                    TotalCount = 0,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                };
            }

            var json = await resp.Content.ReadAsStringAsync(ct);
            _logger.LogInformation("Google Books Search API response received. Query: {Query}, Length: {Length}", q, json.Length);
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                int total = root.TryGetProperty("totalItems", out var ti) && ti.ValueKind == JsonValueKind.Number && ti.TryGetInt32(out var t)
                    ? t : 0;

                var items = new List<BookDto>();
                if (root.TryGetProperty("items", out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in arr.EnumerateArray())
                    {
                        var dto = ParseFromSearchItem(item);
                        if (dto is null) continue;

                        // Lưu DB: Book và BookCache từ item search
                        var book = new Book
                        {
                            Id = dto.Id,
                            Title = dto.Title,
                            Authors = dto.Authors,
                            Publisher = dto.Publisher,
                            Description = dto.Description,
                            Thumbnail = dto.Thumbnail,
                            AverageRating = dto.AverageRating,
                            RatingsCount = dto.RatingsCount,
                            PublishedDate = dto.PublishedDate,
                            ISBN = dto.ISBN,
                            Categories = dto.Categories,
                            Price = dto.Price,
                            Url_Buy = dto.Url_Buy,
                            CachedAt = DateTime.UtcNow
                        };
                        await _repo.UpsertBookAsync(book, ct);

                        var rawItemJson = item.GetRawText();
                        var cache = new BookCache
                        {
                            BookId = dto.Id,
                            JsonData = rawItemJson,
                            CachedAt = DateTime.UtcNow
                        };
                        await _repo.UpsertBookCacheAsync(cache, ct);

                        items.Add(dto);
                    }
                }

                return new PageResult<BookDto>
                {
                    Items = items,
                    TotalCount = total,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Google Books Search API response. Query: {Query}", q);
                return new PageResult<BookDto>
                {
                    Items = new List<BookDto>(),
                    TotalCount = 0,
                    PageSize = pageSize,
                    CurrentPage = pageNumber
                };
            }
        }

        private static BookDto? ParseFromSearchItem(JsonElement item)
        {
            try
            {
                if (item.ValueKind != JsonValueKind.Object) return null;
                if (!item.TryGetProperty("id", out var idEl) || idEl.ValueKind != JsonValueKind.String) return null;
                var id = idEl.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(id)) return null;

                if (!item.TryGetProperty("volumeInfo", out var vi)) return null;

                string GetString(JsonElement obj, string prop)
                    => obj.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";

                string? authors = null;
                if (vi.TryGetProperty("authors", out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<string>();
                    foreach (var a in arr.EnumerateArray())
                    {
                        if (a.ValueKind == JsonValueKind.String) list.Add(a.GetString() ?? "");
                    }
                    authors = list.Count > 0 ? string.Join(", ", list) : null;
                }

                string? thumbnail = null;
                if (vi.TryGetProperty("imageLinks", out var il) && il.ValueKind == JsonValueKind.Object)
                {
                    thumbnail = GetString(il, "thumbnail");
                    if (string.IsNullOrEmpty(thumbnail))
                        thumbnail = GetString(il, "smallThumbnail");
                }

                double? avgRating = null;
                if (vi.TryGetProperty("averageRating", out var ar) && ar.ValueKind == JsonValueKind.Number && ar.TryGetDouble(out var d))
                {
                    avgRating = d;
                }

                int? ratingsCount = null;
                if (vi.TryGetProperty("ratingsCount", out var rc) && rc.ValueKind == JsonValueKind.Number && rc.TryGetInt32(out var i))
                {
                    ratingsCount = i;
                }

                double? price = null;
                if (vi.TryGetProperty("price", out var priceEl) && priceEl.ValueKind == JsonValueKind.Number && priceEl.TryGetDouble(out var priceVal))
                {
                    price = priceVal;
                }

                string? url = null;
                if (vi.TryGetProperty("buyLink", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                {
                    url = urlEl.GetString();
                }

                DateTime? publishedDate = null;
                if (vi.TryGetProperty("publishedDate", out var pdEl) && pdEl.ValueKind == JsonValueKind.String)
                {
                    var pdStr = pdEl.GetString();
                    if (!string.IsNullOrWhiteSpace(pdStr))
                    {
                        DateTime parsed;

                        if (pdStr!.Length == 4 && int.TryParse(pdStr, out var year))
                        {
                            parsed = new DateTime(year, 1, 1);
                        }
                        else if (pdStr.Length == 7 && DateTime.TryParse(pdStr + "-01", out parsed))
                        {
                            // parsed ok
                        }
                        // Full date
                        else if (DateTime.TryParse(pdStr, out parsed))
                        {
                            // parsed ok
                        }
                        else
                        {
                            parsed = DateTime.UtcNow; // fallback tránh lỗi
                        }
                        publishedDate = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
                    }
                }

                string? isbn = null;
                if (vi.TryGetProperty("industryIdentifiers", out var iiArr) && iiArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var iden in iiArr.EnumerateArray())
                    {
                        if (iden.ValueKind == JsonValueKind.Object &&
                            iden.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String &&
                            iden.TryGetProperty("identifier", out var identifierEl) && identifierEl.ValueKind == JsonValueKind.String)
                        {
                            var typeStr = typeEl.GetString();
                            if (typeStr == "ISBN_13" || typeStr == "ISBN_10")
                            {
                                isbn = identifierEl.GetString();
                                break;
                            }
                        }
                    }
                }

                // Parse Categories
                string? categories = null;
                if (vi.TryGetProperty("categories", out var catArr) && catArr.ValueKind == JsonValueKind.Array)
                {
                    var catList = new List<string>();
                    foreach (var cat in catArr.EnumerateArray())
                    {
                        if (cat.ValueKind == JsonValueKind.String)
                        {
                            var catStr = cat.GetString();
                            if (!string.IsNullOrWhiteSpace(catStr))
                                catList.Add(catStr);
                        }
                    }
                    categories = catList.Count > 0 ? string.Join(", ", catList) : null;
                }

                // Parse Price and Buy Link from saleInfo
                double? salePrice = null;
                string? buyLink = null;
                if (item.TryGetProperty("saleInfo", out var saleInfo) && saleInfo.ValueKind == JsonValueKind.Object)
                {
                    // Get price
                    if (saleInfo.TryGetProperty("listPrice", out var listPriceObj) && listPriceObj.ValueKind == JsonValueKind.Object)
                    {
                        if (listPriceObj.TryGetProperty("amount", out var amountEl) && amountEl.ValueKind == JsonValueKind.Number && amountEl.TryGetDouble(out var amount))
                        {
                            salePrice = amount;
                        }
                    }
                    // Get buy link
                    if (saleInfo.TryGetProperty("buyLink", out var buyLinkEl) && buyLinkEl.ValueKind == JsonValueKind.String)
                    {
                        buyLink = buyLinkEl.GetString();
                    }
                }

                // Use saleInfo data if available, otherwise fall back to volumeInfo
                price = salePrice ?? price;
                url = buyLink ?? url;


                return new BookDto
                {
                    Id = id,
                    Title = GetString(vi, "title"),
                    Authors = authors,
                    Publisher = GetString(vi, "publisher"),
                    Description = GetString(vi, "description"),
                    Thumbnail = thumbnail,
                    AverageRating = avgRating,
                    Price = price,
                    Url_Buy = url,
                    ISBN = isbn,
                    PublishedDate = publishedDate,
                    Categories = categories,
                    RatingsCount = ratingsCount
                };
            }
            catch
            {
                return null;
            }
        }

        private static BookDto? ParseFromGoogleJson(string id, string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return null;

                if (!root.TryGetProperty("volumeInfo", out var vi)) return null;

                string GetString(JsonElement obj, string prop)
                    => obj.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";

                string? authors = null;
                if (vi.TryGetProperty("authors", out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<string>();
                    foreach (var a in arr.EnumerateArray())
                    {
                        if (a.ValueKind == JsonValueKind.String) list.Add(a.GetString() ?? "");
                    }
                    authors = list.Count > 0 ? string.Join(", ", list) : null;
                }

                string? thumbnail = null;
                if (vi.TryGetProperty("imageLinks", out var il) && il.ValueKind == JsonValueKind.Object)
                {
                    thumbnail = GetString(il, "thumbnail");
                    if (string.IsNullOrEmpty(thumbnail))
                        thumbnail = GetString(il, "smallThumbnail");
                }

                double? avgRating = null;
                if (vi.TryGetProperty("averageRating", out var ar) &&
                    (ar.ValueKind == JsonValueKind.Number) &&
                    ar.TryGetDouble(out var d))
                {
                    avgRating = d;
                }

                int? ratingsCount = null;
                if (vi.TryGetProperty("ratingsCount", out var rc) &&
                    rc.ValueKind == JsonValueKind.Number &&
                    rc.TryGetInt32(out var i))
                {
                    ratingsCount = i;
                }

                string? publishedDate = null;
                if (vi.TryGetProperty("publishedDate", out var pd) && pd.ValueKind == JsonValueKind.String)
                {
                    publishedDate = pd.GetString();
                }

                string? isbn = null;
                if (vi.TryGetProperty("industryIdentifiers", out var iiArr) && iiArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var iden in iiArr.EnumerateArray())
                    {
                        if (iden.ValueKind == JsonValueKind.Object &&
                            iden.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String &&
                            iden.TryGetProperty("identifier", out var identifierEl) && identifierEl.ValueKind == JsonValueKind.String)
                        {
                            var typeStr = typeEl.GetString();
                            if (typeStr == "ISBN_13" || typeStr == "ISBN_10")
                            {
                                isbn = identifierEl.GetString();
                                break;
                            }
                        }
                    }
                }

                double? price = null;
                if (vi.TryGetProperty("price", out var priceEl) && priceEl.ValueKind == JsonValueKind.Number && priceEl.TryGetDouble(out var priceVal))
                {
                    price = priceVal;
                }

                string? url = null;
                if (vi.TryGetProperty("buyLink", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                {
                    url = urlEl.GetString();
                }

                return new BookDto
                {
                    Id = id,
                    Title = GetString(vi, "title"),
                    Authors = authors,
                    Publisher = GetString(vi, "publisher"),
                    Description = GetString(vi, "description"),
                    Thumbnail = thumbnail,
                    AverageRating = avgRating,
                    RatingsCount = ratingsCount,
                    ISBN = isbn,
                    PublishedDate = string.IsNullOrWhiteSpace(publishedDate) ? null : DateTime.Parse(publishedDate),
                    Price = price,
                    Url_Buy = url
                };
            }
            catch
            {
                return null;
            }
        }

        private static BookDto ToDto(Book b) => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Authors = b.Authors,
            Publisher = b.Publisher,
            Description = b.Description,
            Thumbnail = b.Thumbnail,
            AverageRating = b.AverageRating,
            RatingsCount = b.RatingsCount
        };

        private static string NormalizeGoogleBooksBaseUrl(string input)
        {
            var trimmed = input.Trim().TrimEnd('/');
            // Accept either https://www.googleapis.com or https://www.googleapis.com/books/v1
            if (trimmed.EndsWith("/books/v1", StringComparison.OrdinalIgnoreCase))
                return trimmed;
            return trimmed + "/books/v1";
        }
    }
}