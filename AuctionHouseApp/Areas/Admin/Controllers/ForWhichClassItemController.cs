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
    public class ForWhichClassItemController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ForWhichClassItemController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.ForWhichClassItemDb.ToListAsync());
        }


        //Create Get
        public IActionResult Create()
        {
            return View();
        }
        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForWhichClassItem classItem)
        {
            if (ModelState.IsValid)
            {
                _db.ForWhichClassItemDb.Add(classItem);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classItem);
        }


        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var classItem = await _db.ForWhichClassItemDb.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }
            ClassItemPlusErrorMsgViewModel model = new ClassItemPlusErrorMsgViewModel();
            model.ForWhichClassItem = classItem;
            model.StatusMessage = null;
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(ClassItemPlusErrorMsgViewModel classItemVM)
        {
            if (ModelState.IsValid)
            {
                var classItem = await _db.ForWhichClassItemDb.FindAsync(classItemVM.ForWhichClassItem.Id);

                if (classItem == null)
                {
                    return NotFound();
                }
                var doesItemClassExists = await _db.ForWhichClassItemDb.FirstOrDefaultAsync(c => c.Name == classItemVM.ForWhichClassItem.Name);
                if (doesItemClassExists != null)
                {
                    classItemVM.StatusMessage = "Error : Category " + doesItemClassExists.Name + " exists! Please use another name.";
                    return View(classItemVM); //category exists...
                }
                else
                {
                    classItem.Name = classItemVM.ForWhichClassItem.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(classItemVM);
        }



        //Delete Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var classItem = await _db.ForWhichClassItemDb.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }
            return View(classItem);

        }

        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var classItem = await _db.ForWhichClassItemDb.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }
            _db.ForWhichClassItemDb.Remove(classItem);
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
            var classItem = await _db.ForWhichClassItemDb.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }
            return View(classItem);
        }






    }
}