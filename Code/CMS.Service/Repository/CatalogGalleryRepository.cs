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
    public class CatalogGalleryRepository : RepositoryBase<CatalogGallery>
    {
        public CatalogGalleryRepository()
            : base()
        {
        }

        public CatalogGalleryRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(CatalogGallery entity)
        {
            try
            {
                Context.Entry(entity).State = entity.CatalogID == 0 ?
                                       EntityState.Added :
                                       EntityState.Modified;
                Context.SaveChanges();
                return entity.CatalogID;
            }
            catch { return 0; }
        }
        public override string Delete(int entityId)
        {
            try
            {
                var entity = Context.CatalogGalleries.SingleOrDefault(o => o.CatalogID == entityId);

                while (entity.Galleries.Count() > 0)
                {
                    var x = entity.Galleries.First();
                    Context.Galleries.Remove(x);
                    entity.Galleries.Remove(x);
                    Context.SaveChanges();
                }

                Context.CatalogGalleries.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override CatalogGallery GetById(int entityId)
        {
            try
            {
                return Context.CatalogGalleries.SingleOrDefault(p => p.CatalogID == entityId);
            }
            catch { return null; }
        }

        public override List<CatalogGallery> List()
        {
            try
            {
                return Context.CatalogGalleries.ToList();
            }
            catch { return null; }
        }

        public override List<CatalogGallery> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }

    }
}
