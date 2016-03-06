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
    public class OrderActivitiesRepository : RepositoryBase<OrderActivity>
    {

        public OrderActivitiesRepository()
                : base()
            {
            }

            public OrderActivitiesRepository(WorkUnit unit)
                : base(unit)
            {
            }

            public override int CreateOrUpdate(OrderActivity entity)
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
                    var entity = Context.OrderActivities.SingleOrDefault(o => o.Id.Equals(entityId));
                    Context.OrderActivities.Remove(entity);
                    Context.SaveChanges();
                    return "success";
                }
                catch { return "error"; }
            }

            public override OrderActivity GetById(int entityId)
            {
                try
                {
                    return Context.OrderActivities.SingleOrDefault(p => p.Id == entityId);
                }
                catch { return null; }
            }

            public override List<OrderActivity> List()
            {
                try
                {
                    return Context.OrderActivities.ToList();
                }
                catch { return null; }
            }

            public override List<OrderActivity> GetTableListById(int entityId)
            {
                try
                {
                    return Context.OrderActivities.Where(p => p.OrderUserID.Value.Equals(entityId)).OrderByDescending(p=> p.CreatedTime).ToList();
                }
                catch { return null; }
            }
            public List<OrderActivity> GetTableListByOrderId(int entityId)
            {
                try
                {
                    return Context.OrderActivities.Where(p => p.OrderID.Value.Equals(entityId)).ToList();
                }
                catch { return null; }
            }

            public List<OrderActivity> GetActivityByPage(int entityId, int page, int num)
            {
                try
                {
                    return Context.OrderActivities.Where(p => p.OrderUserID.Value.Equals(entityId)).OrderByDescending(p => p.CreatedTime).Skip(page).Take(num).ToList();
                }
                catch { return null; }
            }
        }
    
}
