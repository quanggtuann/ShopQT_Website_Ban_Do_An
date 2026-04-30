namespace ShopDAL.Models.Dto
{
    public class FoodListResponse
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<FoodItemDto> Data { get; set; }
    }
}