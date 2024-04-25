using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using System.ComponentModel;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IDoctorServicesManager : ICommonManager<DoctorServices>
    {
        DoctorServices GetById(int id);
        ICollection<DoctorDetailsVm> GetDoctorDetails(int branchId, int departmentId);
        DoctorServices GetDoctorServiceByDoctorIdAndWeekDaysId(int weekDaysId, string doctorId, int branchId, int departmentId);
        List<WeekDaysVm> GetDoctorServiceDayByDoctorId(string doctorId, int branchId, int departmentId);
        ICollection<ServiceInfoVm> GetDoctorServiceInfo(int? departmentId, string? doctorId, int? weekdayId);
        ICollection<DoctorServices> GetListByDepartmentId(int departmentId);
        ICollection<DoctorServices> GetDoctorService(string doctorId, int branchId, int departmentId);

        ICollection<DoctorServices> GetDoctorServiceByDoctorIdAndWeekDaysIdAndDepartmentId(int weekDaysId,
            string doctorId, int branchId,
            int departmentId);

        DoctorServices GetServiceByTimeWise(string time, int weekDayId, string doctorId, int departmentId,
            int branchId);
    }
}
