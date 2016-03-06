using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using CMS.Data;
using CMS.Service.UnitOfWork;

namespace CMS.Service.Repository
{
    public class UserInRolesRepository : RepositoryBase<UserInRole>
    {
        public UserInRolesRepository()
            : base()
        {
        }

        public UserInRolesRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(UserInRole entity)
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
                var entity = Context.UserInRoles.SingleOrDefault(o => o.Id == entityId);
                Context.UserInRoles.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override UserInRole GetById(int entityId)
        {
            try
            {
                return Context.UserInRoles.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<UserInRole> List()
        {
            try
            {
                return Context.UserInRoles.ToList();
            }
            catch { return null; }
        }

        public override List<UserInRole> GetTableListById(int entityId)
        {
            return null;
        }

        public UserInRole GetByUserId(int entityId)
        {
            try
            {
                return Context.UserInRoles.SingleOrDefault(c => c.UserId.Equals(entityId));
            }
            catch(Exception ex) { return null; }
        }


        public void DeleteByRolesID ( int rolesID)
        {
            try
            {
                var c = Context.UserInRoles.Where(o => o.RolesId == rolesID).ToList();
                foreach (var g in c)
                {
                    Context.UserInRoles.Remove(g);
                    Context.SaveChanges();
                }
            }
            catch {  }
        }

        public void DeleteByUserID(int userID)
        {
            try
            {
                var c = Context.UserInRoles.Where(o => o.UserId == userID).ToList();
                foreach (var g in c)
                {
                    Context.UserInRoles.Remove(g);
                    Context.SaveChanges();
                }
            }
            catch { }
        }
    }
}