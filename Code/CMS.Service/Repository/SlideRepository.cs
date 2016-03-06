using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMS.Data;
using CMS.Service.UnitOfWork;
using System.Data;
using System.Data.Entity;

namespace CMS.Service.Repository
{
    public class SlideRepository : RepositoryBase<Slide>
    {
        public SlideRepository()
            : base()
        {
        }

        public SlideRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Slide entity)
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
                var entity = Context.Slides.SingleOrDefault(o => o.Id == entityId);
                Context.Slides.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Slide GetById(int entityId)
        {
            try
            {
                return Context.Slides.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<Slide> List()
        {
            try
            {
                return Context.Slides.ToList();
            }
            catch { return null; }
        }

        public override List<Slide> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }

    }
}
