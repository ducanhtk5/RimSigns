using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMS.Service.UnitOfWork;
using CMS.Data;
using System.Data;
using System.Data.Entity;

namespace CMS.Service.Repository
{
    public class NavigationRepository : RepositoryBase<Navigation>
    {
        public NavigationRepository()
            : base()
        {
        }

        public NavigationRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Navigation entity)
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
                var entity = Context.Navigations.SingleOrDefault(o => o.Id == entityId);
                Context.Navigations.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Navigation GetById(int entityId)
        {
            try
            {
                return Context.Navigations.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<Navigation> List()
        {
            try
            {
                return Context.Navigations.ToList();
            }
            catch { return null; }
        }

        public override List<Navigation> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }

        public string GetCategoryBreadCrumb(Navigation Navigations, NavigationRepository NavigationService)
        {
            string result = string.Empty;

            if (Navigations.Id == 0)
            {
                result = "[No parent]";
            }
            else
            {

                while (Navigations != null && Navigations.Active)
                {
                    if (String.IsNullOrEmpty(result))
                        result = Navigations.Name;
                    else
                        result = Navigations.Name + " >> " + result;

                    Navigations = NavigationService.GetById(Navigations.ParentID);

                }
            }
            return result;
        }
    }
}
