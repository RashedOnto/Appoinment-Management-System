using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface;
using AppointmentManagementSystem.Repository;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using AppointmentManagementSystem.Interface.Manager;

namespace AppointmentManagementSystem.Manager
{
    public class BranchManager : CommonManager<Branch>, IBranchManager
    {
        private readonly ApplicationDbContext _db;
        public BranchManager(ApplicationDbContext db) : base(new CommonRepository<Branch>(db))
        {
            _db = db;
        }

        public ICollection<Branch> GetAll()
        {
            return Get(c => true && c.IsActive);
        }

        public ICollection<Branch> GetByUserId(string userId)
        {
            var branches = (from b in _db.Branches
                            join bp in _db.BranchPermissions on b.Id equals bp.BranchId
                            where bp.UserId == userId
                            select new Branch()
                            {
                                Name = b.Name,
                                Id = b.Id,
                            }).ToList();

            return branches;
        }

        public Branch GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }
        //public ICollection<Branch> GetAllByOrganizationId(int organizationId)
        //{
        //    return Get(c => c.OrganizationId == organizationId, c => c.Organization);
        //}
    }
}
