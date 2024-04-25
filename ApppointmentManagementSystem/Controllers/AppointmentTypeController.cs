using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Controllers
{
    public class AppointmentTypeController : BaseController
    {
        private readonly IAppointmentTypeManager _appointmentTypeManager;
        public AppointmentTypeController(ApplicationDbContext Context,UserManager<IdentityUser> userManager):base(userManager)
        {
            _appointmentTypeManager = new AppointmentTypeManager(Context);
        }
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            return View();
        }
        public IActionResult GetAllType()
        {

            var typeList = _appointmentTypeManager.GetAll().ToList();
            return Json(typeList);
        }
        [HttpPost]
        public IActionResult Add(AppointmentType appointmentType)
        {
            string errorMessage = "";
            try
            {
                AddedBy(appointmentType);
                var result = _appointmentTypeManager.Add(appointmentType);
                errorMessage = "Appointment Type Add Successfully.";
                return Json(new { ErrorMessage = errorMessage }); ;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(appointmentType);
        }
       
        [HttpPost]
        public IActionResult Update(AppointmentType appointmentType)
        {
            string errorMessage = "";
            try
            {
                var getData = _appointmentTypeManager.GetById(appointmentType.Id);
                if (getData != null)
                {
                    getData.Name = appointmentType.Name;
                   
                    ModifiedBy(getData);
                    if (_appointmentTypeManager.Update(getData))
                    {
                        errorMessage = "Appointment Type Updated Successfully.";
                    }
                    else
                    {
                        errorMessage = "AppointmentType Update Failed.";
                    }
                }
                else
                {
                    errorMessage = "Data not found.";
                }
                return Json(new { ErrorMessage = errorMessage }); ;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(appointmentType);

        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string errorMessage = "";
            try
            {

                var type = _appointmentTypeManager.GetById(id);
                if (type == null)
                {
                    errorMessage = "Data not found.";
                    return Json(new { ErrorMessage = errorMessage });
                }
                else
                {
                    type.IsActive = false;
                    DeletedBy(type);
                    _appointmentTypeManager.Update(type);
                    errorMessage = "Appointment Type deleted successfully.";
                    return Json(new { ErrorMessage = errorMessage }); ;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Json(new { ErrorMessage = errorMessage });
        }
    }
}
