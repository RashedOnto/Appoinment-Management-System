using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
     interface IDoctorQualificationManager:ICommonManager<DoctorQualifications>
    {
        DoctorQualifications GetById(int id);
        ICollection<DoctorQualifications> GetDoctorDetailsByDoctorId(string  doctorId);
    }
}
