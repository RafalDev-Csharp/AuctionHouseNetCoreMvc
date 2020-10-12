using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Models.ViewModels;
using AuctionHouseApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class ItemQualityController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ItemQualityController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.ItemQualityDb.ToListAsync());
        }

        //Create Get
        public IActionResult Create()
        {
            return View();
        }
        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemQuality itemQuality)
        {
            if (ModelState.IsValid)
            {
                _db.ItemQualityDb.Add(itemQuality);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemQuality);
        }



        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemQuality = await _db.ItemQualityDb.FindAsync(id);
            if (itemQuality == null)
            {
                return NotFound();
            }
            ItemQualityPlusErrorViewModel model = new ItemQualityPlusErrorViewModel();
            model.ItemQuality = itemQuality;
            model.StatusMessage = null;
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(ItemQualityPlusErrorViewModel itemQualityVM)
        {
            if (ModelState.IsValid)
            {
                var itemQ = await _db.ItemQualityDb.FindAsync(itemQualityVM.ItemQuality.Id);

                if (itemQ == null)
                {
                    return NotFound();
                }
                var doesItemQualityExists = await _db.ItemQualityDb.FirstOrDefaultAsync(c => c.Name == itemQualityVM.ItemQuality.Name);
                if (doesItemQualityExists != null)
                {
                    itemQualityVM.StatusMessage = "Error : Category " + doesItemQualityExists.Name + " exists! Please use another name.";
                    return View(itemQualityVM); //category exists...
                }
                else
                {
                    itemQ.Name = itemQualityVM.ItemQuality.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(itemQualityVM);
        }



        //Delete Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemQuality = await _db.ItemQualityDb.FindAsync(id);
            if (itemQuality == null)
            {
                return NotFound();
            }
            return View(itemQuality);

        }

        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var itemQuality = await _db.ItemQualityDb.FindAsync(id);
            if (itemQuality == null)
            {
                return NotFound();
            }
            _db.ItemQualityDb.Remove(itemQuality);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Details Get
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemQuality = await _db.ItemQualityDb.FindAsync(id);
            if (itemQuality == null)
            {
                return NotFound();
            }
            return View(itemQuality);
        }





    }
}