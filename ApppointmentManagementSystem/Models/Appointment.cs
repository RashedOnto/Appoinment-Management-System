namespace AppointmentManagementSystem.Models
{
    public class Appointment : BaseClass
    {
        public int Id { get; set; }
        public int AppointmentTypeId { get; set; }
        public string VisitorId { get; set; }
        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientGender { get; set; }
        public int DoctorServiceId { get; set; }
        public DateTime Date { get; set; }
        public string SLNo { get; set; }
        public string TimeSlot { get; set; }
        public string AppointmentStatus { get; set; }
        public double PaymentAmount { get; set; }
        public double DiscountAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitDate { get; set; }
        public bool IsSelfAppointment { get; set; }
    }
}
