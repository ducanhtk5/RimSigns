using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class FileUploadModels
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderTitle { get; set; }
        public int UploadById { get; set; }
        public string UploadByName { get; set; }
        public DateTime DateUpload { get; set; }
        public int ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime DateModified { get; set; }
        public int CountModified { get; set; }
        public bool Published { get; set; }
        public string FileName { get; set; }

        public static void ToModel(FileUpload entity, ref FileUploadModels model)
        {
            model.Id = entity.Id;
            model.OrderId = (int)entity.OrderId;
            model.OrderTitle = entity.OrderTitle;
            model.UploadById = (int)entity.UploadById;
            model.UploadByName = entity.UploadByName;
            model.DateUpload = (DateTime)entity.DateUpload;
            model.ModifiedById = (int)entity.ModifiedById;
            model.ModifiedByName = entity.ModifiedByName;
            model.DateModified = (DateTime)entity.DateModified;
            model.CountModified = (int)entity.CountModified;
            model.Published = (bool)entity.Published;
            model.FileName = entity.FileName;            
        }
        public static void ToEntity(FileUploadModels model, ref FileUpload entity)
        {
            entity.Id = model.Id;
            entity.OrderId = model.OrderId;
            entity.OrderTitle = model.OrderTitle;
            entity.UploadById = model.UploadById;
            entity.UploadByName = model.UploadByName;
            entity.DateUpload = model.DateUpload;
            entity.ModifiedById = model.ModifiedById;
            entity.ModifiedByName = model.ModifiedByName;
            entity.DateModified = model.DateModified;
            entity.CountModified = model.CountModified;
            entity.Published = model.Published;
            entity.FileName = model.FileName;
        }
    }
}