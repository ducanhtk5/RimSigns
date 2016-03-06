using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CMS.Data;

namespace CMS.Common.Security
{
    public class UserMapping
    {
        public static MembershipUser CreateMembershipUser(string appName, User user)
        {
            if (user != null)
            {
                return new MembershipUser(appName, user.Email, user.Id, user.Email, null, null, true, user.Status,
                    user.DateCreate, user.DateLogin, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
            }
            return null;
        }
        public static MembershipUserCollection CreateMembershipUser(string appName, List<User> userList)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            foreach (User user in userList)
            {
                MembershipUser mUser = CreateMembershipUser(appName, user);
                users.Add(mUser);
            }

            return users;
        }
    }
}
