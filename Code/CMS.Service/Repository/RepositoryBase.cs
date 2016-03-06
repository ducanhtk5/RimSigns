using System;
using System.Collections.Generic;
using CMS.Service.UnitOfWork;
using CMS.Data;

namespace CMS.Service.Repository
{
    public abstract class RepositoryBase<T> : IDisposable
    {
        protected WorkUnit unit = null;
        private bool isWorkUnitGranular = true;

        public RepositoryBase()
        {
            unit = new WorkUnit();
        }

        public RepositoryBase(WorkUnit unit)
        {
            this.unit = unit;
            isWorkUnitGranular = false;
        }

        public void Dispose()
        {
            if (isWorkUnitGranular)
            {
                unit.Context.SaveChanges();
            }
            unit.Dispose();
        }

        public CMSDBEntities Context { get { return unit.Context; } }

        public abstract int CreateOrUpdate(T entity);

        public abstract string Delete(int entityId);

        public abstract T GetById(int entityId);

        public abstract List<T> List();

        public abstract List<T> GetTableListById(int entityId);

    }

}
