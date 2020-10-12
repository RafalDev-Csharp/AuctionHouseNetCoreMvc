using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuctionHouseApp.Models;
using AuctionHouseApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AuctionHouseApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.CodeAnalysis.FlowAnalysis;
using ReflectionIT.Mvc.Paging;

namespace AuctionHouseApp.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        static string sortAscDsc = "";
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }





        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).OrderBy(s => s.Id).ToListAsync();
            var categories = await _db.Categories.ToListAsync();
            var subcategories = await _db.SubCategories.Include(c => c.CategoryVP).ToListAsync();
            var itemQualityList = await _db.ItemQualityDb.OrderBy(i=>i.Name).ToListAsync();
            var itemsForWhichClassList = await _db.ForWhichClassItemDb.OrderBy(f=>f.Name).ToListAsync();

            sellItems = UpdateDateAndBackToInventory(sellItems).Result;    //check that some item's date has not expired            

            IndexViewModel indexViewModel = new IndexViewModel()
            {
                SellItemsVM = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                    .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).OrderBy(s => s.Id).ToListAsync(),//if some sellitems dateTimeEnd has expired, we need to update this list.
                CategoriesVM = categories,
                SubCategoriesVM = subcategories,
                MinLvl = 0,
                MaxLvl = 100,
                ItemQualityListVP = itemQualityList,
                ForClassListVP = itemsForWhichClassList,
                PagingListSellItems = PagingList.Create(sellItems, 10, page),
                ItemType = "any",
                ForClass = "any"
            };
            foreach (var item in indexViewModel.SellItemsVM)
            {
                item.TimeRemained = item.DateTimeEnd - DateTime.Now;
            }

            //var query = _db.SellItemDb.AsNoTracking().Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
            //        .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).OrderBy(s => s.Id);
            //var model = await PagingList<SellItem>.CreateAsync(sellItems, 2, page);
            //var x = PagingList.Create(sellItems, 2, page);
            //sellItems = PagingList.Create(sellItems, 2, page);
            return View(indexViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(string sortOrder, IndexViewModel model, int page)
        {
            var doesItemsExists = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).ToListAsync();
            doesItemsExists = UpdateDateAndBackToInventory(doesItemsExists).Result;    //check that some item's date has not expired



            var itemSelected = Request.Form["itemSelected"].ToString();
            var itemQ = Request.Form["itemQ"].ToString();
            int idCategoryOrSubCategory;
            string identifierOfItem = itemSelected[0].ToString();

            List<SellItem> sellItems = new List<SellItem>();
            var categories = await _db.Categories.ToListAsync();
            var subcategories = await _db.SubCategories.Include(c => c.CategoryVP).ToListAsync();
            var itemQualityList = await _db.ItemQualityDb.ToListAsync();
            var itemsForWhichClassList = await _db.ForWhichClassItemDb.ToListAsync();

            if (identifierOfItem == "a")
            {
                sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).ToListAsync();

            }
            else
            {
                idCategoryOrSubCategory = int.Parse(itemSelected.Substring(1, itemSelected.Length - 1));
                if (identifierOfItem == "c")
                {
                    sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                    .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).Where(c => c.GameItemVP.CategoryId == idCategoryOrSubCategory).ToListAsync();
                }
                else if (identifierOfItem == "s")
                {
                    sellItems = await _db.SellItemDb.Include(c => c.GameItemVP).Include(c => c.GameItemVP.ForWhichClassItemVP).Include(i => i.GameItemVP.ItemQualityVP)
                    .Include(c => c.GameItemVP.CategoryVP).Include(s => s.GameItemVP.SubCategoryVP).Where(c => c.GameItemVP.SubCategoryId == idCategoryOrSubCategory).ToListAsync();
                }
            }
            //
            var forclassFilter = _db.ForWhichClassItemDb.Find(Convert.ToInt32(model.ForClass));
            var itemQualityFilter = _db.ItemQualityDb.Find(Convert.ToInt32(model.ItemType));

            //if (model.ForClass.ToLower() != "6")
            if (forclassFilter.Name.ToLower() != "any")
            {
                int idForClassFromModel = Convert.ToInt32(model.ForClass);
                sellItems = sellItems.Where(i => i.GameItemVP.ForWhichClassItemVP.Id == idForClassFromModel).ToList();
            }
            if (itemQualityFilter.Name.ToLower() != "any")
            {
                int idForQuality = Convert.ToInt32(model.ItemType);
                sellItems = sellItems.Where(i => i.GameItemVP.ItemQualityVP.Id == idForQuality).ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                sellItems = sellItems.Where(i => i.GameItemVP.Name.ToLower().Contains(model.Search.ToLower())).ToList();
            }
            if (model.MinLvl > 0 || model.MaxLvl < 100)
            {
                sellItems = sellItems.Where(s => s.GameItemVP.LevelRequired >= model.MinLvl && s.GameItemVP.LevelRequired <= model.MaxLvl).ToList();
            }



            /////////////////////
            foreach (var item in sellItems)
            {
                item.TimeRemained = item.DateTimeEnd - DateTime.Now;
            }

            sellItems = sellItems.Where(s => s.DateTimeEnd > DateTime.Now).ToList();
            SetDirectionOfSorting(ref sellItems, model.SortOrder);  //private method returning sorted list of sellItems.
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                SellItemsVM = sellItems,
                CategoriesVM = categories,
                SubCategoriesVM = subcategories,
                ItemQualityListVP = itemQualityList,
                ForClassListVP = itemsForWhichClassList,
                PagingListSellItems = PagingList.Create(sellItems, 10, page),
            };
            return View(indexViewModel);
        }


        private async Task<List<SellItem>> UpdateDateAndBackToInventory(List<SellItem> sellItems)
        {
            foreach (var item in sellItems)
            {
                if (item.DateTimeEnd <= DateTime.Now)
                {
                    Inventory inventory = new Inventory()
                    {
                        GameItemId = item.GameItemVP.Id,
                        GameItemVP = item.GameItemVP,
                        GameUserId = item.SellerId,
                        UserName = item.SellerName,
                        Count = item.CountOfItemsToSell,
                        GameUserVP = await _db.GameUserDb.FindAsync(item.SellerId)
                    };
                    await _db.InventoryDb.AddAsync(inventory);
                    _db.SellItemDb.Remove(item);
                    //sellItems.Remove(item);
                }
            }
            await _db.SaveChangesAsync();
            return sellItems;
        }

        private void SetDirectionOfSorting(ref List<SellItem> sellItems, string nameOfSorting)
        {
            if (String.IsNullOrEmpty(sortAscDsc))
            {
                sortAscDsc = nameOfSorting;
            }
            if (nameOfSorting != sortAscDsc)
            {
                if ((nameOfSorting + "_desc") != sortAscDsc)
                    sortAscDsc = nameOfSorting;
            }

            switch (sortAscDsc)
            {
                case "Item Name":
                    sellItems = sellItems.OrderByDescending(s => s.GameItemVP.Name).ToList();
                    sortAscDsc = "Item Name_desc";
                    break;
                case "Item Name_desc":
                    sellItems = sellItems.OrderBy(s => s.GameItemVP.Name).ToList();
                    sortAscDsc = "Item Name";
                    break;

                case "Seller Name":
                    sellItems = sellItems.OrderBy(s => s.SellerName).ToList();
                    sortAscDsc = "Seller Name_desc";
                    break;
                case "Seller Name_desc":
                    sellItems = sellItems.OrderByDescending(s => s.SellerName).ToList();
                    sortAscDsc = "Seller Name";
                    break;

                case "Date Time End":
                    sellItems = sellItems.OrderBy(s => s.DateTimeEnd).ToList();
                    sortAscDsc = "Date Time End_desc";
                    break;
                case "Date Time End_desc":
                    sellItems = sellItems.OrderByDescending(s => s.DateTimeEnd).ToList();
                    sortAscDsc = "Date Time End";
                    break;

                case "Price":
                    sellItems = sellItems.OrderBy(s => s.Price).ToList();
                    sortAscDsc = "Price_desc";
                    break;
                case "Price_desc":
                    sellItems = sellItems.OrderByDescending(s => s.Price).ToList();
                    sortAscDsc = "Price";
                    break;

                case "Count":
                    sellItems = sellItems.OrderBy(s => s.CountOfItemsToSell).ToList();
                    sortAscDsc = "Count_desc";
                    break;
                case "Count_desc":
                    sellItems = sellItems.OrderByDescending(s => s.CountOfItemsToSell).ToList();
                    sortAscDsc = "Count";
                    break;
            }
        }



        //Post?
        public async Task<IActionResult> BuyItem(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _db.GameUserDb.FindAsync(claim.Value);

            //var item = await _db.SellItemDb.FindAsync(id);
            var item = await _db.SellItemDb.Include(g => g.GameItemVP).FirstOrDefaultAsync(i => i.Id == id);
            var seller = await _db.GameUserDb.FindAsync(item.SellerId);
            if (user.Cash >= item.Price)
            {
                user.Cash -= item.Price;
                seller.Cash += item.Price;


                Inventory inventory = new Inventory()
                {
                    GameItemId = item.GameItemVP.Id,
                    GameItemVP = item.GameItemVP,
                    GameUserId = claim.Value,
                    UserName = claimsIdentity.Name,
                    Count = item.CountOfItemsToSell
                };
                _db.InventoryDb.Add(inventory);
                _db.SellItemDb.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}











