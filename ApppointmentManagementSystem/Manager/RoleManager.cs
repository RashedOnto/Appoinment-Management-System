using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;
using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Manager
{
    public class RoleManager : CommonManager<Role>, IRoleManager
    {
        private readonly UserManager<IdentityUser> _userManager;
        public RoleManager(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(new CommonRepository<Role>(db))

        {
            _userManager = userManager;
        }

        public ICollection<Role> GetRoles(int order)
        {
            var roles = Get(c => c.OrderNo >= order);
            return roles;

        }

        public Role GetRoleById(string id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public Role GetRoleByUserId(string userId)
        {
            var identityUser = _userManager.Users.FirstOrDefault(c => c.Id == userId);
            IdentityModel model = identityUser as IdentityModel;
            if (model != null)
            {
                var roleId = model.RoleId;
                var role = GetRoleById(roleId);
                return role;

            }
            return null;
        }
    }
}
