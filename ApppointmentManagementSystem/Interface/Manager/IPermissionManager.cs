using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IPermissionManager:ICommonManager<RolePermission>
    {
       ICollection<RolePermission> GetPermissionByRoleId(string roleId);
       ICollection<RolePermission> GetAll();
    }
}
