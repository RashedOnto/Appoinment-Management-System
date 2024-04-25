using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class PermissionManager:CommonManager<RolePermission>, IPermissionManager
    {
        public PermissionManager(ApplicationDbContext db) : base(new CommonRepository<RolePermission>(db))
        {

        }

        public ICollection<RolePermission> GetAll()
        {
            return Get(c => true);
        }

        public ICollection<RolePermission> GetPermissionByRoleId(string roleId)
        {
            return Get(c=>c.RoleId == roleId);  
        }
    }
}
