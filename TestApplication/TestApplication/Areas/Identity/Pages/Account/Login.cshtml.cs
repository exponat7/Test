using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TestApplication.Areas.Identity.Models;
using TestApplication.Areas.Identity.Managers;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace TestApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly CustomSignInManager<CustomIdentityUser> _signInManager;
        private readonly CustomUserManager<CustomIdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(CustomUserManager<CustomIdentityUser> userManager, CustomSignInManager<CustomIdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле \"{0}\" обязательно")]
            //[EmailAddress]
            [Display(Name = "Логин")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Поле \"{0}\" обязательно")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.Email);
                if (_signInManager.IsUserLoggedIn(user))
                {
                    ModelState.AddModelError(string.Empty, "Уже есть подключение по данному логину с другой рабочей станции.");
                    _logger.LogWarning("Попытка входа под одной учетной записью с разных рабочих станций");
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неудачная попытка.");
                    //ViewData["type"] = 3;
                    //ViewData["message"] = "Неудачная попытка входа.";
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
