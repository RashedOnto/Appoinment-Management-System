using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface;
using AppointmentManagementSystem.Repository;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using AppointmentManagementSystem.Interface.Manager;
using System.Collections;

namespace AppointmentManagementSystem.Manager
{
    public class BranchWiseDepartmentManager : CommonManager<BranchWiseDepartment>, IBranchWiseDepartmentManager
    {
        private readonly ApplicationDbContext _db;

        public BranchWiseDepartmentManager(ApplicationDbContext db) : base(new CommonRepository<BranchWiseDepartment>(db))
        {
            _db = db;
        }

        public ICollection<BranchWiseDepartment> GetAll()
        {
            return Get(c => true);
        }

        public BranchWiseDepartment GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public ICollection<BranchWiseDepartment> GetDepartmentByBranchId(int branchId)
        {
            return Get(c=>c.BranchId == branchId);
        }

        public ICollection<Department> GetDepartmentData(int branchId)
        {
            var departmentList = (from bd in _db.BranchWiseDepartments
                join d in _db.Departments on bd.DepartmentId equals d.Id
                where bd.BranchId == branchId
                select d).ToList();

            return departmentList;
        }
    }
}
