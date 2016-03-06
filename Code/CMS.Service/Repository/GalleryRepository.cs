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
    public class GalleryRepository : RepositoryBase<Gallery>
    {
        public GalleryRepository()
            : base()
        {
        }

        public GalleryRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Gallery entity)
        {
            try
            {
                Context.Entry(entity).State = entity.ID == 0 ?
                                       EntityState.Added :
                                       EntityState.Modified;
                Context.SaveChanges();
                return entity.ID;
            }
            catch { return 0; }
        }
        public override string Delete(int entityId)
        {
            try
            {
                var entity = Context.Galleries.SingleOrDefault(o => o.ID == entityId);
                Context.Galleries.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Gallery GetById(int entityId)
        {
            try
            {
                return Context.Galleries.SingleOrDefault(p => p.ID == entityId);
            }
            catch { return null; }
        }

        public override List<Gallery> List()
        {
            try
            {
                return Context.Galleries.ToList();
            }
            catch { return null; }
        }

        public override List<Gallery> GetTableListById(int entityId)
        {
            try
            {
                return Context.Galleries.Where(m => m.CatalogID == entityId).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
