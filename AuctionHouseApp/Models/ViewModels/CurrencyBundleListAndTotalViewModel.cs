using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class CurrencyBundleListAndTotalViewModel
    {
        public IEnumerable<CurrencyBundle> CurrencyBundleList { get; set; }
        public double TotalCash { get; set; }
    }
}
