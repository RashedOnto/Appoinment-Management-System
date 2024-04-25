namespace AppointmentManagementSystem.Models
{
    public class DoctorServices : BaseClass
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int DepartmentId { get; set; }
        public string DoctorId { get; set; }
        public int WeekDaysId { get; set; }
        public string ServiceTime { get; set; }
        public int TotalPatient { get; set; }
        public int ExtraPatient { get; set; }
        public bool IsTimeSlotWise { get; set; }

    }
}
