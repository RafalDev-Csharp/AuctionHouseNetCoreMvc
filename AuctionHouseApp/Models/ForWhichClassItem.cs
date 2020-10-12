using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class ForWhichClassItem
    {
       [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Item intended")]
        public string Name { get; set; }
    }
}
