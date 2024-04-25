using System.Diagnostics;
using System.Globalization;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AppointmentManagementSystem.Controllers
{
    public class HomeController : BaseController
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly BranchManager _branchManager;
        private readonly BranchWiseDepartmentManager _branchWiseDepartmentManager;
        private readonly DoctorManager _doctorManager;
        private readonly DoctorServicesManager _doctorServicesManager;
        private readonly VisitorManager _visitorManager;
        private readonly AppointmentTypeManager _appointmentTypeManager;
        private readonly AppointmentManager _appointmentManager;
       

        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IConfiguration configuration) : base(userManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _branchManager = new BranchManager(db);
            _branchWiseDepartmentManager = new BranchWiseDepartmentManager(db);
            _doctorManager = new DoctorManager(db);
            _doctorServicesManager = new DoctorServicesManager(db);
            _visitorManager = new VisitorManager(db);
            _appointmentTypeManager = new AppointmentTypeManager(db);
            _appointmentManager = new AppointmentManager(db);
           

        }
        //Application Main page
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            var userId = GetUserId();
            if (userId != null)
            {
                var getUser = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (getUser != null)
                {
                    IdentityModel model = getUser as IdentityModel;
                    if (model != null)
                    {
                        string doctorRoleId = _configuration["Role:Doctor"];
                        string visitorRoleId = _configuration["Role:Visitor"];
                        if (!string.IsNullOrEmpty(model.RoleId) && model.RoleId == visitorRoleId)
                        {
                            ViewBag.BranchList = _branchManager.GetAll();
                            ViewBag.UserId = userId;
                            return View();
                        }
                        else if (model.RoleId == doctorRoleId)
                        {
                            return RedirectToAction("DoctorDashboard");
                        }
                        else
                        {
                            return RedirectToAction("Dashboard");
                        }
                    }
                }
            }

            ViewBag.BranchList = _branchManager.GetAll();
            return View();
        }

        public IActionResult GetAppointment(string doctorId, int branchId, int departmentId, int id)
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];

            var userId = GetUserId();
            if (userId != null)
            {
                var getUser = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (getUser != null)
                {
                    IdentityModel model = getUser as IdentityModel;
                    if (model != null)
                    {
                        string visitorRoleId = _configuration["Role:Visitor"];
                        if (!string.IsNullOrEmpty(model.RoleId) && model.RoleId == visitorRoleId)
                        {
                            var visitor = _visitorManager.GetById(getUser.Id);
                            AppointmentEntryVm appointmentEntryVm = new AppointmentEntryVm();
                            appointmentEntryVm.VisitorAddress = visitor.Address;
                            appointmentEntryVm.VisitorMobile = visitor.Mobile;
                            appointmentEntryVm.VisitorId = visitor.Id;



                            ViewBag.ServiceDate = _doctorServicesManager.GetDoctorServiceDayByDoctorId(doctorId, branchId, departmentId);
                            ViewBag.AppointmentType = _appointmentTypeManager.GetAll();
                            ViewBag.DoctorId = doctorId;
                            ViewBag.BranchId = branchId;
                            ViewBag.DepartmentId = departmentId;

                            //Reschedule Appointment
                            if (id != 0)
                            {
                                var getBookPooAppointment = _appointmentManager.GetById(id);
                                ViewBag.PreviousDate = getBookPooAppointment.Date;
                                ViewBag.PreviousSlot = getBookPooAppointment.TimeSlot;
                                appointmentEntryVm.PatientName = getBookPooAppointment.PatientName;
                                appointmentEntryVm.PatientGender = getBookPooAppointment.PatientGender;
                                appointmentEntryVm.PatientAge = getBookPooAppointment.PatientAge;
                                appointmentEntryVm.AppointmentStatus = getBookPooAppointment.AppointmentStatus;
                            }
                            return View(appointmentEntryVm);
                        }
                        else
                        {

                            return Redirect("/Account/VisitorLogin?ReturnUrl=" + Uri.EscapeDataString("/Home/GetAppointment%3FdoctorId%3D" + doctorId + "%26branchId%3D" + branchId + "%26departmentId%3D" + departmentId));
                        }
                    }
                }
            }
            else
            {
                return Redirect("/Account/VisitorLogin?ReturnUrl=" + Uri.EscapeDataString("/Home/GetAppointment%3FdoctorId%3D" + doctorId + "%26branchId%3D" + branchId + "%26departmentId%3D" + departmentId));
            }
            return View();
        }

        [HttpPost]
        public IActionResult GetAppointmentEntry(Appointment appointment, string dateString)
        {
            if (appointment != null)
            {
                if (appointment.Id == 0)
                {
                    appointment.IsSelfAppointment = true;
                    DateTime dateTimeValue = DateTime.ParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    var lastAppointment =
                        _appointmentManager.GetLastAppointmentData(appointment.DoctorServiceId, dateTimeValue);
                    appointment.IsSelfAppointment = true;

                    if (lastAppointment != null)
                    {
                        var sLNo = Int32.Parse(lastAppointment.SLNo) + 1;
                        appointment.SLNo = sLNo.ToString();
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
                        TempData["Success"] = "Thanks for booking! Arrive on time for your confirmed appointment. See you soon!";
                    }
                    else
                    {
                        TempData["Error"] = "Operation failed";
                    }
                }
                else
                {
                    var oldData = _appointmentManager.GetById(appointment.Id);
                    DateTime dateTimeValue = DateTime.ParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    if (oldData.Date.Date != dateTimeValue.Date)
                    {
                        var lastAppointment =
                            _appointmentManager.GetLastAppointmentData(appointment.DoctorServiceId, dateTimeValue);

                        if (lastAppointment != null)
                        {
                            var sLNo = Int32.Parse(lastAppointment.SLNo) + 1;
                            appointment.SLNo = sLNo.ToString();
                        }
                        else
                        {
                            appointment.SLNo = "1001";
                        }
                    }
                    
                    oldData.Date = dateTimeValue;
                    oldData.TimeSlot = appointment.TimeSlot;
                    ModifiedBy(oldData);
                    var result = _appointmentManager.Update(oldData);
                    if (result)
                    {
                        TempData["Success"] = "Reschedule success! See you at the updated time. Thanks!";
                    }
                    else
                    {
                        TempData["Error"] = "Operation failed";
                    }
                }
            }
            return RedirectToAction("SelfAppointmentList");
        }



        public IActionResult LoadAppointmentTime(int weekDaysId, string doctorId, int branchId, int departmentId, string dateString)
        {
            //var doctorServices = _doctorServicesManager.GetDoctorService(doctorId, branchId, departmentId);



            //var service = _doctorServicesManager.GetDoctorServiceByDoctorIdAndWeekDaysId(weekDaysId, doctorId, branchId, departmentId);

            //DateTime dateTimeValue = DateTime.ParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            //object result = null;

            //List<string> minuteIntervals = new List<string>();

            //if (service.IsTimeSlotWise)
            //{
            //    DateTime startTime = DateTime.ParseExact(service.ServiceTime.Split('-')[0].Trim(), "h:mm tt", null);
            //    DateTime endTime = DateTime.ParseExact(service.ServiceTime.Split('-')[1].Trim(), "h:mm tt", null);



            //    int totalMinutes = (int)(endTime - startTime).TotalMinutes;
            //    int totalPatientsPerMinute = totalMinutes / (service.TotalPatient + service.ExtraPatient);

            //    var totalPatient = 0;
            //    for (int i = 0; i < totalMinutes; i += totalPatientsPerMinute)
            //    {
            //        if ((service.TotalPatient + service.ExtraPatient) == totalPatient)
            //        {
            //            break;
            //        }

            //        DateTime currentStart = startTime.AddMinutes(i);
            //        DateTime currentEnd = startTime.AddMinutes(i + totalPatientsPerMinute);

            //        string serviceTimeInterval = $"{currentStart.ToString("hh:mm tt")}-{currentEnd.ToString("hh:mm tt")}";

            //        var alreadyBookTimeSlot = _appointmentManager.GetTimeSlotWiseAppointments(service.Id, dateTimeValue, serviceTimeInterval);
            //        if (alreadyBookTimeSlot.Count().Equals(0))
            //        {
            //            minuteIntervals.Add(serviceTimeInterval);
            //        }
            //    }


            //    result = new { ServiceId = service.Id, MinuteIntervals = minuteIntervals };
            //}
            //else
            //{
            //    result = new { ServiceId = service.Id, IsTimeSlotWise = false };
            //}

            //return Json(result);
            var serviceList = _doctorServicesManager.GetDoctorServiceByDoctorIdAndWeekDaysIdAndDepartmentId(weekDaysId, doctorId, branchId, departmentId);

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
                    DateTime dateTimeValue = DateTime.ParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
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

        [HttpGet]
        public IActionResult GetIndividualTimeSlot(string date, string time, int weekDayId, string doctorId, int departmentId,int branchId)
        {
          
            object result = null;
            DateTime dateTimeValue = DateTime.ParseExact(date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            var service = _doctorServicesManager.GetServiceByTimeWise(time, weekDayId, doctorId, departmentId, branchId);
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
        public IActionResult SelfAppointmentList()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            var userId = GetUserId();
            if (userId != null)
            {
                var getUser = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (getUser != null)
                {
                    IdentityModel model = getUser as IdentityModel;
                    if (model != null)
                    {
                        string visitorRoleId = _configuration["Role:Visitor"];
                        if (!string.IsNullOrEmpty(model.RoleId) && model.RoleId == visitorRoleId)
                        {
                            var selfAppointmentList = _appointmentManager.GetByVisitorId(userId);
                            return View(selfAppointmentList);
                        }
                    }
                }
            }
            return Redirect("/Account/VisitorLogin");
        }

        public IActionResult CancelAppointment(int id)
        {

            var getData = _appointmentManager.GetById(id);
            if (getData != null)
            {
                getData.AppointmentStatus = "Cancel";
                var result = _appointmentManager.Update(getData);
                if (result)
                {
                    TempData["Success"] = "Patient Cancel Successfully.";
                    return RedirectToAction("SelfAppointmentList");
                }
                else
                {
                    TempData["Error"] = "Patient Cancel Failed.";
                    return RedirectToAction("SelfAppointmentList");
                }

            }
            else
            {
                TempData["Error"] = "Patient Not Found.";
                return RedirectToAction("SelfAppointmentList");
            }
        }


        [HttpGet]
        //[MiddlewareFilter(typeof(MyCustomAuthenticationMiddlewarePipeline))]
        public IActionResult DoctorProfile(string doctorId, int branchId, int departmentId)
        {
            var doctorDetailsVm = _doctorManager.GetByIdWithDoctorDetails(doctorId);
            doctorDetailsVm.BranchId = branchId;
            doctorDetailsVm.DepartmnetId = departmentId;
            return View(doctorDetailsVm);
        }

        [HttpPost]
        public IActionResult LoadDepartmentByBranchId(int branchId)
        {
            var branchWiseDepartment = _branchWiseDepartmentManager.GetDepartmentData(branchId);
            return Json(branchWiseDepartment);
        }

        [HttpPost]
        public IActionResult GetDoctorDetailsByBranchAndDepId(int branchId, int departmentId)
        {
            var doctorDetailsList = _doctorServicesManager.GetDoctorDetails(branchId, departmentId);
            return Json(doctorDetailsList);
        }

        //[MiddlewareFilter(typeof(MyCustomAuthenticationMiddlewarePipeline))]
        public IActionResult Dashboard()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                return View();
            }
            return Redirect("/Account/Login");
        }
        [Authorize]
        public IActionResult DoctorDashboard()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var getUser = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (getUser != null)
                {
                    IdentityModel model = getUser as IdentityModel;
                    if (model != null)
                    {
                        string doctorRoleId = _configuration["Role:Doctor"];
                        if (string.IsNullOrEmpty(model.RoleId) || model.RoleId == doctorRoleId)
                        {
                            return View();
                        }
                        else
                        {
                            return Redirect("/Account/Login");
                        }
                    }
                }
            }
            return Redirect("/Account/Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}