using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IAppointmentTypeManager:ICommonManager<AppointmentType>
    {
        AppointmentType GetById(int id);
        ICollection<AppointmentType> GetAll();
    }
}
