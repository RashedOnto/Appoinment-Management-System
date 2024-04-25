using System.Collections;
using System.Collections.Generic;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Interface.Manager;

namespace AppointmentManagementSystem.Interface.Manager
{
    interface IBranchWiseDepartmentManager:ICommonManager<BranchWiseDepartment>
    {
        BranchWiseDepartment GetById(int id);
        ICollection<BranchWiseDepartment> GetAll();
        ICollection<BranchWiseDepartment> GetDepartmentByBranchId(int branchId);
        ICollection<Department> GetDepartmentData(int branchId);


    }
}
