using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.CodeAnalysis.Operations;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IAppointmentManager:ICommonManager<Appointment>
    {
        Appointment GetById(int id);
        ICollection<AppointmentEntryVm> GetByVisitorId(string visitorId);
        Appointment GetLastAppointmentData(int doctorServiceId, DateTime date);
        ICollection<AppointmentVm> GetDoctorAppointments(int branchId, string doctorId, DateTime date);
        ICollection<AppointmentListVm> GetAppointmentList(int departmentId, string doctorId, DateTime selectedDate,int branchId);

        ICollection<Appointment> GetAppointmentListForCancel(int departmentId, string doctorId, DateTime selectedDate,
            int branchId);
        ICollection<Appointment> GetTimeSlotWiseAppointments(int serviceId, DateTime date, string timeSlot);
    }
}
