using System;
using System.Collections.Generic;

namespace Yash_Gems___Jewelleries.Models.ViewModels
{
    public class ShopFilterViewModel
    {
        // Selected filters
        public int[]? BrandIds { get; set; }
        public int[]? CategoryIds { get; set; }
        public int[]? GoldTypeIds { get; set; }
        public string[]? StoneColors { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Availability { get; set; } // "inStock", "outStock"
        public string? SortBy { get; set; }
        public string? SearchQuery { get; set; }
        public int Page { get; set; } = 1;

        // Display Data & Counts
        public List<FilterOption> Categories { get; set; } = new();
        public List<FilterOption> Brands { get; set; } = new();
        public List<FilterOption> Materials { get; set; } = new();
        public List<FilterOption> StoneColorsList { get; set; } = new();
        
        public int InStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        
        // The actual items for the current page
        public IEnumerable<Yash_Gems___Jewelleries.Models.Item> Items { get; set; } = new List<Yash_Gems___Jewelleries.Models.Item>();
        public List<string> WishlistStyleCodes { get; set; } = new();
    }

    public class FilterOption
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty; // Used for non-int IDs like Stone Color
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsSelected { get; set; }
        public string? ImageUrl { get; set; } // For color icons
    }
}
