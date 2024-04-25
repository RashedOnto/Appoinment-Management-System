using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NuGet.Protocol;
using System.Globalization;
using static AppointmentManagementSystem.Controllers.DoctorServicesController;

namespace AppointmentManagementSystem.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IDoctorManager _doctorManager;
        private readonly IAppointmentTypeManager _appointmentTypeManager;
        private readonly IBranchWiseDepartmentManager _branchWiseDepartmentManager;
        private readonly IDesignationManager _designationManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppointmentManager _appointmentManager;
        private readonly IDoctorServicesManager _doctorServicesManager;
        private readonly IDepartmentManager _departmentManager;
        public AppointmentController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _doctorManager = new DoctorManager(db);
            _appointmentTypeManager = new AppointmentTypeManager(db);
            _branchWiseDepartmentManager = new BranchWiseDepartmentManager(db);
            _userManager = userManager;
            _appointmentManager = new AppointmentManager(db);
            _doctorServicesManager = new DoctorServicesManager(db);
            _departmentManager= new DepartmentManager(db);
        }
        public IActionResult Index()
        {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            var departmentIdList = _branchWiseDepartmentManager.GetDepartmentByBranchId(branchId);
            var departmentList = _departmentManager.GetAllActiveData();
            var departmentvmList = departmentIdList
                .Join(
                    departmentList,
                    bwd => bwd.DepartmentId,
                    dep => dep.Id,
                    (bwd, dep) => new
                    {
                        DepartmentId = dep.Id,
                        dep.Name
                    })
                .ToList();
            ViewBag.DepartmentList = departmentvmList;
            //ViewBag.DepartmentList = _departmentManager.GetAllActiveData();
            ViewBag.AppointmentType = _appointmentTypeManager.GetAll();

            return View();
        }
        public IActionResult GetDoctorByDepartmentId(int departmentId)
        {
            var ServiceList = _doctorServicesManager.GetListByDepartmentId(departmentId);
            var doctorList = _doctorManager.GetAllActiveData();
            var DepartmentDoctorVm = (from service in ServiceList
                                      join doctor in doctorList on service.DoctorId equals doctor.Id
                                      select new DepartmentDoctorVm
                                      {
                                          Id = doctor.Id,
                                          Name = doctor.Name
                                      }).Distinct(new DepartmentDoctorComparer()).ToList();

            return Json(DepartmentDoctorVm);
        }
        public class DepartmentDoctorComparer : IEqualityComparer<DepartmentDoctorVm>
        {
            public bool Equals(DepartmentDoctorVm x, DepartmentDoctorVm y)
            {
                return x.Id == y.Id && x.Name == y.Name;
            }

            public int GetHashCode(DepartmentDoctorVm obj)
            {
                return HashCode.Combine(obj.Id, obj.Name);
            }
        }

        public IActionResult GetAppointmentList(int departmentId, string doctorId, DateTime selectedDate)
        {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            var appointmentList = _appointmentManager.GetAppointmentList(departmentId, doctorId, selectedDate, branchId);

            return Json(appointmentList.ToList());

        }
        [HttpPost]
        public IActionResult UpdateAppointmentStatus(int id)
        {
            string errorMessage = "";
            var getData = _appointmentManager.GetById(id);
            if (getData != null)
            {
                getData.AppointmentStatus = "Entry";
                getData.EntryTime = DateTime.Now;
                _appointmentManager.Update(getData);
                errorMessage = "Patient Entry Successfully.";
                return Json(new { ErrorMessage = errorMessage });
            }
            else
            {
                errorMessage = "Patient Id Not Found.";
                return Json(new { ErrorMessage = errorMessage });
            }
        }
        [HttpPost]
        public IActionResult DeletePatient(int id)
        {
            string errorMessage = "";
            var getData = _appointmentManager.GetById(id);
            if (getData != null)
            {
                getData.AppointmentStatus = "Cancel";
                _appointmentManager.Update(getData);
                errorMessage = "Schedule Cancel Successfully.";
                return Json(new { ErrorMessage = errorMessage });
            }
            else
            {
                errorMessage = "Patient Id Not Found.";
                return Json(new { ErrorMessage = errorMessage });
            }
        }

        [HttpPost]
        public IActionResult CancelAllAppointment(int departmentId, string doctorId, DateTime selectedDate)
        {
            string errorMessage = "";
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            List<Appointment> list = new List<Appointment>();
            var appointmentList = _appointmentManager.GetAppointmentListForCancel(departmentId, doctorId, selectedDate, branchId);
            foreach (var item in appointmentList)
            {
                if (item.AppointmentStatus == "Pending")
                {
                    item.AppointmentStatus = "Cancel";
                    list.Add(item);
                }
                else
                {
                    continue;
                }

            }
            bool isUpdate = _appointmentManager.Update(list);
            if (isUpdate)
            {
                errorMessage = "Schedule Cancel Successfully.";
                return Json(new { ErrorMessage = errorMessage });
            }
            else
            {
                errorMessage = "Schedule Doesn't Cancel.";
                return Json(new { ErrorMessage = errorMessage });
            }
        }

        public IActionResult UpdateAppointmentSchedule(int id, DateTime formattedDate, string? newTimeSlot)
        {
            string errorMessage = "";
            var getData = _appointmentManager.GetById(id);
            if (getData != null)
            {
                var lastAppointment =
                    _appointmentManager.GetLastAppointmentData(id, formattedDate);
                if (lastAppointment != null)
                {
                    var slNo = Int32.Parse(lastAppointment.SLNo) + 1;
                    getData.SLNo = slNo.ToString();
                }
                else
                {
                    getData.SLNo = "1001";
                }
                if (formattedDate == null && newTimeSlot == null)
                {
                    getData.TimeSlot = getData.TimeSlot;
                    getData.Date = getData.Date;
                }
                //if (formattedDate == null && newTimeSlot != null)
                //{
                //    getData.TimeSlot = newTimeSlot;
                //    getData.Date = date;
                //}

                if (formattedDate != null && newTimeSlot != null)
                {
                    getData.TimeSlot = newTimeSlot;
                    getData.Date = formattedDate;
                    // getData.Date = formattedDate ?? DateTime.Now;
                }
                if (formattedDate != null && newTimeSlot == null)
                {
                    getData.TimeSlot = null;
                    getData.Date = formattedDate;
                }

                bool isUpdate = _appointmentManager.Update(getData);
                if (isUpdate)
                {
                    errorMessage = "Schedule Change Successfully.";
                    return Json(new { ErrorMessage = errorMessage });
                }
                else
                {
                    errorMessage = "Schedule Doesn't Change.";
                    return Json(new { ErrorMessage = errorMessage });
                }
            }
            else
            {
                errorMessage = "Schedule data not found.";
                return Json(new { ErrorMessage = errorMessage });
            }
        }

        public IActionResult GetDoctorServiceDate(string doctorId, int departmentId)
        {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            var availableSlot = _doctorServicesManager.GetDoctorServiceDayByDoctorId(doctorId, branchId, departmentId);
            return Json(availableSlot);
        }
        //public IActionResult LoadAppointmentTime(int weekDayId, string doctorId, int departmentId, string date)
        //{
        //    DateTime dateObject = DateTime.Parse(date);
        //    string formattedDate = dateObject.ToString("yyyy-MM-dd");
        //    int branchId = 0;
        //    var userId = GetUserId();
        //    var user = _userManager.FindByIdAsync(userId).Result;
        //    UserInfo model = user as UserInfo;
        //    if (model != null)
        //    {
        //        branchId = model.BranchId;
        //    }
        //    var service = _doctorServicesManager.GetDoctorServiceByDoctorIdAndWeekDaysId(weekDayId, doctorId, branchId, departmentId);


        //    DateTime dateTimeValue = DateTime.ParseExact(formattedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    object result = null;

        //    if (service != null)
        //    {
        //        DateTime startTime = DateTime.ParseExact(service.ServiceTime.Split('-')[0].Trim(), "h:mm tt", null);
        //        DateTime endTime = DateTime.ParseExact(service.ServiceTime.Split('-')[1].Trim(), "h:mm tt", null);

        //        int totalMinutes = (int)(endTime - startTime).TotalMinutes;
        //        int totalPatientsPerMinute = totalMinutes / (service.TotalPatient + service.ExtraPatient);

        //        List<string> minuteIntervals = new List<string>();

        //        var totalPatient = 0;
        //        for (int i = 0; i < totalMinutes; i += totalPatientsPerMinute)
        //        {
        //            if ((service.TotalPatient + service.ExtraPatient) == totalPatient)
        //            {
        //                break;
        //            }
        //            DateTime currentStart = startTime.AddMinutes(i);
        //            DateTime currentEnd = startTime.AddMinutes(i + totalPatientsPerMinute);

        //            string serviceTimeInterval = $"{currentStart.ToString("hh:mm tt")}-{currentEnd.ToString("hh:mm tt")}";

        //            var alreadyBookTimeSlot = _appointmentManager.GetTimeSlotWiseAppointments(service.Id, dateTimeValue, serviceTimeInterval);
        //            if (alreadyBookTimeSlot.Count().Equals(0))
        //            {
        //                minuteIntervals.Add(serviceTimeInterval);
        //            }

        //            totalPatient++;
        //        }


        //        result = new { ServiceId = service.Id, MinuteIntervals = minuteIntervals };
        //    }

        //    return Json(result);

        //}
        public IActionResult LoadAppointmentTime(int weekDayId, string doctorId, int departmentId, string date)
        {
            DateTime dateObject = DateTime.Parse(date);
            string formattedDate = dateObject.ToString("dd-MMM-yyyy");
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            var serviceList = _doctorServicesManager.GetDoctorServiceByDoctorIdAndWeekDaysIdAndDepartmentId(weekDayId, doctorId, branchId, departmentId);
           
            if (serviceList.Count > 1)
            {
                List<string> serviceTimes = new List<string>();
                foreach (var item in serviceList)
                {
                    serviceTimes.Add(item.ServiceTime);
                }
                return Json(serviceTimes);
            }
            else
            {
                object result = null;
                foreach (var service in serviceList)
                {
                    DateTime dateTimeValue = DateTime.ParseExact(formattedDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    List<string> minuteIntervals = new List<string>();
                    if (service.IsTimeSlotWise)
                    {
                        DateTime startTime = DateTime.ParseExact(service.ServiceTime.Split('-')[0].Trim(), "h:mm tt", null);
                        DateTime endTime = DateTime.ParseExact(service.ServiceTime.Split('-')[1].Trim(), "h:mm tt", null);
                        int totalMinutes = (int)(endTime - startTime).TotalMinutes;
                        int totalPatientsPerMinute = totalMinutes / (service.TotalPatient + service.ExtraPatient);

                        var totalPatient = 0;
                        for (int i = 0; i < totalMinutes; i += totalPatientsPerMinute)
                        {
                            if ((service.TotalPatient + service.ExtraPatient) == totalPatient)
                            {
                                break;
                            }
                            DateTime currentStart = startTime.AddMinutes(i);
                            DateTime currentEnd = startTime.AddMinutes(i + totalPatientsPerMinute);
                            string serviceTimeInterval = $"{currentStart.ToString("hh:mm tt")}-{currentEnd.ToString("hh:mm tt")}";
                            var alreadyBookTimeSlot = _appointmentManager.GetTimeSlotWiseAppointments(service.Id, dateTimeValue, serviceTimeInterval);
                            if (alreadyBookTimeSlot.Count().Equals(0))
                            {
                                minuteIntervals.Add(serviceTimeInterval);
                            }
                        }
                        result = new { ServiceId = service.Id, MinuteIntervals = minuteIntervals };
                    }
                    else
                    {
                        result = new { ServiceId = service.Id, IsTimeSlotWise = false };
                    }
                }
                return Json(result);
            }
        }
        [HttpPost]
        public IActionResult AddAppointmentByUser(Appointment appointment, string dateString)
        {
            string errorMessage = "";
            DateTime dateTimeValue = DateTime.ParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            appointment.IsSelfAppointment = false;
            var lastAppointment =
                _appointmentManager.GetLastAppointmentData(appointment.DoctorServiceId, dateTimeValue);

            if (lastAppointment != null)
            {
                var slNo = Int32.Parse(lastAppointment.SLNo) + 1;
                appointment.SLNo = slNo.ToString();
            }
            else
            {
                appointment.SLNo = "1001";
            }

            appointment.Date = dateTimeValue;

            appointment.AppointmentStatus = "Pending";
            AddedBy(appointment);
            var result = _appointmentManager.Add(appointment);
            if (result)
            {
                errorMessage = "Appointment successful.";
                return Json(new { ErrorMessage = errorMessage });
            }
            else
            {
                errorMessage = "Appointment does not add .";
                return Json(new { ErrorMessage = errorMessage });
            }
        }

        [HttpGet]
        public IActionResult GetIndividualTimeSlot(string date,string time, int weekDayId, string doctorId, int departmentId)
        {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            object result = null;
            DateTime dateTimeValue = DateTime.ParseExact(date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            var service = _doctorServicesManager.GetServiceByTimeWise(time, weekDayId, doctorId,departmentId,branchId);
            List<string> minuteIntervals = new List<string>();
            if (service.IsTimeSlotWise)
            {
                DateTime startTime = DateTime.ParseExact(service.ServiceTime.Split('-')[0].Trim(), "h:mm tt", null);
                DateTime endTime = DateTime.ParseExact(service.ServiceTime.Split('-')[1].Trim(), "h:mm tt", null);
                int totalMinutes = (int)(endTime - startTime).TotalMinutes;
                int totalPatientsPerMinute = totalMinutes / (service.TotalPatient + service.ExtraPatient);

                var totalPatient = 0;
                for (int i = 0; i < totalMinutes; i += totalPatientsPerMinute)
                {
                    if ((service.TotalPatient + service.ExtraPatient) == totalPatient)
                    {
                        break;
                    }
                    DateTime currentStart = startTime.AddMinutes(i);
                    DateTime currentEnd = startTime.AddMinutes(i + totalPatientsPerMinute);
                    string serviceTimeInterval = $"{currentStart.ToString("hh:mm tt")}-{currentEnd.ToString("hh:mm tt")}";
                    var alreadyBookTimeSlot = _appointmentManager.GetTimeSlotWiseAppointments(service.Id, dateTimeValue, serviceTimeInterval);
                    if (alreadyBookTimeSlot.Count().Equals(0))
                    {
                        minuteIntervals.Add(serviceTimeInterval);
                    }
                }
                result = new { ServiceId = service.Id, MinuteIntervals = minuteIntervals };
            }
            else
            {
                result = new { ServiceId = service.Id, IsTimeSlotWise = false };
            }
            return Json(result);
        }
    }
}
