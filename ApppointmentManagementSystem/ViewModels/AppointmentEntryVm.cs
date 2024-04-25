namespace AppointmentManagementSystem.ViewModels
{
    public class AppointmentEntryVm
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientGender { get; set; }
        public string VisitorMobile { get; set; }
        public string VisitorAddress { get; set; }
        public int AppointmentTypeId { get; set; }
        public string AppointmentStatus { get; set; }
        public string SLNO { get; set; }
        public string AppointmentTypeName { get; set; }
        public string VisitorId { get; set; }
        public string DoctorId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DoctorServiceId { get; set; }
        public string DoctorName { get; set; }
        public double PaymentAmount { get; set; }
        public double DiscountAmount { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitDate { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }

    }
}
