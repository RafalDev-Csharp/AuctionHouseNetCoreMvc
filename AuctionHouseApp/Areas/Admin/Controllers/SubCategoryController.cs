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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subcategories = await _db.SubCategories.Include(c=>c.CategoryVP).ToListAsync();
            return View(subcategories);
        }

        //Create get
        public async Task<IActionResult> Create()
        {
            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategoryVM = new Models.SubCategory(),
                SubCategoryListStr = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubExists = _db.SubCategories.Include(c => c.CategoryVP).Where(s => s.Name == model.SubCategoryVM.Name
                                    && s.CategoryVP.Id == model.SubCategoryVM.CategoryId);
                if (doesSubExists.Count() > 0)
                {
                    StatusMessage = $"Error! Subkategoria {doesSubExists.First().Name} in {doesSubExists.First().CategoryVP.Name} ! ";
                }
                else
                {
                    _db.SubCategories.Add(model.SubCategoryVM);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategoryVM = model.SubCategoryVM,
                SubCategoryListStr = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync(),
                StatusMsg = StatusMessage
            };
            return View(modelVM);
        }



        [HttpGet]
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategories
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _db.SubCategories.FirstOrDefaultAsync(s=>s.Id==id);
            if (subcategory == null)
            {
                return NotFound();
            }

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategoryListStr = await _db.SubCategories.OrderBy(s=>s.Name).Select(s=>s.Name).Distinct().ToListAsync(),
                SubCategoryVM = subcategory
            };
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubExists = _db.SubCategories.Include(c => c.CategoryVP).Where(s => s.Name == model.SubCategoryVM.Name
                                    && s.CategoryVP.Id == model.SubCategoryVM.CategoryId);
                if (doesSubExists.Count() > 0)
                {
                    StatusMessage = $"Error! Subkategoria {doesSubExists.First().Name} in {doesSubExists.First().CategoryVP.Name} ! ";
                }
                else
                {
                    var subFromDb = await _db.SubCategories.FindAsync(model.SubCategoryVM.Id);
                    subFromDb.Name = model.SubCategoryVM.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategoryVM = model.SubCategoryVM,
                SubCategoryListStr = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
                StatusMsg = StatusMessage
            };
            return View(modelVM);
        }

        //Delete Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var subcategory = await _db.SubCategories.FindAsync(id);
            var subcategory = await _db.SubCategories.FirstOrDefaultAsync(s => s.Id == id);
            if (subcategory == null)
            {
                return NotFound();
            }
            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategoryVM = subcategory,
                SubCategoryListStr = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync(),
                StatusMsg = StatusMessage
            };
            return View(modelVM);
        }
        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var subcategory = await _db.SubCategories.SingleOrDefaultAsync(s => s.Id == id);
            if (subcategory == null)
            {
                return NotFound();
            }
            _db.Remove(subcategory);
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
            var subcategory = await _db.SubCategories.Include(c => c.CategoryVP).FirstOrDefaultAsync(s => s.Id == id);
            if (subcategory == null)
            {
                return NotFound();
            }
            return View(subcategory);
        }


    }
}