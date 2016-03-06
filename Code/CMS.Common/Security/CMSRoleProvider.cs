using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CMS.Service.Repository;
using CMS.Data;

namespace CMS.Common.Security
{
    public class CMSRoleProvider : RoleProvider
    {
        private UserRepositry userService = null;

        public CMSRoleProvider()
        {
            userService = new UserRepositry();
        }

        private string applicationName = null;
        public override string ApplicationName
        {
            get
            {
                if (applicationName == null)
                {
                    // Doi ten
                    applicationName = "CMSMemberShipRestaurant";
                }
                return applicationName;
            }
            set { applicationName = value; }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {

        }

        public override void CreateRole(string roleName)
        {
            return;
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            try
            {
                return userService.FindUsersInRole(roleName, usernameToMatch);
            }
            catch
            {
                return new string[] { string.Empty };
            }
        }

        public override string[] GetAllRoles()
        {
            return Enum.GetValues(typeof(Role)).Cast<string>().ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                return userService.GetRolesForUser(username);
            }
            catch
            {
                return new string[] { string.Empty };
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            try
            {
                return userService.GetUsersInRole(roleName);
            }
            catch
            {
                return new string[] { string.Empty };
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return userService.checkUserInroles(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
