using AppointmentManagementSystem.Common;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.SecurityExtension;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserInfoManager _userInfoManager;
        private readonly IBranchManager _branchManager;
        private readonly IDesignationManager _designationManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager _roleManager;
        public UserController(ApplicationDbContext db, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _userInfoManager = new UserInfoManager(db);
            _branchManager = new BranchManager(db);
            _designationManager = new DesignationManager(db);
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = new RoleManager(db, userManager);
        }
        public IActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var role = _roleManager.GetRoleByUserId(userId);
            //var branches = _branchManager.GetByUserId(userId);
            var branches = _branchManager.GetAll();
            ViewBag.Branches = branches;
            ViewBag.DesignationList = _designationManager.GetActive();
            ViewBag.RoleList = _roleManager.GetRoles(role?.OrderNo ?? 1);
            return View();
        }

        [HttpPost]
        public IActionResult Add(UserInfo userInfo, string Password, IFormFile Image)
        {
            try
            {
                var path = _configuration.GetValue<string>("ImagePath");

                if (userInfo.Id == null)
                {
                    var userInfoObj = new UserInfo();

                    var id = userInfoObj.Id;
                    var conStamp = userInfo.ConcurrencyStamp;
                    var secStamp = userInfo.SecurityStamp;
                    userInfoObj = userInfo;



                    userInfoObj.Id = id;
                    userInfoObj.ConcurrencyStamp = conStamp;
                    userInfoObj.SecurityStamp = secStamp;

                    userInfoObj.IsActive = true;
                    userInfoObj.CreatedAt = DateTime.Now;
                    userInfoObj.CreatedBy = User.Identity.Name;
                    userInfoObj.Image = Utility.SaveFile(Image, path);
                    userInfoObj.UserType = "UserInfo";
                    userInfoObj.UserName = userInfo.Email;
                    userInfoObj.EmailConfirmed = true;
                    var userAdd = _userManager.CreateAsync(userInfo, Password).Result;
                    if (userAdd.Succeeded)
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully Added User" });

                    }
                    return Ok(new { success = false, ErrorMessage = "Failed to add." });

                }
                else
                {
                    var oldData = _userInfoManager.GetById(userInfo.Id);
                    if (oldData != null)
                    {
                        oldData.Name = userInfo.Name;
                        oldData.Mobile = userInfo.Mobile;
                        if (userInfo.DOB != null)
                        {
                            oldData.DOB = userInfo.DOB;
                        }
                        oldData.Gender = userInfo.Gender;
                        oldData.DesignationId = userInfo.DesignationId;
                        oldData.FatherName = userInfo.FatherName;
                        oldData.MotherName = userInfo.MotherName;
                        oldData.BranchId = userInfo.BranchId;
                        oldData.Email = userInfo.Email;
                        oldData.UpdatedAt = DateTime.Now;
                        oldData.UpdatedBy = User.Identity.Name;
                        oldData.RoleId = userInfo.RoleId;
                        if (Image != null)
                        {
                            oldData.Image = Utility.SaveFile(Image, path);
                        }

                        var userUpdate = _userInfoManager.Update(oldData);
                        if (userUpdate)
                        {
                            return Ok(new { success = true, SuccessMessage = "Successfully Update User." });
                        }
                        return Ok(new { success = false, ErrorMessage = "Failed to update." });
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Ok(new { success = false, ErrorMessage = "Failed to update." });
            }
        }
        public IActionResult LoadUser(int branchId)
        {
            var userList = _userInfoManager.GetUserInfoByBranchId(branchId);
            return PartialView("_UserList", userList);
        }

        public IActionResult DeActiveUser(string id)
        {
            var userInfo = _userInfoManager.GetById(id);
            if (userInfo != null)
            {
                userInfo.IsActive = false;
                var result = _userInfoManager.Update(userInfo);
                if (result)
                {
                    return Ok(new { success = true, SuccessMessage = "Successfully Deleted." });
                }
                else
                {
                    return Ok(new { success = false, ErrorMessage = "Failed to delete." });
                }

            }
            else
            {
                return Ok(new { success = false, ErrorMessage = "Data not found." });
            }
        }
    }
}
