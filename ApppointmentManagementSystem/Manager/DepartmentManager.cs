using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class DepartmentManager : CommonManager<Department>, IDepartmentManager
    {
        public DepartmentManager(ApplicationDbContext db) : base(new CommonRepository<Department>(db)) 
        { }

        public ICollection<Department> GetAllActiveData()
        {
            return Get(c=>c.IsActive);
        }

        public Department GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }
    }
}
