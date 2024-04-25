using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.SecurityExtension;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Components
{
    public class Sidebar : ViewComponent
    {

        private readonly IRoleManager _roleManager;

        private readonly ApplicationDbContext _db;
        public Sidebar(ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {
            // roleSubMenuManager = new RoleSubMenuManager(db);
            _db = db;
            _roleManager = new RoleManager(db,userManager);
        }

        public IViewComponentResult Invoke()
        {
            var res = GetMenuByRole();
            return View(res);
        }
        public List<DynamicMenuVm> GetMenuByRole()
        {
            List<DynamicMenuVm> dMList = new List<DynamicMenuVm>();
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var roleId = _roleManager.GetRoleByUserId(userId)?.Id;



                 dMList = (from sr in _db.RolePermissions
                          join sub in _db.SubMenus on sr.SubMenuId equals sub.Id
                          join m in _db.Menus on sub.MenuId equals m.Id
                          where sr.RoleId == roleId
                          orderby m.OrderNo
                          select new SubmenuVm
                          {
                              ActionName = sub.ActionName,
                              ActiveSubMenuId = sub.ActiveSubMenuId,
                              ControllerName = sub.ControllerName,
                              Id = sub.Id,
                              MenuId = sub.MenuId,
                              MenuName = m.Name,
                              Name = sub.Name,
                              ActiveMenuId = m.ActiveMenuId,
                              Order = m.OrderNo,
                              //Icon = m.Icon

                          }).GroupBy(p => new { p.MenuId, p.MenuName, p.Order, p.ActiveMenuId })
                .Select(group => new DynamicMenuVm()
                {
                    MainMenuName = group.Key.MenuName,
                    ActiveMenuId = group.Key.ActiveMenuId,
                    Order = group.Key.Order,
                    SubMenuLists = group.ToList()
                }).ToList();

            }

            return dMList.OrderBy(c => c.Order).ToList();
        }
    }
}
