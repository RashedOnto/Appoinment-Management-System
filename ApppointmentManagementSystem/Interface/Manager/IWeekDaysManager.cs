using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IWeekDaysManager : ICommonManager<WeekDays>
    {
        ICollection<WeekDays> GetAllWeekDays();
    }
}
