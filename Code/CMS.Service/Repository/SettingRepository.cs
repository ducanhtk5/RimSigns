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
    public class SettingRepository : RepositoryBase<Setting>
    {
        public SettingRepository()
            : base()
        {
        }

        public SettingRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(Setting entity)
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
                var entity = Context.Settings.SingleOrDefault(o => o.Id == entityId);
                Context.Settings.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override Setting GetById(int entityId)
        {
            try
            {
                return Context.Settings.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }
        public Setting SettingByName(string SettingName)
        {
            try
            {
                return Context.Settings.SingleOrDefault(p => p.SettingName == SettingName);
            }
            catch (Exception)
            {
                
                return null;
            }
        }
        public override List<Setting> List()
        {
            try
            {
                return Context.Settings.ToList();
            }
            catch { return null; }
        }

        public override List<Setting> GetTableListById(int entityId)
        {
            throw new NotImplementedException();
        }
    }
}
