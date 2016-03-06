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
    public class TopicRepository : RepositoryBase<Topic>
    {
        public TopicRepository()
            : base()
        {
        }

        public TopicRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Topic entity)
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
                var entity = Context.Topics.SingleOrDefault(o => o.ID == entityId);
                Context.Topics.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Topic GetById(int entityId)
        {
            try
            {
                return Context.Topics.SingleOrDefault(p => p.ID == entityId);
            }
            catch { return null; }
        }

        public override List<Topic> List()
        {
            try
            {
                return Context.Topics.ToList();
            }
            catch { return null; }
        }

        public override List<Topic> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }

        public Topic getTopiccByName(string sysName)
        {
            try
            {
                return Context.Topics.SingleOrDefault(p => p.SystemName == sysName);
            }
            catch { return null; }
        }
    }
}
