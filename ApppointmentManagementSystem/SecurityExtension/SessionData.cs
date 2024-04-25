namespace AppointmentManagementSystem.SecurityExtension
{
    public class SessionData
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionData(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string LogCurrentUserEmail()
        {
            var email = _httpContextAccessor.HttpContext.User.Identity.GetUserEmail();
            return email;

        }
    }
}
