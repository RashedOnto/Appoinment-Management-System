using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class WeekdaysManager:CommonManager<WeekDays>,IWeekDaysManager
    {
        public WeekdaysManager(ApplicationDbContext db):base(new CommonRepository<WeekDays>(db)) { }

       public ICollection<WeekDays> GetAllWeekDays()
        {
            return Get(c => true);
        }
    }
}
