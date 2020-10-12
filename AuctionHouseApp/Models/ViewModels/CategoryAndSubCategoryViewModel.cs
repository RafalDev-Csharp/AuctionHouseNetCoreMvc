using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class CategoryAndSubCategoryViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategoryVM { get; set; }
        public List<string> SubCategoryListStr { get; set; }
        public string StatusMsg { get; set; }
    }
}
