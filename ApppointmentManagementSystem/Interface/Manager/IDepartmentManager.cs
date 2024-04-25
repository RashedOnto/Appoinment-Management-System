using AppointmentManagementSystem.Models;
using System.Net.NetworkInformation;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IDepartmentManager : ICommonManager<Department>
    {
        Department GetById(int id);
        ICollection<Department> GetAllActiveData();
       
    }
}
