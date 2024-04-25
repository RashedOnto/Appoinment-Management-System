using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class VisitorManager : CommonManager<Visitor>, IVisitorManager
    {
        public VisitorManager(ApplicationDbContext db):base(new CommonRepository<Visitor>(db)) { }

        public Visitor GetById(string id)
        {
            return GetFirstOrDefault(c => c.Id == id);
        }

        public Visitor GetByMobile(string mobile)
        {
            return GetFirstOrDefault(c => c.Mobile == mobile && c.UserType == "Visitor");
        }
    }
}
