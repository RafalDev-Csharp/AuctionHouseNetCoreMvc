using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<SellItem> SellItemsVM { get; set; }
        public IEnumerable<Category> CategoriesVM { get; set; }
        public IEnumerable<SubCategory> SubCategoriesVM { get; set; }
        public SubCategory SubCategoryVM { get; set; }
        public IEnumerable<ForWhichClassItem> ForClassListVP { get; set; }
        public IEnumerable<ItemQuality> ItemQualityListVP { get; set; }
        [Range(0, 100)]
        public int MinLvl { get; set; }
        [Range(0, 100)]
        public int MaxLvl { get; set; }
        public string ForClass { get; set; }
        public string ItemType { get; set; }
        //public string CategorySelected { get; set; }
        //public string SubCategorySelected { get; set; }
        public string Search { get; set; } = "";//isnullorempty.. change this.
        public string SortOrder { get; set; }
        public PagingList<SellItem> PagingListSellItems { get; set; }
    }
}
