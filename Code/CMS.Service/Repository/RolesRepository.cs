using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Linq;
using CMS.Data;
using CMS.Service.UnitOfWork;
using CMS.Service.Repository;

namespace CMS.Service.Repository
{
    public class RolesRepository : RepositoryBase<Role>
    {
        public RolesRepository()
            : base()
        {
        }

        public RolesRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public override int CreateOrUpdate(Role entity)
        {
            try
            {
                Context.Entry(entity).State = entity.Id == 0 ?
                                       EntityState.Added :
                                       EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch { return 0; }
        }

        public override string Delete(int entityId)
        {
            try
            {
                var entity = Context.Roles.SingleOrDefault(o => o.Id == entityId);
                Context.Roles.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Role GetById(int entityId)
        {
            try
            {
                return Context.Roles.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<Role> List()
        {
            try
            {
                return Context.Roles.ToList();
            }
            catch { return null; }
        }

        public override List<Role> GetTableListById(int entityId)
        {
            try
            {
                var c = from i in Context.Users
                        join k in Context.UserInRoles on i.Id equals k.UserId
                        join l in Context.Roles on k.RolesId equals l.Id
                        where i.Id == entityId
                        select l;

                return c.ToList();
            }
            catch { return null; }
        }

        public bool checkRolesExits(string systeName)
        {
            try
            {
                var c = Context.Roles.SingleOrDefault(p => p.Name == systeName);
                if (c != null) return true;
                else return false;
            }
            catch { return false; }
        }

        public void AddUserInrolesID(int userId, int RoledID)
        {
            try
            {
                UserInRole UIR = new UserInRole();
                UIR.RolesId = RoledID;
                UIR.Id = userId;
                Context.UserInRoles.Add(UIR);
                Context.SaveChanges();
            }
            catch { }
        }

        public void removeRolesInUsser(int userId, int RoledID)
        {
            try
            {
                var c = Context.UserInRoles.SingleOrDefault(k => k.UserId == userId && k.RolesId == RoledID);
                Context.UserInRoles.Remove(c);
                Context.SaveChanges();
            }
            catch { }
        }

        public UserInRole checkExitsRolesiD(int userId)
        {
            try
            {
                var c = Context.UserInRoles.Where(k => k.UserId == userId).First();
                return c;
            }
            catch { return null; }
        }

        public void UpdateUserInroles(int userId, int rolesID)
        {
            try
            {
                var c = Context.UserInRoles.Where(k => k.UserId == userId).First();
                c.RolesId = rolesID;
                Context.SaveChanges();
            }
            catch {  }
        }
    }
}
