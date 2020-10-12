using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionHouseApp.Models.ViewModels
{
    public class UserAndListOfRolesViewModel
    {
        public GameUser GameUserVM { get; set; }
        //public IEnumerable<RoleManager<IdentityRole>> RoleList { get; set; }
        public IQueryable<IdentityRole> Roles { get; set; }
    }
}
