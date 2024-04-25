using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Repository;
using AppointmentManagementSystem.ViewModels;

namespace AppointmentManagementSystem.Manager
{
    public class UserInfoManager : CommonManager<UserInfo>, IUserInfoManager
    {
        private ApplicationDbContext _db;
        public UserInfoManager(ApplicationDbContext db) : base(new CommonRepository<UserInfo>(db))
        {
            _db = db;
        }


        public ICollection<UserInfoVm> GetUserInfo()
        {
            throw new NotImplementedException();
        }

        public ICollection<UserInfoVm> GetUserInfoByBranchId(int branchId)
        {
            var data = (from u in _db.UserInfo
                join r in _db.Role on u.RoleId equals r.Id
                join d in _db.Designations on u.DesignationId equals d.Id
                where u.IsActive && u.BranchId==branchId && u.UserType=="UserInfo"
                select new UserInfoVm
                {
                    RoleId = r.Id,
                    Email = u.Email,
                    DOB = u.DOB,
                    Designation = d.Name,
                    DesignationId = d.Id,
                    Gender = u.Gender,
                    Id = u.Id,
                    Image = u.Image,
                    Mobile = u.Mobile,
                    Name = u.Name,
                    Role = r.Name,
                    BranchId = branchId,
                    MotherName = u.MotherName,
                    FatherName = u.FatherName

                }).ToList();
            return data;
        }

        public UserInfo GetById(string id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }
    }
}
