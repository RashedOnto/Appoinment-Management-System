using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;
using AppointmentManagementSystem.ViewModels;
using Microsoft.CodeAnalysis.Operations;

namespace AppointmentManagementSystem.Manager
{
    public class AppointmentManager : CommonManager<Appointment>, IAppointmentManager
    {
        private readonly ApplicationDbContext _db;
        public AppointmentManager(ApplicationDbContext db) : base(new CommonRepository<Appointment>(db))
        {
            _db = db;
        }

        public Appointment GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public ICollection<AppointmentEntryVm> GetByVisitorId(string visitorId)
        {
            var appointmentEntryVmList = (from app in _db.Appointments
                join ds in _db.DoctorServices on app.DoctorServiceId equals ds.Id
                join doc in _db.Doctors on ds.DoctorId equals doc.Id
                join b in _db.Branches on ds.BranchId equals b.Id
                join at in _db.AppointmentTypes on app.AppointmentTypeId equals at.Id
                join dip in _db.Departments on ds.DepartmentId equals dip.Id 

                where app.IsActive
                select new AppointmentEntryVm()
                {
                    Id = app.Id,
                    PatientName = app.PatientName,
                    PatientGender = app.PatientGender,
                    PatientAge = app.PatientAge,
                    SLNO = app.SLNo,
                    AppointmentTypeName = at.Name,
                    AppointmentStatus = app.AppointmentStatus,
                    BranchId = b.Id,
                    BranchName = b.Name,
                    DepartmentId = dip.Id,
                    DepartmentName = dip.Name,
                    DoctorId = doc.Id,
                    DoctorName = doc.Name,
                    Date = app.Date,
                    TimeSlot = app.TimeSlot,
                    PaymentAmount = app.PaymentAmount,
                    DiscountAmount = app.DiscountAmount,
                    EntryTime = app.EntryTime,
                    ExitDate = app.ExitDate,
                }).ToList();

            return appointmentEntryVmList;
        }

        public ICollection<AppointmentVm> GetDoctorAppointments(int branchId, string doctorId, DateTime date)
        {
            var data = (from a in _db.Appointments
                        join s in _db.DoctorServices on a.DoctorServiceId equals s.Id
                        join at in _db.AppointmentTypes on a.AppointmentTypeId equals at.Id
                        join v in _db.Visitors on a.VisitorId equals v.Id
                        where s.BranchId == branchId && s.DoctorId == doctorId && a.Date.Date == date.Date
                        select new AppointmentVm()
                        {
                            Date = a.Date,
                            AppointmentStatus = a.AppointmentStatus,
                            AppointmentType = at.Name,
                            AppointmentTypeId = a.AppointmentTypeId,
                            DiscountAmount = a.DiscountAmount,
                            DoctorServiceId = a.DoctorServiceId,
                            EntryTime = a.EntryTime,
                            ExitDate = a.ExitDate,
                            Id = a.Id,
                            IsSelfAppointment = a.IsSelfAppointment,
                            PaymentAmount = a.PaymentAmount,
                            PaymentStatus = a.PaymentStatus,
                            SLNo = a.SLNo,
                            ServiceTime = s.ServiceTime,
                            TimeSlot = a.TimeSlot,
                            VisitorId = v.Id,
                            VisitorName = v.Name,
                            Gender = v.Gender,
                            VisitorAddress = v.Address,
                            VisitorDob = v.DOB.Value.ToString("dd-MMM-yyyy")
                        }).ToList();
            return data;

            //return Get(c => c.IsActive && c.doc == doctorServiceId && c.Date.Date == date.Date);
        }
       
        public ICollection<AppointmentListVm> GetAppointmentList(int departmentId, string doctorId, DateTime selectedDate, int branchId)
        {
            var query = (from appointment in _db.Appointments
                        join doctorService in _db.DoctorServices on appointment.DoctorServiceId equals doctorService.Id
                        join branch in _db.Branches on doctorService.BranchId equals branch.Id
                        join doctor in _db.Doctors on doctorService.DoctorId equals doctor.Id
                        join appointmentType in _db.AppointmentTypes on appointment.AppointmentTypeId equals appointmentType.Id
                        where doctorService.DepartmentId == departmentId &&
                              doctorService.DoctorId == doctorId &&
                              appointment.Date.Date == selectedDate.Date &&
                              branch.Id == branchId
                        select new AppointmentListVm()
                        {
                            Id=appointment.Id,
                           PatientName = appointment.PatientName,
                           Age = appointment.PatientAge,
                           Gender = appointment.PatientGender,
                           DoctorName = doctor.Name,
                           AppointmentTypeName=appointmentType.Name,
                           Date=appointment.Date.Date,
                           SLNo = appointment.SLNo,
                           TimeSlot = appointment.TimeSlot,
                           AppointmentStatus = appointment.AppointmentStatus,
                           PaymentAmount = appointment.PaymentAmount,
                           PaymentStatus = appointment.PaymentStatus,
                           DiscountAmount = appointment.DiscountAmount,
                           EntryTime = appointment.EntryTime,
                           ExitDate = appointment.ExitDate.Date,
                           IsSelfAppointment = appointment.IsSelfAppointment,
                           DoctorId=doctorService.DoctorId,
                           DepartmentId = doctorService.DepartmentId,
                           WeekDayId = doctorService.WeekDaysId,
                           Today = DateTime.Now.Date
                        }).ToList();

            return query;
        }
        public ICollection<Appointment> GetAppointmentListForCancel(int departmentId, string doctorId, DateTime selectedDate, int branchId)
        {
            var query = from appointment in _db.Appointments
                join doctorService in _db.DoctorServices on appointment.DoctorServiceId equals doctorService.Id
                join branch in _db.Branches on doctorService.BranchId equals branch.Id
                join doctor in _db.Doctors on doctorService.DoctorId equals doctor.Id
                //join visitor in _db.Visitors on appointment.VisitorId equals visitor.Id
                join appointmentType in _db.AppointmentTypes on appointment.AppointmentTypeId equals appointmentType.Id
                where doctorService.DepartmentId == departmentId &&
                      doctorService.DoctorId == doctorId &&
                      appointment.Date.Date == selectedDate.Date &&
                      branch.Id == branchId
                select appointment;
            return query.ToList();
        }


        public Appointment GetLastAppointmentData(int doctorServiceId, DateTime date)
        {
            return Get(c => c.DoctorServiceId == doctorServiceId && c.Date.Date == date.Date)
                .OrderByDescending(c => int.TryParse(c.SLNo, out int slNo) ? slNo : int.MinValue)
                .FirstOrDefault();
        }

        public ICollection<Appointment> GetTimeSlotWiseAppointments(int serviceId, DateTime date, string timeSlot)
        {
            return Get(c =>
                c.DoctorServiceId == serviceId && c.Date.Date == date.Date &&
                c.TimeSlot == timeSlot && c.AppointmentStatus == "Pending" && c.AppointmentStatus != "Cancel");
        }
    }
}
