using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Models.ViewModels;
using AuctionHouseApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class GameItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public GameItemViewModel GameItemVM { get; set; }

        public GameItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            GameItemVM = new GameItemViewModel()
            {
                GameItemPVM = new Models.GameItem(),
                CategoryList = _db.Categories,
                ForWhichClassItemsList = _db.ForWhichClassItemDb,
                ItemQualityList = _db.ItemQualityDb
            };
        }

        public async Task<IActionResult> Index()
        {
            var gameItems = await _db.GameItemsDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
                .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).ToListAsync();
            return View(gameItems);
        }


        //Create Get
        public IActionResult Create()
        {
            return View(GameItemVM);
        }

        //Create Post
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            GameItemVM.GameItemPVM.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            
            if (!ModelState.IsValid)
            {
                return View(GameItemVM);
            }
            
            _db.GameItemsDb.Add(GameItemVM.GameItemPVM);
            await _db.SaveChangesAsync();

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var gameItemFromDb = await _db.GameItemsDb.FindAsync(GameItemVM.GameItemPVM.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, GameItemVM.GameItemPVM.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                gameItemFromDb.Image = @"\images\" + GameItemVM.GameItemPVM.Id + extension;
            }
            else
            {
                //no file was uploaded - use default image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + GameItemVM.GameItemPVM.Id + ".png");
                gameItemFromDb.Image = @"\images\" + GameItemVM.GameItemPVM.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameItemVM.GameItemPVM = await _db.GameItemsDb.Include(c => c.CategoryVP)
                .Include(s => s.SubCategoryVP).Include(i => i.ItemQualityVP)
                .Include(f => f.ForWhichClassItemVP).SingleOrDefaultAsync(g => g.Id == id);
            GameItemVM.SubCategoryList = await _db.SubCategories.Where(s => s.CategoryId == GameItemVM.GameItemPVM.CategoryId).ToListAsync();
        
            if (GameItemVM.GameItemPVM == null)
            {
                return NotFound();
            }
            return View(GameItemVM);
        }

        //Edit - Post
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameItemVM.GameItemPVM.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                GameItemVM.SubCategoryList = await _db.SubCategories.Where(s => s.CategoryId == GameItemVM.GameItemPVM.Id).ToListAsync();
                return View(GameItemVM);
            }

            //Saving image:
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var gameItemFromDb = await _db.GameItemsDb.FindAsync(GameItemVM.GameItemPVM.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //delete origin path file
                var imagePath = Path.Combine(webRootPath, gameItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //upload file
                using (var fileStream = new FileStream(Path.Combine(uploads, GameItemVM.GameItemPVM.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                gameItemFromDb.Image = @"\images\" + GameItemVM.GameItemPVM.Id + extension_new;
            }
            gameItemFromDb.Name = GameItemVM.GameItemPVM.Name;
            gameItemFromDb.LevelRequired = GameItemVM.GameItemPVM.LevelRequired;
            gameItemFromDb.CategoryId = GameItemVM.GameItemPVM.CategoryId;
            gameItemFromDb.SubCategoryId = GameItemVM.GameItemPVM.SubCategoryId;
            gameItemFromDb.ItemQualityId = GameItemVM.GameItemPVM.ItemQualityId;
            gameItemFromDb.ForWhichClassItemId = GameItemVM.GameItemPVM.ForWhichClassItemId;
            gameItemFromDb.PremiumCurrencyPrice = GameItemVM.GameItemPVM.PremiumCurrencyPrice;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Delete - Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GameItemVM.GameItemPVM = await _db.GameItemsDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
                .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).SingleOrDefaultAsync(g => g.Id == id);
            //var gameItemFromDb = await _db.GameItemsDb.FindAsync(GameItemVM.GameItemPVM.Id);
            if (GameItemVM.GameItemPVM == null)
            {
                return NotFound();
            }
            return View(GameItemVM);
        }

        //Delete - Post
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            GameItem gameItem = await _db.GameItemsDb.FindAsync(id);

            if (gameItem != null)
            {
                var imagePath = Path.Combine(webRootPath, gameItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.GameItemsDb.Remove(gameItem);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //Details get
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GameItemVM.GameItemPVM = await _db.GameItemsDb.Include(c => c.ForWhichClassItemVP).Include(i => i.ItemQualityVP)
                .Include(c => c.CategoryVP).Include(s => s.SubCategoryVP).SingleOrDefaultAsync(g => g.Id == id);
            if (GameItemVM.GameItemPVM == null)
            {
                return NotFound();
            }
            return View(GameItemVM);
        }




    }
}