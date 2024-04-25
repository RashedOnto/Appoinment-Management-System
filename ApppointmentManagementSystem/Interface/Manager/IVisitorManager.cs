using AppointmentManagementSystem.Models;
using System.Net.NetworkInformation;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IVisitorManager : ICommonManager<Visitor>
    {
        Visitor GetById(string id);
        Visitor GetByMobile(string mobile);
       
    }
}
