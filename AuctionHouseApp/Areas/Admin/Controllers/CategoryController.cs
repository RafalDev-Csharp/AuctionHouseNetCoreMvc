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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get Index
        public async Task<IActionResult> Index()
        {
            return View(await _db.Categories.ToListAsync());
        }


        //Create Get
        public IActionResult Create()
        {
            return View();
        }
        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }




        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            CategoryPlusErrorMsgViewModel model = new CategoryPlusErrorMsgViewModel();
            model.Category = category;
            model.StatusMessage = null;
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(CategoryPlusErrorMsgViewModel categoryVM)
        {
            if (ModelState.IsValid)
            {
                var cat = await _db.Categories.FindAsync(categoryVM.Category.Id);

                if (cat == null)
                {
                    return NotFound();
                }
                var doesCategoryExists = await _db.Categories.FirstOrDefaultAsync(c => c.Name == categoryVM.Category.Name);
                if (doesCategoryExists != null)
                {
                    categoryVM.StatusMessage = "Error : Category " + doesCategoryExists.Name + " exists! Please use another name.";
                    return View(categoryVM); //category exists...
                }
                else
                {
                    cat.Name = categoryVM.Category.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(categoryVM);
        }

        //Delete Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
            
        }

        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
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
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

    }
}