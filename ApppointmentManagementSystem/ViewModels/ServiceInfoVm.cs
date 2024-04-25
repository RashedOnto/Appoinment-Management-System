namespace AppointmentManagementSystem.ViewModels
{
    public class ServiceInfoVm
    {
        public int DoctorServiceId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        public string Days { get; set; }
        public int DayId { get; set; }
        public string Time { get; set; }
        public string VisitFee { get; set; }
        public int TotalPatient { get; set; }
        public int ExtraPatient { get; set; }
        public bool IsTimeSlotWise { get; set; }

    }
}
