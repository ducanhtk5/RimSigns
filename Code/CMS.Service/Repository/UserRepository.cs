using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using CMS.Data;
using CMS.Service.UnitOfWork;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace CMS.Service.Repository
{
    public class UserRepositry : RepositoryBase<User>
    {

        public UserRepositry()
            : base()
        {
        }

        public UserRepositry(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(User entity)
        {
            try
            {
                Context.Entry(entity).State = entity.Id == 0 ?
                                       EntityState.Added :
                                       EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public override string Delete(int entityId)
        {
            try
            {
                var entity = Context.Users.SingleOrDefault(o => o.Id == entityId);
                Context.Users.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override User GetById(int entityId)
        {
            try
            {
                return Context.Users.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<User> List()
        {
            try
            {
                return Context.Users.ToList();
            }
            catch { return null; }
        }

        public override List<User> GetTableListById(int entityId)
        {
            try
            {
                var c = from i in Context.Users
                        join k in Context.UserInRoles on i.Id equals k.UserId
                        join l in Context.Roles on k.RolesId equals l.Id
                        where (k.RolesId == entityId || entityId == 0)
                        select i;
                return c.ToList();
            }
            catch { return null; }
        }

        #region Function


        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                var entityUser = Context.Users.First(k => k.UserName == username && k.Password == oldPassword);
                entityUser.Password = newPassword;
                Context.SaveChanges();
                return true;
            }
            catch { return false; }
        }


        public User CreateUser(string username, string password, string email)
        {
            try
            {
                User entityUser = new User();
                entityUser.UserName = username;
                entityUser.Password = password;
                entityUser.Email = email;
                Context.Users.Add(entityUser);
                Context.SaveChanges();
                return entityUser;
            }
            catch { return null; }
        }


        public bool DeleteUserByUserName(string userName)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.UserName == userName);
                Context.Users.Remove(entityUser);
                Context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.Email == email);
                return entityUser;
            }
            catch { return null; }
        }

        public User GetUserByUserName(string username)
        {
            try
            {
                var entityUser = Context.Users.FirstOrDefault(k => k.UserName == username);
                return entityUser;
            }
            catch { return null; }
        }
        public User GetUserByPassword(string pass)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.Password == pass);
                return entityUser;
            }
            catch { return null; }
        }

        public User GetUserByEnCodeActive(string codeactive)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.CodeActive == codeactive);
                return entityUser;
            }
            catch { return null; }
        }
        public User GetUserByUserEnCodePass(string username, string encode)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.Password == encode & k.UserName == username);
                return entityUser;
            }
            catch { return null; }
        }

        public User LogIn(string username, string Password)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.UserName == username && k.Password == Password);
                if (entityUser != null)
                    return entityUser;
                else return null;
            }
            catch { return null; }
        }

        public bool checkStatusUser(string username, string Password)
        {
            try
            {
                var entityUser = Context.Users.SingleOrDefault(k => k.UserName == username && k.Password == Password && k.Status == true);
                if (entityUser != null)
                    return true;
                else return false;
            }
            catch { return false; }
        }

        public string[] GetRolesForUser(string username)
        {
            var c = (from i in Context.Users
                     join k in Context.UserInRoles on i.Id equals k.UserId
                     join l in Context.Roles on k.RolesId equals l.Id
                     where i.UserName == username
                     select l.Name).ToArray();
            return c;
        }

        public string[] GetUsersInRole(string rolesName)
        {
            var c = (from i in Context.Users
                     join k in Context.UserInRoles on i.Id equals k.UserId
                     join l in Context.Roles on k.RolesId equals l.Id
                     where l.Name == rolesName
                     select i.UserName).ToArray();
            return c;
        }

        public List<User> GetUsersByRole(string rolesName)
        {
            var c = (from i in Context.Users
                     join k in Context.UserInRoles on i.Id equals k.UserId
                     join l in Context.Roles on k.RolesId equals l.Id
                     where l.Name == rolesName
                     select i).ToList();
            return c;
        }

        public bool checkUserInroles(string UserName, string RolesName)
        {
            try
            {
                var c = (from i in Context.Users
                         from j in Context.UserInRoles
                         from k in Context.Roles
                         where i.Id == j.Id && j.RolesId == k.Id
                         select i);
                if (c != null) return true;
                else return false;
            }
            catch { return false; }
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var c = (from i in Context.Users
                     join k in Context.UserInRoles on i.Id equals k.UserId
                     join l in Context.Roles on k.RolesId equals l.Id
                     where l.Name == roleName && i.UserName == usernameToMatch
                     select i.UserName).ToArray();
            return c;
        }

        #endregion


        public List<User> SearchUserByName(string query)
        {

            query = query.Replace(" ", "");
            if (query.Length > 1)
            {
                int op = query.LastIndexOf(",");
                query = query.Substring(op + 1);
            }

            var users = (from u in List()
                         where u.UserName.Contains(query)
                         orderby u.UserName
                         select u).Distinct().ToList();

            return users;
        }


        public List<User> FullSearch(string keys)
        {
            try
            {
                return Context.Users.Where(c => c.UserName.Contains(keys) || c.FullName.Contains(keys) || c.Email.Contains(keys)).ToList();       
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
