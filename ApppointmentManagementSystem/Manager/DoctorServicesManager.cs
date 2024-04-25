using AppointmentManagementSystem.Common;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;
using AppointmentManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Globalization;

namespace AppointmentManagementSystem.Manager
{
    public class DoctorServicesManager : CommonManager<DoctorServices>, IDoctorServicesManager
    {
        private readonly ApplicationDbContext _db;

        public DoctorServicesManager(ApplicationDbContext db) : base(new CommonRepository<DoctorServices>(db))
        {
            _db = db;
        }

        public DoctorServices GetById(int id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public ICollection<DoctorDetailsVm> GetDoctorDetails(int branchId, int departmentId)
        {

            var doctorList = _db.DoctorServices
                .Where(ds => ds.IsActive && ds.BranchId == branchId && ds.DepartmentId == departmentId)
                .Join(_db.Doctors, ds => ds.DoctorId, d => d.Id, (ds, d) => new { ds, d })
                .Join(_db.DoctorQualifications, dd => dd.d.Id, dq => dq.DoctorId, (dd, dq) => new { dd.d, dq })
                .GroupBy(joined => joined.d.Id)
                .Select(group => new DoctorDetailsVm
                {
                    DoctorId = group.Key,
                    DoctorDegreeName = string.Join(", ",
                        group.Select(doctor => $"{doctor.dq.DegreeName} ({doctor.dq.Country})").Distinct()),
                    DoctorImage = Utility.PathToBase64(group.First().d.Image),
                    DoctorInstitute = group.First().d.Institute,
                    DoctorLanguage = group.First().d.Language,
                    DoctorName = group.First().d.Name,
                    DoctorSpecialty = group.First().d.Specialty,
                    DoctorDescription = group.First().d.Description
                })
                .ToList();



            return doctorList;
        }

        public DoctorServices GetDoctorServiceByDoctorIdAndWeekDaysId(int weekDaysId, string doctorId, int branchId,
            int departmentId)
        {
            return GetFirstOrDefault(c =>
                c.DoctorId == doctorId && c.BranchId == branchId && c.DepartmentId == departmentId &&
                c.WeekDaysId == weekDaysId);
        }
        public ICollection<DoctorServices> GetDoctorServiceByDoctorIdAndWeekDaysIdAndDepartmentId(int weekDaysId, string doctorId, int branchId,
            int departmentId)
        {
            return Get(c =>
                c.DoctorId == doctorId && c.BranchId == branchId && c.DepartmentId == departmentId &&
                c.WeekDaysId == weekDaysId);
        }
        public List<WeekDaysVm> GetDoctorServiceDayByDoctorId(string doctorId, int branchId, int departmentId)
        {
            //var doctorServiceList = _db.DoctorServices
            //    .Where(ds => ds.DoctorId == doctorId && ds.BranchId == branchId && ds.DepartmentId == departmentId)
            //    .ToList();

            //Dictionary<string, int> dayNameToDayOfWeek = new Dictionary<string, int>
            //{
            //    {"Sunday", 0}, {"Monday", 1}, {"Tuesday", 2}, {"Wednesday", 3}, {"Thursday", 4}, {"Friday", 5}, {"Saturday", 6}
            //};

            //var todayDayOfWeek = (int)DateTime.Today.DayOfWeek;

            //var weekDaysVmList = doctorServiceList
            //    .Join(_db.WeekDays,
            //        ds => ds.WeekDaysId,
            //        wd => wd.Id,
            //        (ds, wd) => new WeekDaysVm
            //        {
            //            Id = wd.Id,
            //            Date = DateTime.Today.AddDays((7 + (dayNameToDayOfWeek[wd.Name] - todayDayOfWeek)) % 7).ToString("dd-MMM-yyyy"),
            //            DoctorServiceId = ds.Id
            //        })
            //    .Distinct()
            //    .Where(day =>
            //    {
            //        DateTime currentDate = DateTime.ParseExact(day.Date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            //        var totalBookingPatients = _db.Appointments.Count(a => a.Date == currentDate);
            //        var service = doctorServiceList.FirstOrDefault(c => c.Id == day.DoctorServiceId);
            //        var totalDoctorPatients = service.TotalPatient + service.ExtraPatient;
            //        return totalBookingPatients != totalDoctorPatients;
            //    })
            //    .ToList();

            //return weekDaysVmList;
            var doctorServiceList = _db.DoctorServices
                .Where(ds => ds.DoctorId == doctorId && ds.BranchId == branchId && ds.DepartmentId == departmentId)
                .ToList();

            Dictionary<string, int> dayNameToDayOfWeek = new Dictionary<string, int>
            {
                {"Sunday", 0}, {"Monday", 1}, {"Tuesday", 2}, {"Wednesday", 3}, {"Thursday", 4}, {"Friday", 5}, {"Saturday", 6}
            };

            var todayDayOfWeek = (int)DateTime.Today.DayOfWeek;

            var weekDaysVmList = doctorServiceList
                .Join(_db.WeekDays,
                    ds => ds.WeekDaysId,
                    wd => wd.Id,
                    (ds, wd) => new WeekDaysVm
                    {
                        Id = wd.Id,
                        Date = DateTime.Today.AddDays((7 + (dayNameToDayOfWeek[wd.Name] - todayDayOfWeek)) % 7).ToString("dd-MMM-yyyy"),
                        DoctorServiceId = ds.Id
                    })
                .Distinct()
                .GroupBy(day => day.Date)
                .Select(group => group.OrderBy(day => day.DoctorServiceId).First()) // Select the first item for each distinct date
                .Where(day =>
                {
                    DateTime currentDate = DateTime.ParseExact(day.Date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    var totalBookingPatients = _db.Appointments.Count(a => a.Date == currentDate);
                    var service = doctorServiceList.FirstOrDefault(c => c.Id == day.DoctorServiceId);
                    var totalDoctorPatients = service.TotalPatient + service.ExtraPatient;
                    return totalBookingPatients != totalDoctorPatients;
                })
                .ToList();

            return weekDaysVmList;

        }

        public ICollection<ServiceInfoVm> GetDoctorServiceInfo(int? departmentId, string doctorId, int? weekdayId)
        {
            var result = (from service in _db.DoctorServices
                join department in _db.Departments on service.DepartmentId equals department.Id
                join doctor in _db.Doctors on service.DoctorId equals doctor.Id
                join weekday in _db.WeekDays on service.WeekDaysId equals weekday.Id
                join fee in _db.DoctorServiceFees on service.Id equals fee.DoctorServicesId
                join ftn in _db.AppointmentTypes on fee.AppointmentTypeId equals ftn.Id
                where (departmentId == null || service.DepartmentId == departmentId) &&
                      (doctorId == null || doctor.Id == doctorId) && (weekdayId == null || weekday.Id == weekdayId)
                group new { service, department, doctor, weekday, fee, ftn } by new { service.Id } into grouped
                select new ServiceInfoVm
                {
                    DoctorServiceId = grouped.Key.Id,
                    DepartmentName = grouped.Select(x => x.department.Name).FirstOrDefault(),
                    DoctorName = grouped.Select(x => x.doctor.Name).FirstOrDefault(),
                    Days = grouped.Select(x => x.weekday.Name).FirstOrDefault(),
                    Time = grouped.Select(x => x.service.ServiceTime).FirstOrDefault(),
                    TotalPatient = grouped.Select(x => x.service.TotalPatient).FirstOrDefault(),
                    ExtraPatient = grouped.Select(x => x.service.ExtraPatient).FirstOrDefault(),
                    IsTimeSlotWise = grouped.Select(x => x.service.IsTimeSlotWise).FirstOrDefault(),
                    DoctorId = grouped.Select(x => x.doctor.Id).FirstOrDefault(),
                    DepartmentId = grouped.Select(x => x.department.Id).FirstOrDefault(),
                    DayId = grouped.Select(x => x.weekday.Id).FirstOrDefault(),
                    VisitFee = string.Join(",", grouped.Select(x => $"{x.ftn.Name}-{x.fee.Fee}"))
                }).ToList();

            return result;
        }

        public ICollection<DoctorServices> GetListByDepartmentId(int departmentId)
        {
            return Get(c => c.DepartmentId == departmentId);
        }

        public ICollection<DoctorServices> GetDoctorService(string doctorId, int branchId, int departmentId)
        {
            return Get(c => c.BranchId == branchId && c.DepartmentId == departmentId && c.DoctorId == doctorId);
        }

        public DoctorServices GetServiceByTimeWise(string time,int weekDayId,string doctorId,int departmentId,int branchId)
        {
            return GetFirstOrDefault(c =>
                c.DoctorId == doctorId && c.BranchId == branchId && c.DepartmentId == departmentId &&
                c.WeekDaysId == weekDayId && c.ServiceTime==time);

        }
    }
}
