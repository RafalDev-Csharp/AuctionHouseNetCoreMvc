using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public InventoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Inventory> listInventory = new List<Inventory>();
            var inventory = _db.InventoryDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).Where(c => c.GameUserId == claim.Value);
            //if (inventory != null)
            if (inventory.Count() > 0)
            {
                listInventory = inventory.ToList();
                listInventory.First().GameUserId = claim.Value;
                listInventory.First().UserName = claimsIdentity.Name;
                //listInventory.First().Count = listInventory.Count();
                return View(listInventory);
            }
            return View();

        }



        //AddToInventory Get
        public async Task<IActionResult> AddToInventory(int gameItemId)
        {
            var gameItem = await _db.GameItemsDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
                .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).FirstOrDefaultAsync(c => c.Id == gameItemId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = await _db.GameUserDb.FindAsync(claim.Value);
            var usersInventory = await _db.InventoryDb.Where(u => u.GameUserId == user.Id).ToListAsync();

            Inventory inventory = new Inventory()
            {
                Count = 0,
                GameItemId = gameItemId,
                GameItemVP = gameItem,
                GameUserId = claim.Value,
                UserName = claimsIdentity.Name

            };
            var gameItemFromUsersInventory = usersInventory.FirstOrDefault(i => i.GameItemId == gameItemId);
            if (gameItemFromUsersInventory != null)
            {
                inventory.GameItemVP.Count = gameItemFromUsersInventory.Count;

            }
            return View(inventory);
        }

        //post?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToInventory(Inventory inventoryModel)//model invItem??? inv.Id Find(), inv.Count = model.count;
        {
            if (ModelState.IsValid)
            {
                var gameItem = await _db.GameItemsDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
                    .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).FirstOrDefaultAsync(c => c.Id == inventoryModel.GameItemId);

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //
                var user = await _db.GameUserDb.FindAsync(claim.Value);
                if (user.Cash >= gameItem.PremiumCurrencyPrice * inventoryModel.Count)
                {
                    Inventory inventory = new Inventory()
                    {
                        Count = inventoryModel.Count,
                        GameItemId = inventoryModel.GameItemId,
                        GameItemVP = gameItem,
                        GameUserId = claim.Value,
                        UserName = claimsIdentity.Name

                    };
                    user.Cash -= gameItem.PremiumCurrencyPrice * inventoryModel.Count;
                    _db.InventoryDb.Add(inventory);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(inventoryModel);
        }


        //DeleteItem
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id != null)
            {
                var itemToDelete = await _db.InventoryDb.FindAsync(id);
                _db.InventoryDb.Remove(itemToDelete);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }






    }
}