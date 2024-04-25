namespace AppointmentManagementSystem.ViewModels
{
    public class AppointmentVm
    {
        public int Id { get; set; }
        public int AppointmentTypeId { get; set; }
        public string AppointmentType { get; set; }
        public string VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string VisitorAddress { get; set; }
        public string VisitorDob { get; set; }
        public string Gender { get; set; }
        public int DoctorServiceId { get; set; }
        public string ServiceTime { get; set; }
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
