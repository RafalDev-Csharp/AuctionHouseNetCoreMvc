using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class SellItem
    {
        public int Id { get; set; }
        public string SellerId { get; set; }
        public int Days { get; set; }

        [Required]
        [Display(Name = "Game Item")]
        public int GameItemId { get; set; }
        [ForeignKey("GameItemId")]
        public virtual GameItem GameItemVP { get; set; }

        //date time and hours;
        public DateTime DateTimeEnd { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd\\.hh\\:mm\\:ss}")]
        public TimeSpan TimeRemained { get; set; }

        public int Price { get; set; }
        public string SellerName { get; set; }
        public int CountOfItemsToSell { get; set; }
        public int InventoryId { get; set; }

    }
}
