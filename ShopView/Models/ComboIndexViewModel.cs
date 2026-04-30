using ShopDAL.Models.Dto;
using ShopDAL.Models;

namespace ShopView.Models
{
    public class ComboIndexViewModel
    {
        public ComboFilterViewmodel Filter { get; set; }
        public List<ComboDto> Combos { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string ImageBaseUrl { get; set; }

        public ComboIndexViewModel()
        {
            Filter = new ComboFilterViewmodel();
            Combos = new List<ComboDto>();
        }
    }
}
