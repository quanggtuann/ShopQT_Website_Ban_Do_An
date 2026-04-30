namespace ShopDAL.Models.Dto
{
 public class ComboListResponse
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ToTalPage { get; set; }
        public List<ComboDto> Data { get; set; }
    }
}
