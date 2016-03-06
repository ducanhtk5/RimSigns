using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CMS.Data;
using CMS.Service.Repository;

namespace CMS.Common.Mvc   
{
    public class UTContext
    {
        public static readonly string SessionContextKey = "CMSessionContextRemsign";

        public int UserId { get; private set; }

        private User user = null;
        public User User
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    return null;
                }

                if (user == null)
                {
                    string userName = HttpContext.Current.User.Identity.Name;
                    if (!string.IsNullOrEmpty(userName) && user == null)
                    {
                        UserRepositry service = new UserRepositry();
                        user = service.GetUserByUserName(userName);
                        UserId = user.Id;
                    }
                }
                return user;
            }
            private set { user = value; }
        }

        public void LogOut()
        {
            User = null;
            UserId = 0;
        }
    }
}
