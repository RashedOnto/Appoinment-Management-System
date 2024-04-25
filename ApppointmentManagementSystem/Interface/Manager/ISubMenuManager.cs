using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface ISubMenuManager: ICommonManager<SubMenu>
    {
        ICollection<SubMenu> GetAll();
    }
}
