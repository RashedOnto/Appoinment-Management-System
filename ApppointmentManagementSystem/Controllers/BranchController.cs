using System.Security.Cryptography.X509Certificates;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;

namespace AppointmentManagementSystem.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchManager _branchManager;
        private readonly IUserInfoManager _userManager;
        private readonly IBranchPermissionManager _branchPermissionManager;
        public BranchController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _branchManager = new BranchManager(db);
            _userManager = new UserInfoManager(db);
            _branchPermissionManager= new BranchPermissionManager(db);

        }
        public IActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.ErrorMessage = TempData["Error"];
            return View();
        }
        public IActionResult GetAllBranch()
        {

            var branchList = _branchManager.GetAll();
            return Json(branchList);
        }
        // [HttpPost]
        //public IActionResult LoadBranchListByOrganizationId(int organizationId)
        //{
        //    var data = _branchManager.GetAllByOrganizationId(organizationId);
        //    return PartialView("_ListByOrganization", data);
        //}
        [HttpGet]
        public IActionResult Add()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Add(Branch branch)
        {
            string errorMessage = "";
            try
            {
                AddedBy(branch);
                var result = _branchManager.Add(branch);
                errorMessage = "Branch Add Successfully.";
                return Json(new { ErrorMessage = errorMessage }); ;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(branch);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _branchManager.GetById(id);

            return View(branch);
        }
        [HttpPost]
        public IActionResult Update(Branch branch)
        {
            string errorMessage = "";
            try
            {
                var getData = _branchManager.GetById(branch.Id);
                if (getData != null)
                {
                    getData.Name = branch.Name;
                    getData.Address = branch.Address;
                    getData.Hotline = branch.Hotline;
                    ModifiedBy(getData);
                    if (_branchManager.Update(getData))
                    {
                        errorMessage = "Branch Updated Successfully.";
                    }
                    else
                    {
                        errorMessage = "Branch Update Failed.";
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
            return Json(branch);

        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string errorMessage = "";
            try
            {

                var branch = _branchManager.GetById(id);
                if (branch == null)
                {
                    errorMessage = "Data not found.";
                    return Json(new { ErrorMessage = errorMessage });
                }
                else
                {
                    branch.IsActive = false;
                    DeletedBy(branch);
                    _branchManager.Update(branch);
                    errorMessage = "Branch deleted successfully.";
                    return Json(new { ErrorMessage = errorMessage }); ;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Json(new { ErrorMessage = errorMessage });
        }

        public IActionResult UserMapping()
        {
            ViewBag.BranchList = _branchManager.GetAll();
            //_branchManager.GetByUserId(GetUserId());
            return View();
        }
        [HttpPost]
        public IActionResult LoadUser(int branchId)
        {
            var user = _userManager.GetUserInfoByBranchId(branchId);
            return Json(user);
        }
        [HttpPost]
        public IActionResult LoadBranch(string userId)
        {
            var branches = _branchManager.GetAll();//_branchManager.GetByUserId(GetUserId());
            var currentBranch = _branchManager.GetByUserId(userId);
            List<BranchPermissionVm> branchList = new List<BranchPermissionVm>();
         
            foreach (var b in branches)
            {
                BranchPermissionVm branch = new BranchPermissionVm();
                branch.BranchId = b.Id;
                branch.BranchName = b.Name;
                branch.IsChecked = currentBranch.Any(d => d.Id == b.Id);
                branchList.Add(branch);
            }
            return PartialView("_BranchList", branchList);
        }
        [HttpPost]
        public IActionResult InsertUserMapping(string userId, string branchIds)
        {
            try
            {
                List<int> branchIdList = JsonConvert.DeserializeObject<List<int>>(branchIds);
                var getData = _branchPermissionManager.GetBranchPermissionsByUserId(userId);
                
                if (getData.Any())
                {
                    _branchPermissionManager.Delete(getData);
                }
                List<BranchPermission> data = new List<BranchPermission>();
                foreach (var item in branchIdList)
                {
                    BranchPermission obj = new BranchPermission();
                    obj.BranchId = item;
                    obj.UserId = userId;
                    data.Add(obj);
                }

                if (_branchPermissionManager.Add(data))
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
           
        }
    }
}
