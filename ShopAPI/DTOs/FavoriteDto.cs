namespace ShopAPI.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public DateTime CreateDate { get; set; }
        public FoodItemDto Food { get; set; } = new();
    }

    public class ToggleFavoriteResultDto
    {
        public bool IsFavorite { get; set; }
        public string Message { get; set; } = string.Empty;
        public FavoriteDto? Favorite { get; set; }
    }
}
