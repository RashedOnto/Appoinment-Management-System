using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Models
{
    public class Role:IdentityRole
    {
        public int  OrderNo { get; set; }
    }
}
