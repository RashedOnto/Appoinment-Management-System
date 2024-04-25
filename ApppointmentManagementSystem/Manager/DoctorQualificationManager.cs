using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class DoctorQualificationManager : CommonManager<DoctorQualifications>, IDoctorQualificationManager
    {
        public DoctorQualificationManager(ApplicationDbContext db):base(new CommonRepository<DoctorQualifications>(db))
        {

        }

        public DoctorQualifications GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public ICollection<DoctorQualifications> GetDoctorDetailsByDoctorId(string  doctorId)

        {
            return Get(c => c.DoctorId == doctorId && c.IsActive);
        }
    }
}
