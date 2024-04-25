using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.SecurityExtension;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppointmentManagementSystem.Controllers
{
    public class BranchWiseDepartmentController : Controller
    {
        private readonly IBranchWiseDepartmentManager _branchWiseDepartmentManager;
        private readonly IBranchManager _branchManager;
        private readonly IDepartmentManager _departmentManager;
        public BranchWiseDepartmentController(ApplicationDbContext context)
        {
            _branchWiseDepartmentManager = new BranchWiseDepartmentManager(context);
            _branchManager = new BranchManager(context);
            _departmentManager= new DepartmentManager(context);
        }
        [HttpGet]
        public IActionResult DepartmentMapping()
        {
            ViewBag.BranchList = _branchManager.GetByUserId(User.Identity.GetUserId());
            return View();
        }

        public IActionResult InsertBranchMapping(string departmentId, int branchId)
        {
            try
            {
                List<int> departmentIdList = JsonConvert.DeserializeObject<List<int>>(departmentId);               
                var getdata = _branchWiseDepartmentManager.GetDepartmentByBranchId(branchId);
                if (getdata.Any())
                {
                    _branchWiseDepartmentManager.Delete(getdata);     
                }
                List<BranchWiseDepartment> data = new List<BranchWiseDepartment>();
                foreach (var item in departmentIdList)
                {
                    BranchWiseDepartment obj=new BranchWiseDepartment();
                    obj.BranchId= branchId;
                    obj.DepartmentId = item;
                    data.Add(obj);
                }
                _branchWiseDepartmentManager.Add(data);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        public IActionResult DepartmentList(int branchId)
        {
            List<DepartmentVm> departmentList = new List<DepartmentVm>();
            var departmentData = _departmentManager.GetAllActiveData();
            var checkDepartment = _branchWiseDepartmentManager.GetDepartmentByBranchId(branchId);
            foreach (var b in departmentData)
            {
                DepartmentVm departmentVm = new DepartmentVm();
                departmentVm.DepartmentId = b.Id;
                departmentVm.DepartmentName = b.Name;
                departmentVm.IsChecked  = checkDepartment.Any(d => d.DepartmentId == b.Id);
                departmentList.Add(departmentVm);
            }
            return PartialView("_DepartmentList", departmentList);
        }
    }
}
