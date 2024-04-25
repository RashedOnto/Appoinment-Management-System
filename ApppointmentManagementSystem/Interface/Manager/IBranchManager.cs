using System.Collections;
using System.Collections.Generic;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface.Manager;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IBranchManager:ICommonManager<Branch>
    {
        Branch GetById(int id);
        ICollection<Branch> GetAll();
        ICollection<Branch> GetByUserId(string userId);


    }
}
