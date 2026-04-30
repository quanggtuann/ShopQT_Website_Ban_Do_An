using Microsoft.AspNetCore.Mvc.Rendering;
using ShopDAL.Models;
using ShopDAL.Models.Dto;

public class FoodIndexViewModel
{
    public FoodItemFilterViewModel Filter { get; set; }
    public List<FoodItemDto> FoodItems { get; set; }
    public List<SelectListItem> Categories { get; set; }
    public int TotalPage { get; set; }
    public int CurrentPage { get; set; }
    public string ImageBaseUrl { get; set; }

    public FoodIndexViewModel()
    {
        Filter = new FoodItemFilterViewModel();
        FoodItems = new List<FoodItemDto>();
        Categories = new List<SelectListItem>();
    }
}