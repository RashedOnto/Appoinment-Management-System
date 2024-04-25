using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IUserInfoManager : ICommonManager<UserInfo>
    {
       // ICollection<UserInfoVm> GetUserInfo();
        ICollection<UserInfoVm> GetUserInfoByBranchId(int branchId);
        UserInfo GetById(string id);
    }
}
