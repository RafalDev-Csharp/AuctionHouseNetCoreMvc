using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SellItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SellItemController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).Where(u => u.SellerId == claim.Value).ToListAsync();
            //await _db.SellItemDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
            //    .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).ToListAsync();

            foreach (var item in sellItems)
            {
                item.TimeRemained = item.DateTimeEnd - DateTime.Now;

                //if(item.DateTimeEnd <= DateTime.Now)
                //{
                //    //Cancel auction button!!!
                //    Inventory inventory = new Inventory()
                //    {
                //        GameItemId = item.GameItemVP.Id,
                //        GameItemVP = item.GameItemVP,
                //        IdentityUserId = item.SellerId,
                //        UserName = item.SellerName
                //    };
                //    _db.InventoryDb.Add(inventory);
                //    _db.SellItemDb.Remove(item);
                //}
            }
            //await _db.SaveChangesAsync();
            //sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).ToListAsync();
            return View(sellItems);
        }

        //Create Get
        public async Task<IActionResult> PostItem(int inventoryId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var inventory = await _db.InventoryDb.Include(g => g.GameItemVP).FirstOrDefaultAsync(i => i.Id == inventoryId);

            SellItem sellItem = new SellItem()
            {
                SellerId = claim.Value,
                Days = 3,
                Price = 100,
                SellerName = claimsIdentity.Name,
                DateTimeEnd = DateTime.Now,
                GameItemVP = inventory.GameItemVP,
                GameItemId = inventory.GameItemVP.Id,
                TimeRemained = new TimeSpan(0, 0, 0, 0, 0),
                InventoryId = inventory.Id,
                CountOfItemsToSell = 0               
            };
            sellItem.GameItemVP.Count = inventory.Count;

            return View(sellItem);
        }


        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostItem(SellItem sItem)
        {
            if (ModelState.IsValid)
            {
                if ((sItem.CountOfItemsToSell > sItem.GameItemVP.Count) || (sItem.CountOfItemsToSell < 1))
                {
                    return View(sItem);
                }

                var itemFromInventory = await _db.InventoryDb.FindAsync(sItem.InventoryId);
                sItem.DateTimeEnd = DateTime.Now.AddDays(sItem.Days);//czy nie doda dat w przypadku invalid model - datEnd = dateNow na koncu?
                sItem.GameItemVP = await _db.GameItemsDb.Include(c => c.CategoryVP).Include(c => c.SubCategoryVP).Include(c => c.ItemQualityVP)
                    .Include(c => c.ForWhichClassItemVP).FirstOrDefaultAsync(s => s.Id == itemFromInventory.GameItemId);

                int balance = itemFromInventory.Count - sItem.CountOfItemsToSell;
                sItem.GameItemVP.Count = sItem.CountOfItemsToSell;
                _db.SellItemDb.Add(sItem);
                if (balance == 0)
                {
                    _db.InventoryDb.Remove(itemFromInventory);
                }
                else
                {
                    itemFromInventory.Count -= sItem.CountOfItemsToSell; // itemFromInventory.Count = balance;
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //sItem.DateTimeEnd = DateTime.Now;
            return View(sItem);
        }



        public async Task<IActionResult> RemoveItem(int id)
        {
            var item = await _db.SellItemDb.Include(i => i.GameItemVP).FirstOrDefaultAsync(i => i.GameItemId == id);
            //item.SellerId
            Inventory inventory = new Inventory()
            {
                GameItemId = item.GameItemVP.Id,
                GameItemVP = item.GameItemVP,
                GameUserId = item.SellerId,
                UserName = item.SellerName
            };
            inventory.Count = item.GameItemVP.Count;
            _db.InventoryDb.Add(inventory);
            _db.SellItemDb.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }









    }//end class
}