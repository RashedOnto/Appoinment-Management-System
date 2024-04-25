using AppointmentManagementSystem.Common;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using AppointmentManagementSystem.SecurityExtension;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppointmentManagementSystem.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IDoctorManager _doctorManager;
        private readonly IDoctorQualificationManager _doctorQualificationManager;
        private readonly IDesignationManager _designationManager;
        private readonly IBranchManager _branchManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAppointmentManager _appointmentManager;

        private readonly IConfiguration _configuration;
     
        public DoctorController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IConfiguration configuration) : base(userManager)
        {
            _doctorManager = new DoctorManager(db);
            _doctorQualificationManager = new DoctorQualificationManager(db);
            _designationManager = new DesignationManager(db);
            _branchManager = new BranchManager(db);
            _userManager = userManager;
            _appointmentManager = new AppointmentManager(db);
            _configuration = configuration;
         
        }
        public IActionResult Index()
        {

            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.DesignationList = _designationManager.GetAll();
            //var doctorList = _doctorManager.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Doctor doctor, string Password, IFormFile Image)
        {
            try
            {
                var path = _configuration.GetValue<string>("ImagePath");
                var doctorRole = _configuration.GetValue<string>("Role:Doctor");

                if (doctor.Id == null)
                {
                    var doctorUser = new Doctor();

                    var id = doctorUser.Id;
                    var conStamp = doctor.ConcurrencyStamp;
                    var secStamp = doctor.SecurityStamp;
                    doctorUser = doctor;



                    doctorUser.Id = id;
                    doctorUser.ConcurrencyStamp = conStamp;
                    doctorUser.SecurityStamp = secStamp;

                    doctorUser.IsActive = true;
                    doctorUser.CreatedAt = DateTime.Now;
                    doctorUser.CreatedBy = User.Identity.Name;
                    doctorUser.Image = Utility.SaveFile(Image, path);
                    doctorUser.UserType = "Doctor";
                    doctorUser.UserName = doctor.Email;
                    doctorUser.RoleId = doctorRole;
                    doctorUser.EmailConfirmed = true;
                    var userAdd = _userManager.CreateAsync(doctor, Password).Result;
                    if (userAdd.Succeeded)
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully Doctor Added." });

                    }
                    return Ok(new { success = false, ErrorMessage = "Failed to add." });

                }
                else
                {
                    var oldData = _doctorManager.GetById(doctor.Id);
                    if (oldData != null)
                    {
                        oldData.Name = doctor.Name;
                        oldData.Mobile = doctor.Mobile;
                        if (doctor.DOB != null)
                        {
                            oldData.DOB = doctor.DOB;
                        }
                        oldData.Gender = doctor.Gender;
                        oldData.DesignationId = doctor.DesignationId;
                        oldData.BMDCNo = doctor.BMDCNo;
                        oldData.Specialty = doctor.Specialty;
                        oldData.Language = doctor.Language;
                        oldData.Institute = doctor.Institute;
                        oldData.Description = doctor.Description;
                        oldData.Email = doctor.Email;
                        oldData.UpdatedAt = DateTime.Now;
                        oldData.UpdatedBy = User.Identity?.Name;
                        if (Image != null)
                        {
                            oldData.Image = Utility.SaveFile(Image, path);
                        }

                        var userUpdate = _doctorManager.Update(oldData);
                        if (userUpdate)
                        {
                            return Ok(new { success = true, SuccessMessage = "Successfully Doctor Update." });
                        }
                        return Ok(new { success = false, ErrorMessage = "Failed to update." });
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
        public IActionResult DeactivateDoctor(string Id)
        {
            var doctor = _doctorManager.GetById(Id);
            if (doctor != null)
            {
                doctor.IsActive = false;
                doctor.DeletedAt = DateTime.Now;
                doctor.DeletedBy = User.Identity.Name;
                var result = _doctorManager.Update(doctor);
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

        [HttpPost]
        public IActionResult LoadDoctorList()
        {
            var doctorVmList = _doctorManager.GetAllActiveDataWithDesignationName();
            return PartialView("_DoctorList", doctorVmList);
        }

        [HttpPost]
        public IActionResult LoadQualificationDiv()
        {
            var doctorList = _doctorManager.GetAllActiveData();
            ViewBag.DoctorList = doctorList;
            return PartialView("_LoadQualificationDiv");
        }
        [HttpPost]
        public IActionResult LoadQualification(string doctorId)
        {
            var details = _doctorQualificationManager.GetDoctorDetailsByDoctorId(doctorId);
            return PartialView("_LoadQualification", details);
        }
        [HttpPost]
        public IActionResult AddQualification(DoctorQualifications qualifications)
        {
            if (qualifications.Id > 0)
            {
                var getQ = _doctorQualificationManager.GetById(qualifications.Id);
                if (getQ != null)
                {
                    ModifiedBy(getQ);
                    getQ.Achievement = qualifications.Achievement;
                    getQ.Country = qualifications.Country;
                    getQ.DegreeName = qualifications.DegreeName;
                    getQ.DoctorId = qualifications.DoctorId;
                    getQ.PassingYear = qualifications.PassingYear;
                    getQ.InstituteName = qualifications.InstituteName;
                    if (_doctorQualificationManager.Update(getQ))
                    {
                        return Ok(new { success = true, SuccessMessage = "Successfully updated." });
                    }
                    else
                    {
                        return Ok(new { success = false, SuccessMessage = "Failed to update." });
                    }
                }
                else
                {
                    return Ok(new { success = false, SuccessMessage = "Failed to update." });

                }
            }


            else
            {
                AddedBy(qualifications);
                if (_doctorQualificationManager.Add(qualifications))
                {
                    return Ok(new { success = true, SuccessMessage = "Successfully added." });

                }
                else
                {
                    return Ok(new { success = false, SuccessMessage = "Failed to add." });
                }
            }

        }

        public IActionResult DeactivateQualification(int id)
        {
            var doctor = _doctorQualificationManager.GetById(id);
            if (doctor != null)
            {
                DeletedBy(doctor);
                var result = _doctorQualificationManager.Update(doctor);
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

        public IActionResult AppointmentList()
        {
            ViewBag.Branches = _branchManager.GetAll();
            return View();
        }
        [HttpPost]
        public IActionResult LoadAppointmentList(int branchId,DateTime date)
        {
            var list = _appointmentManager.GetDoctorAppointments(branchId, User.Identity.GetUserId(), date);
            return PartialView("_LoadAppointmentList",list);
        }
    }
}
