using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class SubMenuManager:CommonManager<SubMenu>,ISubMenuManager
    {
        public SubMenuManager(ApplicationDbContext db) : base(new CommonRepository<SubMenu>(db))
        {

        }

        public ICollection<SubMenu> GetAll()
        {
            return Get(x => true);
        }
    }
}
