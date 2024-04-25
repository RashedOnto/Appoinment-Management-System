namespace AppointmentManagementSystem.ViewModels
{
    public class DoctorVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BMDCNo { get; set; }
        public string Image { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string Specialty { get; set; }
        public string Language { get; set; }
        public string Institute { get; set; }
        public string Description { get; set; }
    }
}
