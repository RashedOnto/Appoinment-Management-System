using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public BaseController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public string GetUserId()
        {
            var userId = new HttpContextAccessor().HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(c => c.Id == userId);
            return user?.Id;
        }



        public void AddedBy(BaseClass baseClass)
        {
            baseClass.CreatedBy = GetUserId();
            baseClass.CreatedAt = DateTime.Now;
            baseClass.IsActive = true;
        }
        public void ModifiedBy(BaseClass baseClass)
        {
            baseClass.UpdatedBy = GetUserId();
            baseClass.UpdatedAt = DateTime.Now;
        }
        public void DeletedBy(BaseClass baseClass)
        {
            baseClass.DeletedBy = GetUserId();
            baseClass.DeletedAt = DateTime.Now;
            baseClass.IsActive = false;
        }
        public void ReActive(BaseClass baseClass)
        {
            baseClass.IsActive = true;
        }
    }
}
