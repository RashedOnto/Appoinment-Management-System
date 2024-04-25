using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IDoctorServiceFee:ICommonManager<DoctorServiceFee>
    {
        ICollection<DoctorServiceFee> GetAll();
        ICollection<DoctorServiceFee> GetByDoctorServiceId(int id);
    }
}
