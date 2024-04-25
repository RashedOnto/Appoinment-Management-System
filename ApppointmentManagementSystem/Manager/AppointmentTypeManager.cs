using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class AppointmentTypeManager : CommonManager<AppointmentType>, IAppointmentTypeManager
    {
        public AppointmentTypeManager(ApplicationDbContext db):base(new CommonRepository<AppointmentType>(db))
        {

        }
        public ICollection<AppointmentType> GetAll()
        {
            return Get(c => true && c.IsActive);
        }

        public AppointmentType GetById(int id)
        {
           return GetFirstOrDefault(c => c.Id == id);
        }
    }
}
