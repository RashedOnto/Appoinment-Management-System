using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IDesignationManager : ICommonManager<Designation>
    {
       Designation GetById(int id);
       ICollection<Designation> GetActive();

    }
}
