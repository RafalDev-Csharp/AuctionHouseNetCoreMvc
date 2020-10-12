using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class CategoryPlusErrorMsgViewModel
    {
        public Category Category { get; set; }
        public string StatusMessage { get; set; }
    }
}
