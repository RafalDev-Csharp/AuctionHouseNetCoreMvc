using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Models.ViewModels;
using AuctionHouseApp.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AuctionHouseApp.Areas.Identity.Pages.Account.ExternalLoginModel;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var gameUsers = await _db.GameUserDb.ToListAsync();
            return View(gameUsers);
        }//Index


        //Index 2 -- different version...
        public async Task<IActionResult> UsersRoles()
        {
            return View(await _userManager.Users.ToListAsync());
        }


        //Get BuyCurrency
        public async Task<IActionResult> BuyCurrency()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = await _db.GameUserDb.FindAsync(claim.Value);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction(nameof(Index));
        }

        //Post BuyCurrency
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyCurrency(GameUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.GameUserDb.FirstOrDefaultAsync(u => u.Id == model.Id);

                user.Cash += model.Cash;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        


        public async Task<IActionResult> GiveCashToUser(string id)
        {
            var userToObtainCash = await _db.GameUserDb.FindAsync(id);
            if (userToObtainCash != null)
            {
                return View(userToObtainCash);
            }
            return RedirectToAction(nameof(Index));
        }

        //Post BuyCurrency
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveCashToUser(GameUser model)
        {
            if (ModelState.IsValid)
            {
                var userToObtainCash = await _db.GameUserDb.FirstOrDefaultAsync(u => u.Id == model.Id);
                userToObtainCash.Cash += model.Cash;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        //Lock User
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.GameUserDb.FirstOrDefaultAsync(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Unlock User
        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.GameUserDb.FirstOrDefaultAsync(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveUser(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _db.GameUserDb.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("RemoveUser")]
        public async Task<IActionResult> RemoveUserPost(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _db.GameUserDb.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _db.GameUserDb.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _db.GameUserDb.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            UserAndListOfRolesViewModel model = new UserAndListOfRolesViewModel()
            {
                GameUserVM = user,
                Roles = _roleManager.Roles,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<IActionResult> ChangeRolePost(UserAndListOfRolesViewModel model)
        {
            var gameUserDb = await _db.GameUserDb.FindAsync(model.GameUserVM.Id);

            var user = await _userManager.FindByIdAsync(model.GameUserVM.Id);   //user
            var userRole = _userManager.GetRolesAsync(user).Result;
            var role = _roleManager.FindByNameAsync(userRole[0]).Result;

            var roleName = role.Name;
            if (role.Id == model.GameUserVM.RoleId)
            {
                return View(new UserAndListOfRolesViewModel { GameUserVM = gameUserDb, Roles = _roleManager.Roles });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, userRole[0]);
            if(result.Succeeded)
            {
                var newRole = _roleManager.FindByIdAsync(model.GameUserVM.RoleId).Result;
                result = await _userManager.AddToRoleAsync(user, newRole.Name);
                gameUserDb.RoleId = newRole.Id;
                gameUserDb.RoleName = newRole.Name;

                await _db.SaveChangesAsync();
            }
            
            ///////////////////////////////////////////////////////////////////////////////////////////////////////Changing GameUsers properties: RoleId and RoleName - Only!
            //var userRole = _db.UserRoles.FirstOrDefaultAsync(a => a.UserId == model.GameUserVM.Id).Result;
            //userRole.RoleId = model.GameUserVM.RoleId;

            //var user = _db.GameUserDb.FirstOrDefaultAsync(a => a.Id == model.GameUserVM.Id).Result;
            //user.RoleId = model.GameUserVM.RoleId;
            //var role = _roleManager.FindByIdAsync(model.GameUserVM.RoleId).Result;
            //user.RoleName = role.Name;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////End
            //await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}