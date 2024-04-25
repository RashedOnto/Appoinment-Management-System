using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppointmentManagementSystem.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleManager _roleManager;
        private readonly ISubMenuManager _subMenuManager;
        private readonly IMainMenuManager _mainMenuManager;
        private readonly IPermissionManager _permissionManager;

        public RoleController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _roleManager = new RoleManager(db, userManager);
            _subMenuManager = new SubMenuManager(db);
            _mainMenuManager=new MainMenuManager(db);
            _permissionManager = new PermissionManager(db);
        }

        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            var userId = GetUserId();
            var myRole = _roleManager.GetRoleByUserId(userId);
            ViewBag.MinOrderNo = myRole?.OrderNo;
            return View();
        }

        [HttpPost]
        public IActionResult Add(Role role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.Id))
                {
                    var obj=new Role() { Name = role.Name,OrderNo = role.OrderNo};

                    var result = _roleManager.Add(obj);
                    if (result)
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully Added Role" });
                    }
                    else
                    {
                        return Ok(new { success = false, ErrorMessage = "Failed to add." });
                    }
                }
                else
                {
                    var oldData = _roleManager.GetRoleById(role.Id);
                    if (oldData != null)
                    {
                        oldData.Name = role.Name;
                        oldData.OrderNo = role.OrderNo;
                        var result = _roleManager.Update(oldData);
                        if (result)
                        {
                            return Ok(new { success = true, SuccessMessage = "Successfully Updated Role." });
                        }
                        else
                        {
                            return Ok(new { success = false, ErrorMessage = "Failed to update." });
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Ok(new { success = false, ErrorMessage = "Failed to update." });
            }
        }
        [HttpPost]
        public IActionResult LoadRole()
        {
            var departmentList = _roleManager.GetRoles(1);
            return PartialView("_RoleList", departmentList);
        }
        [HttpPost]
        public IActionResult DeActiveRole(string Id)
        {
            var role = _roleManager.GetRoleById(Id);
            if (role != null)
            {

                var result = _roleManager.Delete(role);
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

        public IActionResult Permission()
        {
            ViewBag.RoleList = _roleManager.GetRoles(1);
            return View();
        }
        [HttpPost]
        public IActionResult LoadSubMenu(string roleId)
        {
            List<MainMenuVm> mainMenuList = new List<MainMenuVm>();
            var allSubmenu = _subMenuManager.GetAll();
            var mainMenuData = _mainMenuManager.GetAll();
            var mainMenu = allSubmenu.GroupBy(x => x.MenuId);
            var checkSubmenu = _permissionManager.GetPermissionByRoleId(roleId).Select(c => c.SubMenuId);
            foreach (var main in mainMenu)
            {
                MainMenuVm mainModel = new MainMenuVm();
                mainModel.MainMenuId = main.Key;
                mainModel.MainMenuName = mainMenuData.FirstOrDefault(x => x.Id == main.Key).Name;
                List<RoleSubMenuVm> subList = new List<RoleSubMenuVm>();
                foreach (var sub in main.ToList())
                {
                    RoleSubMenuVm model = new RoleSubMenuVm();
                    model.SubmenuId = sub.Id;
                    model.SubmenuName = sub.Name;
                    if (checkSubmenu.Contains(sub.Id))
                    {
                        model.IsChecked = true;
                    }
                    else
                    {
                        model.IsChecked = false;
                    }
                    subList.Add(model);
                }
                mainModel.SubmenuList = subList;
                mainMenuList.Add(mainModel);
            }
            return PartialView("_SubMenuList", mainMenuList);
        }
        public bool InsertRoleMapping(string submenuId, string roleId)
        {
            List<RolePermission> list = new List<RolePermission>();
            dynamic submenuIdList = JsonConvert.DeserializeObject(submenuId);
            try
            {
                foreach (var Id in submenuIdList)
                {
                    RolePermission model = new RolePermission();
                    model.RoleId = roleId;
                    model.SubMenuId = Convert.ToInt32(Id.ToString());
                    list.Add(model);
                }
                var oldData = _permissionManager.GetPermissionByRoleId(roleId);
                if (oldData.Any())
                {
                    bool delete = _permissionManager.Delete(oldData);
                }
                if (_permissionManager.Add(list))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
