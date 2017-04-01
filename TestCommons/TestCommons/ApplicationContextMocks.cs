using ToleLibraries.ApplicationContext;

namespace ToleLibraries.TestCommons
{
    public class AuthenticatedApplicationContext : IApplicationContext
    {
        public bool IsAuthenticated => true;
        public string UserId => "jros";
    }
    public class GuestApplicationContext : IApplicationContext
    {
        public bool IsAuthenticated => false;
        public string UserId => "false";
    }
}
