namespace AppointmentManagementSystem.Models
{
    public class DoctorServiceFee: BaseClass
    {
        public int Id { get; set; }
        public int DoctorServicesId { get; set; }
        public int AppointmentTypeId { get; set; }
        public double Fee { get; set; }
    }
}
