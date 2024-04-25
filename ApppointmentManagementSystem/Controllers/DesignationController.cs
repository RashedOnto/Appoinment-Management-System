using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Controllers
{
    public class DesignationController : BaseController
    {
        private readonly IDesignationManager _designationManager;
        public DesignationController(ApplicationDbContext db,UserManager<IdentityUser> userManager):base(userManager)
        {
            _designationManager = new DesignationManager(db);
        }
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            return View();

        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Designation designation)
        {
            try
            {
                if (designation.Id == 0)
                {
                    AddedBy(designation);
                    var result = _designationManager.Add(designation);
                    if (result)
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully Designation Added." });
                    }
                    else
                    {
                        return Ok(new { success = false, ErrorMessage = "Failed to add." });
                    }
                }
                else
                {
                    var oldData = _designationManager.GetById(designation.Id);
                    if (oldData != null)
                    {
                      ModifiedBy(oldData);
                      oldData.Name=designation.Name;
                        var result = _designationManager.Update(oldData);
                        if (result)
                        {
                            return Ok(new { success = true, SuccessMessage = "Successfully Designation Update." });
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
        public IActionResult LoadDesignation()
        {
            var designationList = _designationManager.GetAll();
            return PartialView("_DesignationList", designationList);
        }
        [HttpPost]
        public IActionResult DeActiveDesignation(int Id)
        {
            var designation = _designationManager.GetById(Id);
            if (designation != null)
            {
                DeletedBy(designation);
                var result = _designationManager.Update(designation);
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
