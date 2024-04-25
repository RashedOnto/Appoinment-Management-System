using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Models
{
    public class Visitor:IdentityModel
    {
        public int Age { get; set; }
        public string  Address { get; set; }
        public double Balance { get; set; }
    }
}
