namespace AppointmentManagementSystem.SecurityExtension
{
    public class MyCustomAuthenticationMiddlewarePipeline
    {

        public void Configure(IApplicationBuilder app)
        {
            app.UseMyCustomAuthentication();
        }
    }
}
