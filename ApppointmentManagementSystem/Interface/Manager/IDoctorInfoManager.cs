using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
     interface IDoctorInfoManager:ICommonManager<DoctorQualifications>
    {
        DoctorQualifications GetDoctorDetailsByDoctorId(string doctorId);
    }
}
