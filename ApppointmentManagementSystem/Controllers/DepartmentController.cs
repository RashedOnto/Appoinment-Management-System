using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentManager _departmentManager;
        public DepartmentController(ApplicationDbContext db)
        {
            _departmentManager = new DepartmentManager(db);
        }
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            return View();

        }

        [HttpPost]
        public IActionResult Add(Department department)
        {
            try
            {
                if (department.Id == 0)
                {
                    department.IsActive= true;
                    department.CreatedAt= DateTime.Now;
                    department.CreatedBy= User.Identity.Name;
                    var result = _departmentManager.Add(department);
                    if (result)
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully Department Added." });
                    }
                    else
                    {
                        return Ok(new { success = false, ErrorMessage = "Failed to add." });
                    }
                }
                else
                {
                    var oldData = _departmentManager.GetById(department.Id);
                    if (oldData != null)
                    {
                        oldData.Name = department.Name;
                        department.UpdatedAt = DateTime.Now;
                        department.UpdatedBy = User.Identity.Name;
                        var result = _departmentManager.Update(oldData);
                        if (result)
                        {
                            return Ok(new { success = true, SuccessMessage = "Successfully Department Update." });
                        }
                        else
                        {
                            return Ok(new { success = false, ErrorMessage = "Failed to update." });
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Ok(new { success = false, ErrorMessage = "Failed to update." });
            }
        }
        [HttpPost]
        public IActionResult LoadDepartment()
        {
            var departmentList = _departmentManager.GetAllActiveData();
            return PartialView("_DepartmentList", departmentList);
        }
        [HttpPost]
        public IActionResult DeActiveDepartment(int Id)
        {
            var department = _departmentManager.GetById(Id);
            if (department != null)
            {
                department.IsActive = false;
                department.DeletedAt = DateTime.Now;
                department.DeletedBy = User.Identity.Name;
                var result = _departmentManager.Update(department);
                if (result)
                {
                    return Ok(new { success = true, SuccessMessage = "Successfully Deleted." });
                }
                else
                {
                    return Ok(new { success = false, ErrorMessage = "Failed to delete." });
                }

            }
            else
            {
                return Ok(new { success = false, ErrorMessage = "Data not found." });
            }
        }
    }
}
