namespace AppointmentManagementSystem.ViewModels
{
    public class WeekDaysVm
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public bool IsTimeSlotWise { get; set; }
        public int DoctorServiceId { get; set; }
    }
}
