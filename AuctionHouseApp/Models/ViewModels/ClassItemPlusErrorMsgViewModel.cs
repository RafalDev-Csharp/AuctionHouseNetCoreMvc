using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class ClassItemPlusErrorMsgViewModel
    {
        public ForWhichClassItem ForWhichClassItem { get; set; }
        public string StatusMessage { get; set; }
    }
}
