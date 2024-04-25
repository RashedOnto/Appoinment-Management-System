namespace AppointmentManagementSystem.Models
{
    public class SubMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MenuId { get; set; }
        public string ActiveSubMenuId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public int OrderNo { get; set; }
    }
}
