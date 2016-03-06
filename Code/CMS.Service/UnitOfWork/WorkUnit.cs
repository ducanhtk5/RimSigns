using System;
using CMS.Data;

namespace CMS.Service.UnitOfWork
{
    public class WorkUnit : IUnitOfWork, IDisposable
    {
        public CMSDBEntities Context { get; private set; }

        public WorkUnit()
        {
            Context = new CMSDBEntities();
        }

        public void Save()
        {
            if (Context != null)
            {
                Context.SaveChanges();
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}
