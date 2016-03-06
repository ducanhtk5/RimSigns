using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Common.Security
{
    public interface IAuthenticationService
    {
        void LogIn(string email);
        void LogOut();
    }
}
