using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.ViewModels
{
    public class AppointmentListVm
    {
       
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentTypeName { get; set; }
        public string DoctorId { get; set; }
        public int DepartmentId { get; set; }
        public int Id { get; set; }
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
        public int WeekDayId { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public DateTime Today { get; set; }
    }
}
