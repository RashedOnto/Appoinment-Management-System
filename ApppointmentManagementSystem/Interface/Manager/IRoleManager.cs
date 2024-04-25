using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IRoleManager : ICommonManager<Role>
    {
        ICollection<Role> GetRoles(int order);
        Role GetRoleById(string  id);
        Role GetRoleByUserId(string userId);
    }
}
