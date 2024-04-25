using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Models
{
    public class IdentityModel:IdentityUser
    {
        public string UserType { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Mobile { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string RoleId { get; set; }
        public int DesignationId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
