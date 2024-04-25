using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class DesignationManager:CommonManager<Designation>,IDesignationManager
    {
        public DesignationManager(ApplicationDbContext db):base(new CommonRepository<Designation>(db)) { }

        public Designation GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public ICollection<Designation> GetActive()
        {
            return Get(c => c.IsActive);
        }
    }
}
