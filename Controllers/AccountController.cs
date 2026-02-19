using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Yash_Gems___Jewelleries.Data;
using Yash_Gems___Jewelleries.Models.Enums;
using Yash_Gems___Jewelleries.ViewModels;
using Yash_Gems___Jewelleries.Interfaces;
using Yash_Gems___Jewelleries.Models;
using Yash_Gems___Jewelleries.Services;

namespace Yash_Gems___Jewelleries.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly INotificationService _notificationService;

        private readonly IImageService _imageService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger,
            IEmailSender emailSender,
            ICompositeViewEngine viewEngine,
            IImageService imageService,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _viewEngine = viewEngine;
            _imageService = imageService;
            _notificationService = notificationService;
        }

        // Render Partial View To String Method
        private async Task<string> RenderPartialViewToString(string viewName, object? model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    viewResult = _viewEngine.GetView(null, viewName, false);
                }

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        // GET: /Account/Login (Login)
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return RedirectToAction("Index", "Home", new { returnUrl, modal = "login" });
        }

        // GET: /Account/Register (Register)
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            return RedirectToAction("Index", "Home", new { returnUrl, modal = "register" });
        }

        // GET: /Account/Index (User Profile)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.OrderCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);
            ViewBag.WishlistCount = await _context.Wishlists.CountAsync(w => w.UserId == user.Id);

            return View(user);
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(Prefix = "Input")] LoginViewModel input, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                // Redirect to Admin Dashboard if user is in Admin role
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    returnUrl = Url.Action(
                        action: "Index",
                        controller: "Dashboard",
                        values: new { area = "Admin" }
                    );
                }

                return Json(new { success = true, redirectUrl = returnUrl });
            }
            if (result.RequiresTwoFactor)
            {
                return Json(new { success = false, message = "Two-factor authentication required. Please use the standard login page or contact support." });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return Json(new { success = false, message = "User account locked out." });
            }
            else
            {
                return Json(new { success = false, message = "Invalid credentials. Please try again." });
            }
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string firstName, string lastName)
        {
            var user = new ApplicationUser 
            { 
                UserName = email, 
                Email = email, 
                FirstName = firstName, 
                LastName = lastName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                // Trigger Notification
                await _notificationService.CreateNotificationAsync(
                    "New Customer Registered",
                    $"New customer {user.FirstName} {user.LastName} ({user.Email}) has joined.",
                    NotificationType.NewCustomerRegistration,
                    user.Id,
                    "System");

                // In production, we will send email verification here
                // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new { area = "Identity", userId = user.Id, code = code }, Request.Scheme);
                // await _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(new { success = true, redirectUrl = Url.Content("~/") });
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return Json(new { success = false, errors = errors });
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl }, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([Bind(Prefix = "Input")] ForgotPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return Json(new { success = true, message = "Please check your email to reset your password." });
                }

                // In production, we will send email here
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = input.Email, code }, protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(input.Email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>clicking here</a>.");

                return Json(new { success = true, message = "Please check your email to reset your password." });
            }

            return Json(new { success = false, message = "Invalid email address." });
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null, string? email = null)
        {
            if (code == null || email == null)
            {
                return RedirectToAction("Index", "Home", new { error = "Invalid password reset token." });
            }

            var model = new ResetPasswordViewModel { Code = code, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new { message = "Your password has been reset." });
            }

            var result = await _userManager.ResetPasswordAsync(user, input.Code, input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { message = "Your password has been reset successfully. Please login to continue." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(input);
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                _logger.LogError($"Error from external provider: {remoteError}");
                return RedirectToAction("Index", "Home", new { error = "Error from external provider" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Error loading external login information.");
                return RedirectToAction("Index", "Home");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation($"{info.Principal.Identity?.Name} logged in with {info.LoginProvider} provider.");
                return Redirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Index", "Home", new { error = "Account locked out" });
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name) ?? "User";
                        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow,
                            EmailConfirmed = true // External logins are usually verified
                        };

                        var createResult = await _userManager.CreateAsync(user);
                        if (createResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "Customer");

                            // Trigger Notification
                            await _notificationService.CreateNotificationAsync(
                                "New Customer Registered",
                                $"New customer {user.FirstName} {user.LastName} ({user.Email}) joined via {info.LoginProvider}.",
                                NotificationType.NewCustomerRegistration,
                                user.Id,
                                info.LoginProvider);

                            createResult = await _userManager.AddLoginAsync(user, info);
                            if (createResult.Succeeded)
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                                return Redirect(returnUrl);
                            }
                        }
                    }
                    else
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(user, info);
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                            return Redirect(returnUrl);
                        }
                    }
                }
                
                return RedirectToAction("Index", "Home", new { error = "Could not link external login." });
            }
        }

        // GET: /Account/Detail (View Profile Details)
        [Authorize]
        public async Task<IActionResult> Detail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new AccountProfileViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Address = user.Address,
                City = user.City,
                State = user.State,
                DateOfBirth = user.DateOfBirth,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            ViewBag.HasPassword = await _userManager.HasPasswordAsync(user);

            return View(model);
        }

        // POST: /Account/Detail (Update Profile Details)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(AccountProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.HasPassword = await _userManager.HasPasswordAsync(user);
                return View(model);
            }

            // Update profile fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.DateOfBirth = model.DateOfBirth;

            // Handle Profile Picture Upload
            if (model.ProfilePictureFile != null)
            {
                try 
                {
                    var profilePictureUrl = await SaveProfilePictureAsync(model.ProfilePictureFile, user.ProfilePictureUrl);
                    if (profilePictureUrl != null)
                    {
                        user.ProfilePictureUrl = profilePictureUrl;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ProfilePictureFile", ex.Message);
                    ViewBag.HasPassword = await _userManager.HasPasswordAsync(user);
                    return View(model);
                }
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            // Handle Password Change
            if (!string.IsNullOrEmpty(model.CurrentPassword) || !string.IsNullOrEmpty(model.NewPassword))
            {
                if (!hasPassword)
                {
                    TempData["Error"] = "Your account is linked to an external provider (e.g., Google/GitHub). You cannot change your password here.";
                }
                else
                {
                    if (string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is required to set a new password.");
                        ViewBag.HasPassword = hasPassword;
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "New password is required.");
                        ViewBag.HasPassword = hasPassword;
                        return View(model);
                    }

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        ViewBag.HasPassword = hasPassword;
                        return View(model);
                    }
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewBag.HasPassword = hasPassword;
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            
            TempData["Success"] = "Profile details updated successfully.";

            return RedirectToAction(nameof(Detail));
        }

        private async Task<string?> SaveProfilePictureAsync(IFormFile file, string? oldPictureUrl)
        {
            // Validate image using centralized service
            if (!_imageService.IsValidImage(file))
            {
                throw new Exception("Invalid image file. Please upload a valid image (jpg, png, webp) under 5MB.");
            }

            // Delete old file if exists and is not the default
            if (!string.IsNullOrEmpty(oldPictureUrl) && !oldPictureUrl.Contains("dummy-user.jpg"))
            {
                _imageService.DeleteImage(oldPictureUrl);
            }

            // Save new file using centralized service
            return await _imageService.SaveImageAsync(file, IImageService.Profile);
        }

        // GET: /Account/Order (View User's Orders)
        [Authorize]
        public async Task<IActionResult> Order()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var model = new CustomerOrderIndexViewModel
            {
                Orders = orders,
            };

            return View(model);
        }

        // GET: /Account/GetOrderDetail/{id}
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return PartialView("_OrderDetailPartial", order);
        }

        // GET: /Account/Inquiries
        [Authorize]
        public async Task<IActionResult> Inquiries()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var inquiries = await _context.Inquiries
                .Include(i => i.Item)
                .Where(i => i.UserId == user.Id)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();

            return View(inquiries);
        }
    }
}
