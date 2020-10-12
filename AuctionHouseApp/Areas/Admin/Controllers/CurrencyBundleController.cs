using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace AuctionHouseApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CurrencyBundleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        public CurrencyBundleController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.CurrencyBundleDb.ToListAsync());
        }


        //Create Get
        public IActionResult Create()
        {
            return View();
        }
        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CurrencyBundle currencyBundle)
        {
            if (ModelState.IsValid)
            {
                _db.CurrencyBundleDb.Add(currencyBundle);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(currencyBundle);
        }




        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            if (currencyBundle == null)
            {
                return NotFound();
            }
            CurrencyBundlePlusErrorViewModel model = new CurrencyBundlePlusErrorViewModel();
            model.CurrencyBundlePVM = currencyBundle;
            model.StatusMessage = null;
            return View(model);
        }

        //Edit Post
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(CurrencyBundlePlusErrorViewModel currencyBundle)
        {
            if (ModelState.IsValid)
            {
                var currencyBundleDb = await _db.CurrencyBundleDb.FindAsync(currencyBundle.CurrencyBundlePVM.Id);

                if (currencyBundleDb == null)
                {
                    return NotFound();
                }
                var doesCurrencyBundleExists = await _db.CurrencyBundleDb.FirstOrDefaultAsync(c => c.CashValue == currencyBundle.CurrencyBundlePVM.CashValue);
                if (doesCurrencyBundleExists != null)
                {
                    currencyBundle.StatusMessage = "Error : Currency bundle " + doesCurrencyBundleExists.CashValue + " exists! Please use another Cash Value.";
                    return View(currencyBundle); //category exists...
                }
                else
                {
                    currencyBundleDb.CashValue = currencyBundle.CurrencyBundlePVM.CashValue;
                    currencyBundleDb.GameCurrencyAmount = currencyBundle.CurrencyBundlePVM.GameCurrencyAmount;
                    currencyBundleDb.BonusCurrency = currencyBundle.CurrencyBundlePVM.BonusCurrency;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(currencyBundle);
        }

        //Delete Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            if (currencyBundle == null)
            {
                return NotFound();
            }
            return View(currencyBundle);

        }

        //Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            if (currencyBundle == null)
            {
                return NotFound();
            }
            _db.CurrencyBundleDb.Remove(currencyBundle);
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
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            if (currencyBundle == null)
            {
                return NotFound();
            }
            return View(currencyBundle);
        }


        [HttpGet]
        public async Task<IActionResult> CurrencyBundleList()
        {
            CurrencyBundleListAndTotalViewModel model = new CurrencyBundleListAndTotalViewModel();
            model.CurrencyBundleList = await _db.CurrencyBundleDb.ToListAsync();
            model.TotalCash = 100;
            return View(model);
        }

        //Currency Bundle Buy!!! --- Get
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CurrencyBundleList")]
        public async Task<IActionResult> CurrencyBundleListPost()
        {
            CurrencyBundleListAndTotalViewModel model = new CurrencyBundleListAndTotalViewModel();
            model.CurrencyBundleList = await _db.CurrencyBundleDb.ToListAsync();

            string itemSelected = Request.Form["itemSelected"].ToString();
            if (String.IsNullOrEmpty(itemSelected))
            {
                return NotFound();
            }
            int id = Int32.Parse(itemSelected);

            return RedirectToAction("BuyCurrencyBundle", "CurrencyBundle", new { id });
        }

        public async Task<IActionResult> BuyCurrencyBundle(int id)//id not needed.
        {
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            if(currencyBundle == null)
            {
                return NotFound();
            }
            return View(currencyBundle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyCurrencyBundle(string stripeToken, int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _db.GameUserDb.FindAsync(claim.Value);
            
            var currencyBundle = await _db.CurrencyBundleDb.FindAsync(id);
            

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(currencyBundle.CashValue * 100),
                Currency = "EUR",
                Description = "Order ID : " + currencyBundle.Id,
                SourceId = stripeToken
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                return NotFound();
            }
            
            

            if (charge.Status.ToLower() != "succeeded")
            {
                await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == claim.Value).FirstOrDefault().Email, "AuctionHouse - Shopping Summary:Some Error was occured. Please try again.",
                    "If Your problem occured again, pleas inform us sending an email to rafalblaszczykdev@gmail.com - Auction House");
                return NotFound();
            }
            user.Cash += (currencyBundle.GameCurrencyAmount + currencyBundle.BonusCurrency);
            var userEmail = await _db.Users.Where(u => u.Id == claim.Value).FirstOrDefaultAsync();
            await _emailSender.SendEmailAsync(userEmail.UserName,
                "AuctionHouse - Shopping Summary",
                "You paid: " + currencyBundle.CashValue+ "$" + ",  You bought: " + currencyBundle.GameCurrencyAmount + " Coins, and Yours bonus is: " + currencyBundle.BonusCurrency
                   + " Coins, in total You receive: " + (currencyBundle.GameCurrencyAmount + currencyBundle.BonusCurrency) +
                "Coins. Thank You! Have fun and Go back if You need more! - Auction House");
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

    }
}