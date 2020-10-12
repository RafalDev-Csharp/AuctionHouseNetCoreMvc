using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models
{
    public class GameUser : IdentityUser
    {
        [Range(1, int.MaxValue, ErrorMessage = "Currency amount should be greater than 1")]
        public int Cash { get; set; }
        public string Name { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}
