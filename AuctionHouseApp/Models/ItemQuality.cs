using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class ItemQuality
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Quality")]
        public string Name { get; set; }

    }
}
