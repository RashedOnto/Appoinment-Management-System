using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IDoctorManager:ICommonManager<Doctor>
    {
        Doctor GetById(string id);
        ICollection<Doctor> GetAll();
        ICollection<Doctor> GetAllActiveData();
        List<DoctorVm> GetAllActiveDataWithDesignationName();
        DoctorDetailsVm GetByIdWithDoctorDetails(string doctorId);
    }
}
