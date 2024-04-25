using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface;
using AppointmentManagementSystem.Repository;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using AppointmentManagementSystem.Interface.Manager;


namespace AppointmentManagementSystem.Manager
{
    public class BranchPermissionManager : CommonManager<BranchPermission>,IBranchPermissionManager
    {
        public BranchPermissionManager(ApplicationDbContext db) : base(new CommonRepository<BranchPermission>(db))
        {
        }


        public ICollection<BranchPermission> GetBranchPermissionsByUserId(string userId)
        {
            return Get(c => c.UserId == userId);
        }
    }
}
