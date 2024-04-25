namespace AppointmentManagementSystem.ViewModels
{
    public class MainMenuVm
    {
        public int MainMenuId { get; set; }
        public string MainMenuName { get; set; }
        public List<RoleSubMenuVm> SubmenuList { get; set; }
    }
    public class RoleSubMenuVm
    {
        public int SubmenuId { get; set; }
        public string SubmenuName { get; set; }
        public bool IsChecked { get; set; }
    }
}
