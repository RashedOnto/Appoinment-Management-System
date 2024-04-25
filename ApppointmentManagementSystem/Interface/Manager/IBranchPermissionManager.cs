using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IBranchPermissionManager:ICommonManager<BranchPermission>
    {
        ICollection<BranchPermission> GetBranchPermissionsByUserId(string userId);
      
    }
}
