namespace AppointmentManagementSystem.ViewModels
{
    public class DynamicMenuVm
    {
        public int MainMenuId { get; set; }
        public string Icon { get; set; }
        public string MainMenuName { get; set; }
        public string ActiveMenuId { get; set; }
        public string AreaName { get; set; }
        public int? Order { get; set; }
        public List<SubmenuVm> SubMenuLists { get; set; }
    }


    public class SubmenuVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActiveSubMenuId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string AreaName { get; set; }
        public bool IsAdminOnly { get; set; }

        // From Menu
        public string MenuName { get; set; }
        public string ActiveMenuId { get; set; }
        public int MenuId { get; set; }
        public int? Order { get; set; }
        public string Icon { get; set; }

    }
}
