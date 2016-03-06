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
    public class OrderRepository : RepositoryBase<Order>
    {
        public OrderRepository()
            : base()
        {
        }

        public OrderRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Order entity)
        {
            try
            {
                if (entity.Id.Equals(0))
                {
                    Context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    var currentProduct = Context.Orders.Find(entity.Id);
                    Context.Entry(currentProduct).CurrentValues.SetValues(entity);
                }

                Context.SaveChanges();
                return entity.Id;
            }
            catch { return 0; }
        }

        public override string Delete(int entityId)
        {
            try
            {
                var entity = Context.Orders.SingleOrDefault(o => o.Id == entityId);
                Context.Orders.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Order GetById(int entityId)
        {
            try
            {
                return Context.Orders.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<Order> List()
        {
            try
            {
                return Context.Orders.ToList();
            }
            catch { return null; }
        }

        public override List<Order> GetTableListById(int entityId)
        {
            try
            {
                return Context.Orders.Where(p=>p.OrderById.Equals(entityId)).ToList();
            }
            catch { return null; }
        }

        public List<Order> FullSearch(string key, string userName = "")
        {
            try
            {
                key = key.Trim();
                return Context.Orders.Where(c => (c.Title.Contains(key) || c.Address.Contains(key) || c.Parties.Contains(key) || c.Size.Contains(key) || c.Status.Contains(key) || c.Type.Contains(key)) && c.OrderByName.Contains(userName)).ToList();
            }
            catch { return null; }
        }

        public List<Order> SortByDay(DateTime day_from, DateTime day_to, string userName = "")
        {
            try
            {
                return Context.Orders.Where(c => c.OrderDate >= day_from && c.OrderDate <= day_to && c.OrderByName.Contains(userName)).ToList();
            }
            catch { return null; }
        }
    }
}
