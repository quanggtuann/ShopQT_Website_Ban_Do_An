using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShopView.Areas.Admin.Models
{
    // For Food Index
    public class FoodIndexViewModel
    {
        public FoodItemFilterViewModel Filter { get; set; }
        public List<FoodItemDto> FoodItems { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string ImageBaseUrl { get; set; }
    }

    public class FoodItemFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? categoryID { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class FoodItemDto
    {
        public int FoodItemID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    // For Create/Edit
    public class FoodItemCreateViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    // Response from API
    public class FoodListResponse
    {
        public List<FoodItemDto> Data { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    // Category DTO for dropdown
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
