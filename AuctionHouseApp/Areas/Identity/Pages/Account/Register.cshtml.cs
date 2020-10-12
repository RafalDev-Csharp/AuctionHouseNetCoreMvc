using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCore;
using AuctionHouseApp.Data;
using AuctionHouseApp.Models;
using AuctionHouseApp.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace AuctionHouseApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public int Cash { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new GameUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Cash = Input.Cash,
                    Name = Input.Email,
                    
                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //if (!await _roleManager.RoleExistsAsync(SD.GamerUser))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(SD.GamerUser));
                    //}
                    //if (!await _roleManager.RoleExistsAsync(SD.ManagerUser))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser));
                    //}

                    if (role == SD.ManagerUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.ManagerUser);
                        var userRole = _roleManager.FindByNameAsync(SD.ManagerUser).Result;
                        user.RoleId = userRole.Id;
                        user.RoleName = userRole.Name;
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation("User created a new account with password.");
                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.GamerUser);
                        var userRole = _roleManager.FindByNameAsync(SD.GamerUser).Result;
                        user.RoleId = userRole.Id;
                        user.RoleName = userRole.Name;
                        await _db.SaveChangesAsync();
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created a new account with password.");
                        return LocalRedirect(returnUrl);
                    }
                    

                    //_logger.LogInformation("User created a new account with password.");
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                    
                    

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    //}
                    //else
                    //{

                    return LocalRedirect(returnUrl);
                    // }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
