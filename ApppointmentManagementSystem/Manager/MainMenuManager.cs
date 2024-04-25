using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;

namespace AppointmentManagementSystem.Manager
{
    public class MainMenuManager:CommonManager<Menu>,IMainMenuManager
    {
        public MainMenuManager(ApplicationDbContext db):base(new CommonRepository<Menu>(db))
        {
        }
    }
}
