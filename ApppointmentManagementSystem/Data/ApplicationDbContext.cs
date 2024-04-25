using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchWiseDepartment> BranchWiseDepartments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorQualifications> DoctorQualifications { get; set; }
        public DbSet<DoctorServiceFee> DoctorServiceFees { get; set; }
        public DbSet<DoctorServices> DoctorServices { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<WeekDays> WeekDays { get; set; }  
        public DbSet<BranchPermission> BranchPermissions { get; set; }
    }
}