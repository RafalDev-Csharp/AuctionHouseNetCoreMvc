using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class GameItemViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<SubCategory> SubCategoryList { get; set; }
        public IEnumerable<ForWhichClassItem> ForWhichClassItemsList { get; set; }
        public IEnumerable<ItemQuality> ItemQualityList { get; set; }
        public GameItem GameItemPVM { get; set; }
    }
}
