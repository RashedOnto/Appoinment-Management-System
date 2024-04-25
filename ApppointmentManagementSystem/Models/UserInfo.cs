using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Models
{
    public class UserInfo: IdentityModel
    {
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public int BranchId { get; set; }
      

    }
}
