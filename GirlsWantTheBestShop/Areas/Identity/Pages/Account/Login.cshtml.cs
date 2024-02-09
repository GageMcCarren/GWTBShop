// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GirlsWantTheBestShop.Data;
using GirlsWantTheBestShop.Models;

namespace GirlsWantTheBestShop.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger,UserManager<IdentityUser> user, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = user;
            _context = context;
        }

      
        [BindProperty]
        public InputModel Input { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        
        public string ReturnUrl { get; set; }

       
        [TempData]
        public string ErrorMessage { get; set; }

      
        public class InputModel
        {
         
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

       
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            _logger.LogInformation("Starting the sign-in process.");

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid. Attempting to sign in user: {Email}", Input.Email);
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    _logger.LogInformation("Sign-in succeeded for user: {Email}", Input.Email);
                    var userInfo = _context.ApplicationUsers.FirstOrDefault(c=>c.UserName.ToLower() == Input.Email.ToLower());
                    if (userInfo == null)
                    {
                        _logger.LogWarning("User info not found for: {Email}", Input.Email);
                    }
                    else
                    {
                        _logger.LogInformation("User info retrieved for: {Email}", Input.Email);
                    }



                    var roleInfo = (from ur in _context.UserRoles
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where ur.UserId == userInfo.Id
                                    select new SessionUserVm()
                                   {
                                       UserName = Input.Email,
                                       RoleName = r.Name

                                   }).FirstOrDefault(); 

                       if(roleInfo!=null)
                       {
                        _logger.LogInformation("Role info found for user: {Email}. Role: {Role}", Input.Email, roleInfo.RoleName);
                       
                        HttpContext.Session.SetString("roleName", roleInfo.RoleName);  
                       }
                    else
                    {
                        _logger.LogWarning("Role info not found for user: {Email}", Input.Email);
                    }
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("Sign-in for user: {Email} requires two-factor authentication.", Input.Email);
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account is locked out: {Email}", Input.Email);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid.");
            }
            return Page();
        }

            // If we got this far, something failed, redisplay form
            
        
    }
}
