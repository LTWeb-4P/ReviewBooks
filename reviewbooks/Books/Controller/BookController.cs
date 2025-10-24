using Microsoft.AspNetCore.Mvc;
using ReviewBooks.Books.Services;
using ReviewBooks.Books.Dto;
using Shared;
using System.Text.Json;
using System.Diagnostics;

namespace ReviewBooks.Controller
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, HttpClient http, IConfiguration config, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _http = http;
            _config = config;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById([FromRoute] string id, CancellationToken ct)
        {
            var result = await _bookService.GetByIdAsync(id, ct);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PageResult<BookDto>>> Search([FromQuery] Query query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.search)) return BadRequest("Missing search term");
            var result = await _bookService.SearchAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("search-debug")]
        public async Task<IActionResult> SearchDebug([FromQuery] Query query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.search)) return BadRequest("Missing search term");

            var q = query.search.Trim();
            var pageSize = query.pageSize <= 0 ? 10 : Math.Min(query.pageSize, 40);
            var pageNumber = query.pageNumber <= 0 ? 1 : query.pageNumber;
            var startIndex = (pageNumber - 1) * pageSize;

            string NormalizeGoogleBooksBaseUrl(string input)
            {
                var trimmed = input.Trim().TrimEnd('/');
                if (trimmed.EndsWith("/books/v1", StringComparison.OrdinalIgnoreCase)) return trimmed;
                return trimmed + "/books/v1";
            }

            var configuredBase = _config.GetValue<string>("GoogleBooks:BaseUrl");
            var baseUrl = string.IsNullOrWhiteSpace(configuredBase)
                ? "https://www.googleapis.com/books/v1"
                : NormalizeGoogleBooksBaseUrl(configuredBase);
            var apiKey = _config.GetValue<string>("GoogleBooks:ApiKey");

            var url = $"{baseUrl}/volumes?q={Uri.EscapeDataString(q)}&startIndex={startIndex}&maxResults={pageSize}";
            if (!string.IsNullOrEmpty(apiKey)) url += $"&key={apiKey}";

            _logger.LogInformation("[Debug] Calling Google Books Search API: {Url}", url);
            var sw = Stopwatch.StartNew();
            using var resp = await _http.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            sw.Stop();

            int total = 0;
            int itemsCount = 0;
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.TryGetProperty("totalItems", out var ti) && ti.ValueKind == JsonValueKind.Number && ti.TryGetInt32(out var t))
                    total = t;
                if (root.TryGetProperty("items", out var arr) && arr.ValueKind == JsonValueKind.Array)
                    itemsCount = arr.GetArrayLength();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Debug] Error parsing Google response");
            }

            return Ok(new
            {
                url,
                status = (int)resp.StatusCode,
                reason = resp.ReasonPhrase,
                elapsedMs = sw.ElapsedMilliseconds,
                totalItems = total,
                itemsCount,
                length = body?.Length ?? 0,
                sample = body is null ? null : (body.Length > 800 ? body.Substring(0, 800) : body)
            });
        }
    }
}
