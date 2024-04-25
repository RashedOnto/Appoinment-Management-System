namespace AppointmentManagementSystem.Models
{
    public class DoctorQualifications: BaseClass
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string DegreeName { get; set; }
        public string InstituteName { get; set; }
        public string PassingYear { get; set; }
        public string Achievement { get; set; }
        public string Country { get; set; }

    }
}
