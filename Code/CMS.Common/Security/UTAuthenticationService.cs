using System.Web.Security;
using CMS.Common.Security;

namespace CMS.Common.Security
{
    public class UTAuthenticationService : IAuthenticationService
    {
        public void LogIn(string email)
        {
            FormsAuthentication.SetAuthCookie(email, true);
        }

        public void LogOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
