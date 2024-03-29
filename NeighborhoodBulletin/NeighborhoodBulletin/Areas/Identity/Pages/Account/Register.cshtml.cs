﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using static Domain.ApplicationDbContext;
using NeighborhoodBulletin.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain;

namespace NeighborhoodBulletin.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectListItem> UserRoles { get; private set; }

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

            [Required]
            public string Role { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            UserRoles = new List<SelectListItem>()
            {
                new SelectListItem { Value = "ShopOwner", Text = "Shop Owner" },
                new SelectListItem { Value = "Neighbor", Text = "Neighbor" },
            }; 
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                //    var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };

                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Role = Input.Role,
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(StaticDetails.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Neighbor))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(StaticDetails.Neighbor));

                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.ShopOwner))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(StaticDetails.ShopOwner));
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                if(user.Role == "ShopOwner")
                {
                    return RedirectToAction("Create", "ShopOwners");
                }
                if(user.Role == "Neighbor")
                {
                    return RedirectToAction("Create", "Neighbors");
                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Home");
        }
    }
}
