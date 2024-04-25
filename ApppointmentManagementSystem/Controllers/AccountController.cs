using AppointmentManagementSystem.Common;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Numerics;
using AppointmentManagementSystem.Manager;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Operations;

namespace AppointmentManagementSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly VisitorManager _visitorManager;

        //private readonly IUserStore<IdentityUser> _userStore;
        //private readonly IUserEmailStore<IdentityUser> _emailStore;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _visitorManager = new VisitorManager(db);

        }
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")

        {
            ViewBag.Error = TempData["Error"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVm loginVm)
        {
            var result = _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, lockoutOnFailure: false).Result;
            if (result.Succeeded)
            {

                var user = _userManager.FindByEmailAsync(loginVm.Email).Result;
                IdentityModel model = user as IdentityModel;
                if (model?.IsActive == false)
                {
                    TempData["Error"] = "Invalid Login attempt";
                    return RedirectToAction("Login");
                }
                if (user is UserInfo { UserType: "UserInfo" })
                {

                    //if (loginVm.ReturnUrl != "/")
                    //{
                    //    return Redirect(loginVm.ReturnUrl);
                    //}
                    return RedirectToAction("Dashboard", "Home");
                }

                if (user is Doctor { UserType: "Doctor" })
                {
                    //if (loginVm.ReturnUrl != "/")
                    //{
                    //    return Redirect(loginVm.ReturnUrl);
                    //}
                    return RedirectToAction("DoctorDashboard", "Home");
                }


                //HttpContext.Response.Cookies.Delete("UserId");
                TempData["Error"] = "Invalid Login attempt";
                return RedirectToAction("Login");

            }

            if (result.IsLockedOut)
            {
                // _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid loginVm attempt.");
                return RedirectToAction("Login");
            }

        }
        [HttpGet]
        public IActionResult VisitorLogin(string returnUrl = "/")
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult VisitorLogin(LoginVm loginVm)
        {

            loginVm.ReturnUrl = loginVm.ReturnUrl.Replace("%3F", "?").Replace("%3D", "=").Replace("%26", "&");

            var result = _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, lockoutOnFailure: false).Result;
            if (result.Succeeded)
            {

                var user = _userManager.FindByNameAsync(loginVm.Email).Result;
                IdentityModel model = user as IdentityModel;
                if (model?.IsActive == false)
                {
                    TempData["Error"] = "Invalid Login attempt";
                    return RedirectToAction("Login");
                }

                if (user is Visitor { UserType: "Visitor" })
                {

                    return Redirect(loginVm.ReturnUrl);

                }
                //HttpContext.Response.Cookies.Delete("UserId");
                TempData["Error"] = "Invalid Login attempt";
                return RedirectToAction("Login");

            }

            if (result.IsLockedOut)
            {
                // _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid loginVm attempt.");
                return RedirectToAction("Login");
            }

        }


        public IActionResult Register(string visitorId, string returnUrl = "/")
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.ReturnUrl = returnUrl;

            if (visitorId != "")
            {
                var visitor = _visitorManager.GetById(visitorId);
                return View(visitor);
            }

            return View();
        }
        [HttpPost]
        public IActionResult Register(Visitor visitor, string password, IFormFile image, string returnUrl)
        {
            try
            {
                if (password != null && visitor.Mobile != null)
                {
                    if (visitor.Mobile.Length != 11)
                    {
                        TempData["Error"] = "Mobile number must be exactly 11 digits.";
                        return RedirectToAction("Register");
                    }
                    var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
                    var regex = new Regex(pattern);

                    if (!regex.IsMatch(password))
                    {
                        TempData["Error"] = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.";
                        return RedirectToAction("Register");
                    }
                }

                var path = _configuration.GetValue<string>("ImagePath");
                var visitorRole = _configuration.GetValue<string>("Role:Visitor");

                if (string.IsNullOrEmpty(visitor.Id))
                {

                    var alreadyVisitor = _visitorManager.GetByMobile(visitor.Mobile);
                    if (alreadyVisitor != null)
                    {
                        TempData["Error"] = "Already same mobile user exists. Please provide a new mobile number.";
                        return RedirectToAction("Register");
                    }


                    var visitorUser = new Visitor();

                    var id = visitorUser.Id;
                    var conStamp = visitor.ConcurrencyStamp;
                    var secStamp = visitor.SecurityStamp;
                    visitorUser = visitor;



                    visitorUser.Id = id;
                    visitorUser.ConcurrencyStamp = conStamp;
                    visitorUser.SecurityStamp = secStamp;

                    visitorUser.IsActive = true;
                    visitorUser.CreatedAt = DateTime.Now;
                    visitorUser.CreatedBy = User.Identity?.Name;
                    if (image != null)
                    {
                        visitorUser.Image = Utility.SaveFile(image, path);
                    }
                    visitorUser.UserType = "Visitor";
                    visitor.UserName = visitor.Mobile;
                    visitorUser.RoleId = visitorRole;
                    visitorUser.EmailConfirmed = true;
                    var userAdd = _userManager.CreateAsync(visitor, password).Result;
                    if (userAdd.Succeeded)
                    {
                        var result = _signInManager.PasswordSignInAsync(visitor.Mobile, password, false, lockoutOnFailure: false).Result;
                        if (result.Succeeded)
                        {
                            TempData["Success"] = "Registration Successful";
                            return Redirect(returnUrl);
                        }

                    }
                    TempData["Success"] = "Failed to registration.";
                    return RedirectToAction("Register");

                }
                else
                {
                    var oldData = _visitorManager.GetById(visitor.Id);

                    if (oldData.Mobile != visitor.Mobile)
                    {
                        var alreadyVisitor = _visitorManager.GetByMobile(visitor.Mobile);
                        if (alreadyVisitor != null)
                        {
                            TempData["Error"] = "Already same mobile user exists. Please provide a new mobile number.";
                            return RedirectToAction("Register");
                        }
                    }
                    if (oldData != null)
                    {
                        oldData.Name = visitor.Name;
                        if (visitor.Mobile != null)
                        {
                            oldData.Mobile = visitor.Mobile;
                            oldData.UserName = visitor.Mobile;
                        }

                        if (visitor.DOB != null)
                        {
                            oldData.DOB = visitor.DOB;
                        }
                        oldData.Gender = visitor.Gender;
                        oldData.Address = visitor.Address;
                        oldData.Email = visitor.Email;
                        oldData.UpdatedAt = DateTime.Now;
                        oldData.UpdatedBy = User.Identity?.Name;
                        if (image != null)
                        {
                            oldData.Image = Utility.SaveFile(image, path);
                        }

                        var userUpdate = _visitorManager.Update(oldData);
                        if (userUpdate)
                        {
                            TempData["Success"] = "Successfully Update Added.";
                            return Redirect(returnUrl);

                        }
                        TempData["Error"] = "Failed to update.";
                        return RedirectToAction("Register");
                    }
                }
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to operation.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult MobileGet(string returnUrl = "/")
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult MobileGet(string mobile, string returnUrl)
        {
            var alreadyVisitor = _visitorManager.GetByMobile(mobile);
            if (alreadyVisitor == null)
            {
                TempData["Error"] = "No registered users found with this number";
                return RedirectToAction("MobileGet");
            }
            return RedirectToAction("GetOTP", "Account", new { visitorId = alreadyVisitor.Id, returnUrl = returnUrl });
        }
        public IActionResult GetOTP(string visitorId, string returnUrl)
        {
            ViewBag.VisitorId = visitorId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetOTP(string OTP, string visitorId, string returnUrl)
        {
            if (!string.IsNullOrEmpty(visitorId))
            {
                var user = _userManager.Users.FirstOrDefault(x => x.Id == visitorId);

                if (user != null)
                {
                    //var res= await SignInOrTwoFactorAsync(user, false, false, false);
                    // var result = _signInManager.PasswordSignInAsync(user.Id, null, false, lockoutOnFailure: false).Result;

                    // // Check if the user is successfully signed in
                    // //if (result)
                    // //{
                    // //    return Redirect(returnUrl);
                    // //}
                    return Redirect(returnUrl);
                }
            }

            // If user is not found or sign-in is not successful, return to the view
            return View();
        }


        //[Route("/doctor")]
        //public IActionResult DoctorLogin()
        //{
        //    ViewBag.Error= TempData["Error"];
        //    return View();
        //}

        //public IActionResult DoctorLogin(LoginVm login)
        //{
        //    var result = _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: false).Result;
        //    if (result.Succeeded)
        //    {
        //        var user = _userManager.FindByEmailAsync(login.Email).Result;
        //        if (user is Doctor { UserType: "Doctor" })
        //        {
        //            return RedirectToAction("DoctorDashboard", "Home");
        //        }

        //        TempData["Error"] = "Invalid Login attempt";
        //        return RedirectToAction("DoctorLogin");

        //    }

        //    if (result.IsLockedOut)
        //    {
        //        // _logger.LogWarning("User account locked out.");
        //        return RedirectToPage("./Lockout");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid loginVm attempt.");
        //        return RedirectToAction("StaffLogin");
        //    }
        //}
        [Route("/visitor")]
        public IActionResult VisitorLogin()
        {
            return View();
        }
    }
}
