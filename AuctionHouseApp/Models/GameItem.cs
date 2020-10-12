using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class GameItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LevelRequired { get; set; }
        
        public string Image { get; set; }


        [Required]
        [Display(Name ="SubCategory")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategoryVP { get; set; }

        [Required]
        [Display(Name ="ItemQuality")]
        public int ItemQualityId { get; set; }
        [ForeignKey("ItemQualityId")]
        public virtual ItemQuality ItemQualityVP { get; set; }

        [Required]
        [Display(Name ="ClassItem")]
        public int ForWhichClassItemId { get; set; }
        [ForeignKey("ForWhichClassItemId")]
        public virtual ForWhichClassItem ForWhichClassItemVP { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category CategoryVP { get; set; }


        [Range(1, 999, ErrorMessage = "Count should be between 1 and 999")]
        public int Count { get; set; } = 1;

        [Required]
        [Range(0, 100000, ErrorMessage = "Price should be between 0 and 100k")]
        public int PremiumCurrencyPrice { get; set; }



    }
}
