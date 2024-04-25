using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers
{
    public class DoctorServicesController : BaseController
    {
        private readonly IDoctorServicesManager _doctorServicesManager;
        private readonly IDoctorServiceFee _doctorServiceFee;
        private readonly IDepartmentManager _departmentManager;
        private readonly IDoctorManager _doctorManager;
        private readonly IWeekDaysManager _weekDaysManager;
        private readonly IAppointmentTypeManager _appointmentTypeManager;
        private readonly IBranchWiseDepartmentManager _branchWiseDepartmentManager;
        private readonly IDesignationManager _designationManager;
        private readonly UserManager<IdentityUser> _userManager;
        //private ApplicationDbContext _db;
        public DoctorServicesController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _doctorServicesManager = new DoctorServicesManager(db);
            _doctorServiceFee = new DoctorServiceFeeManager(db);
            _departmentManager = new DepartmentManager(db);
            _doctorManager = new DoctorManager(db);
            _weekDaysManager = new WeekdaysManager(db);
            _appointmentTypeManager = new AppointmentTypeManager(db);
            _branchWiseDepartmentManager = new BranchWiseDepartmentManager(db);
            _userManager = userManager;
            //_db = db;

        }
        public IActionResult GetAppointmentType()
        {
            var appointmentType = _appointmentTypeManager.GetAll();
            return Json(appointmentType);
        }

        public IActionResult GetAppointmentTypeForUpdate(int id)
        {
            var getData = _doctorServiceFee.GetByDoctorServiceId(id);
            var appointmentType = _appointmentTypeManager.GetAll();
            var result = (from data in getData
                join type in appointmentType on data.AppointmentTypeId equals type.Id
                select new
                {
                    name = type.Name,
                    id=type.Id,
                    fee = data.Fee
                });
            return Json(result);
        }
       
        public IActionResult GetDepartment()
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
            return Json(departmentvmList);
        }
        public IActionResult GetWeekDays()
        {
            var days = _weekDaysManager.GetAll();
            return Json(days);
        }
        public IActionResult GetDoctor()
        {
            var doctor = _doctorManager.GetAllActiveData();
            return Json(doctor);
        }
        [HttpPost]
        public IActionResult Add(int departmentId, string startTime, int noOfPatient, string doctorId, string endTime, int noOfExtraPatient, bool isTimeSlot, int weekdayId, string typeId, string visitFee)
        {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            DateTime sTime = DateTime.ParseExact(startTime, "HH:mm", null);
            string fTime = sTime.ToString("h:mm tt");
            DateTime eTime = DateTime.ParseExact(endTime, "HH:mm", null);
            string tTime = eTime.ToString("h:mm tt");
            DoctorServices doctorServices = new DoctorServices();
            string errorMessage = "";
            try
            {
                doctorServices.BranchId = branchId;
                doctorServices.DepartmentId = departmentId;
                doctorServices.ServiceTime = fTime + "-" + tTime;
                doctorServices.TotalPatient = noOfPatient;
                doctorServices.ExtraPatient = noOfExtraPatient;
                doctorServices.DoctorId = doctorId;
                doctorServices.IsTimeSlotWise = isTimeSlot;
                doctorServices.WeekDaysId = weekdayId;
                AddedBy(doctorServices);
                var result = _doctorServicesManager.Add(doctorServices);
                if (result)
                {
                    List<int?> AppointmentTypeIdList = JsonConvert.DeserializeObject<List<int?>>(typeId);
                    List<double?> feeList = JsonConvert.DeserializeObject<List<double?>>(visitFee);
                    List<DoctorServiceFee> serviceFeeList = new List<DoctorServiceFee>();
                    for (int i = 0; i < AppointmentTypeIdList.Count && i < feeList.Count; i++)
                    {
                        int? appointmentTypeId = AppointmentTypeIdList[i];
                        double? fee = feeList[i];
                        if (appointmentTypeId.HasValue && fee.HasValue)
                        {
                            DoctorServiceFee doctorServiceFee = new DoctorServiceFee
                            {
                                DoctorServicesId = doctorServices.Id,
                                AppointmentTypeId = appointmentTypeId.Value,
                                Fee = fee.Value
                            };
                            AddedBy(doctorServiceFee);
                            serviceFeeList.Add(doctorServiceFee);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    _doctorServiceFee.Add(serviceFeeList);
                }
                errorMessage = "Doctor Assign Successfully.";
                return Json(new { ErrorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(doctorServices);
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
            ViewBag.DoctorList = _doctorManager.GetAllActiveData();
            ViewBag.DayList = _weekDaysManager.GetAllWeekDays();
            return View();
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string errorMessage = "";
            try
            {
                var doctorService = _doctorServicesManager.GetById(id);
                if (doctorService == null)
                {
                    errorMessage = "Data not found.";
                    return Json(new { ErrorMessage = errorMessage });
                }
                else
                {
                    doctorService.IsActive = false;
                    DeletedBy(doctorService);
                    _doctorServicesManager.Update(doctorService);
                    errorMessage = "Doctor Service deleted successfully.";
                    return Json(new { ErrorMessage = errorMessage }); ;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Json(new { ErrorMessage = errorMessage });
        }
        public IActionResult GetDoctorByDepartmentId(int departmentId)
        {
            var ServiceList = _doctorServicesManager.GetListByDepartmentId(departmentId);           
            var doctorList = _doctorManager.GetAllActiveData();          
            var DepartmentDoctorVm = (from service in ServiceList                                 
                                     join doctor in doctorList on service.DoctorId equals doctor.Id  
                                     select new DepartmentDoctorVm
                                     {
                                        Id=doctor.Id,
                                        Name=doctor.Name
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
        public IActionResult GetList(int? departmentId,string? doctorId, int? weekdayId)
        {
            var serviceInfoVmList = _doctorServicesManager.GetDoctorServiceInfo(departmentId,doctorId,weekdayId);

            return Json(serviceInfoVmList.ToList());
        }
        
        [HttpPost]
       public IActionResult UpdateDoctorAssignInfo(int id, int departmentId, string startTime, int noOfPatient, string doctorId, string endTime, int noOfExtraPatient, bool isTimeSlot, int weekdayId, string typeId, string visitFee)
       {
            int branchId = 0;
            var userId = GetUserId();
            var user = _userManager.FindByIdAsync(userId).Result;
            UserInfo model = user as UserInfo;
            if (model != null)
            {
                branchId = model.BranchId;
            }
            DateTime sTime = DateTime.ParseExact(startTime, "HH:mm", null);
            string fTime = sTime.ToString("h:mm tt");
            DateTime eTime = DateTime.ParseExact(endTime, "HH:mm", null);
            string tTime = eTime.ToString("h:mm tt");
            var doctorServices = _doctorServicesManager.GetById(id);
            string errorMessage = "";
            try
            {
                doctorServices.BranchId = branchId;
                doctorServices.DepartmentId = departmentId;
                doctorServices.ServiceTime = fTime + "-" + tTime;
                doctorServices.TotalPatient = noOfPatient;
                doctorServices.ExtraPatient = noOfExtraPatient;
                doctorServices.DoctorId = doctorId;
                doctorServices.IsTimeSlotWise = isTimeSlot;
                doctorServices.WeekDaysId = weekdayId;
                ModifiedBy(doctorServices);
                var result = _doctorServicesManager.Update(doctorServices);
                if (result)
                {
                    List<int?> AppointmentTypeIdList = JsonConvert.DeserializeObject<List<int?>>(typeId);
                    List<double?> feeList = JsonConvert.DeserializeObject<List<double?>>(visitFee);
                    List<DoctorServiceFee> serviceFeeList = new List<DoctorServiceFee>();
                    for (int i = 0; i < AppointmentTypeIdList.Count && i < feeList.Count; i++)
                    {
                        int? appointmentTypeId = AppointmentTypeIdList[i];
                        double? fee = feeList[i];
                        if (appointmentTypeId.HasValue && fee.HasValue)
                        {
                            var getData = _doctorServiceFee.GetByDoctorServiceId(id).Where(c=>c.AppointmentTypeId==appointmentTypeId);
                            foreach (var item in getData)
                            {
                                item.AppointmentTypeId=appointmentTypeId.Value;
                                item.Fee=fee.Value;
                                ModifiedBy(item);
                                serviceFeeList.Add(item);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    _doctorServiceFee.Update(serviceFeeList);
                }
                errorMessage = "Doctor Assign Update Successfully.";
                return Json(new { ErrorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(doctorServices);
        }
    }
}
