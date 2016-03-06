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
    public class CommentRepository : RepositoryBase<Comment>
    {
         public CommentRepository()
            : base()
        {
        }

        public CommentRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Comment entity)
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
                var entity = Context.Comments.SingleOrDefault(o => o.Id == entityId);
                Context.Comments.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Comment GetById(int entityId)
        {
            try
            {
                return Context.Comments.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<Comment> List()
        {
            try
            {
                return Context.Comments.ToList();
            }
            catch { return null; }
        }

        public override List<Comment> GetTableListById(int entityId)
        {
            try
            {
                return Context.Comments.Where(p=>p.OrderId.Value.Equals(entityId)).OrderByDescending(c=>c.CreatedTime).ToList();
            }
            catch { return null; }
        }
    }
    
}
