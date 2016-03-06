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
    public class FileUploadRepository : RepositoryBase<FileUpload>
    {
        public FileUploadRepository()
            : base()
        {
        }

        public FileUploadRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override int CreateOrUpdate(FileUpload entity)
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
                var entity = Context.FileUploads.SingleOrDefault(o => o.Id == entityId);
                Context.FileUploads.Remove(entity);
                Context.SaveChanges();
                return "success";
            }
            catch { return "error"; }
        }

        public override FileUpload GetById(int entityId)
        {
            try
            {
                return Context.FileUploads.SingleOrDefault(p => p.Id == entityId);
            }
            catch { return null; }
        }

        public override List<FileUpload> List()
        {
            try
            {
                return Context.FileUploads.ToList();
            }
            catch { return null; }
        }

        public override List<FileUpload> GetTableListById(int entityId)
        {
            try
            {
                return Context.FileUploads.Where(c=>c.OrderId.Value.Equals(entityId)).OrderByDescending(c=>c.DateUpload).ToList();
            }
            catch { return null; }
        }
    }
}
