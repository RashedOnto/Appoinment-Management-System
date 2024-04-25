using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Interface.Repository;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class DoctorServiceFeeManager:CommonManager<DoctorServiceFee>,IDoctorServiceFee
    {
        public DoctorServiceFeeManager(ApplicationDbContext db):base(new CommonRepository<DoctorServiceFee>(db)) { }
        public ICollection<DoctorServiceFee> GetAll()
        {
            return Get(c => true);
        }

        public ICollection<DoctorServiceFee> GetByDoctorServiceId(int id)
        {
            return Get(c => c.DoctorServicesId == id && c.IsActive);
        }
    }
}
