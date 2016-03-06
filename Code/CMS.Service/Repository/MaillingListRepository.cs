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
    public class MaillingListRepository : RepositoryBase<ListMail>
    {
        public MaillingListRepository()
            : base()
        {
        }

        public MaillingListRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(ListMail entity)
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
                var entity = Context.ListMails.SingleOrDefault(o => o.Id == entityId);
                Context.ListMails.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }
        public string DeleteByName(string entityName)
        {
            try
            {
                var entity = Context.ListMails.SingleOrDefault(o => o.Email == entityName);
                Context.ListMails.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override ListMail GetById(int entityId)
        {
            try
            {
                return Context.ListMails.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public ListMail GetByName(string entityName)
        {
            try
            {
                return Context.ListMails.SingleOrDefault(p => p.Email == entityName);
            }
            catch { return null; }
        }


        public override List<ListMail> List()
        {
            try
            {
                return Context.ListMails.ToList();
            }
            catch { return null; }
        }

        public override List<ListMail> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }
    }
}
