namespace ReviewBooks.Favorite.Dto
{
    public class FavoriteBookDto
    {
        public string BookId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class AddFavoriteDto
    {
        public string BookId { get; set; } = string.Empty;
    }
}
