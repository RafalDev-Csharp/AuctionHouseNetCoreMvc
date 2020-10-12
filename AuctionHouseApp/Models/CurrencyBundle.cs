using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class CurrencyBundle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GameCurrencyAmount { get; set; }
        [Required]
        public int BonusCurrency { get; set; }
        [Required]
        public double CashValue { get; set; }
    }
}
