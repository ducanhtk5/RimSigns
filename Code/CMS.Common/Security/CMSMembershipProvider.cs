using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using CMS.Service.Repository;
using CMS.Data;
using CMS.Common.Helpers;

namespace CMS.Common.Security
{
    public class CMSMembershipProvider : MembershipProvider
    {
        private string applicationName = null;
        public override string ApplicationName
        {
            get
            {
                if (applicationName == null)
                {
                    // Mỗi lần ghép code vào thì thay đổi cái này.

                    applicationName = "CMSMemberShipClick4Restaurent";
                }
                return applicationName;
            }
            set { applicationName = value; }
        }

        private UserRepositry userService = null;

        public CMSMembershipProvider()
        {
            userService = new UserRepositry();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "CMSMembershipProvider";
            }
            base.Initialize(name, config);
        }    

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return userService.ChangePassword(username, oldPassword, newPassword);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            MembershipUser member = null;
            try
            {
                User user = userService.CreateUser(username,email, password);
                member = UserMapping.CreateMembershipUser(Name, user);

                status = MembershipCreateStatus.Success;
            }
            catch (Exception)
            {
                status = MembershipCreateStatus.UserRejected;
            }

            return member;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return userService.DeleteUserByUserName(username);
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return true; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection userColl = new MembershipUserCollection();
            User u = userService.GetUserByEmail(emailToMatch);
            if (u != null)
            {
                userColl.Add(UserMapping.CreateMembershipUser(ApplicationName, u));
                totalRecords = 1;
            }
            else
            {
                totalRecords = 0;
            }
            return userColl;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return FindUsersByEmail(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            List<User> users = userService.List();
            List<User> selectedUsers = users.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            MembershipUserCollection userColl = UserMapping.CreateMembershipUser(ApplicationName, selectedUsers);
            totalRecords = users.Count;

            return userColl;
        }

        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            User u = userService.GetUserByEmail(username);
            if (u != null)
            {
                return UserMapping.CreateMembershipUser(ApplicationName, u);
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }
            if (!(providerUserKey is int))
            {
                throw new ArgumentException("Membership_InvalidProviderUserKey", "providerUserKey");
            }

            User u = userService.GetById((int)providerUserKey);
            if (u != null)
            {
                return UserMapping.CreateMembershipUser(ApplicationName, u);
            }
            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            User u = userService.GetUserByEmail(email);
            if (u != null)
            {
                return email;
            }
            return null;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 99; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return Parameters.GetValueInt("MinRequiredPasswordLength", 8); }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Clear; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            User u = userService.LogIn(username, password);
            if (u != null)
            {
                return true;
            }
            return false;
        }
    }
}
