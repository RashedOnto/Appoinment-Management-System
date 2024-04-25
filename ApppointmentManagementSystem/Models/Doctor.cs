using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Models
{
    public class Doctor:IdentityModel
    {
        public string Specialty { get; set; }
        public string Language { get; set; }
        public string Institute { get; set; }
        public string Description { get; set; }
        public string BMDCNo { get; set; }
    }
}
