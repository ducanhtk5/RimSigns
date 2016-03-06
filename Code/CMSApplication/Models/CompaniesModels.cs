using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class CompaniesModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Domain { get; set; }
        public int Enable { get; set; }

        public static void ToModel(Company entity, ref CompaniesModels model)
        {
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.Description = entity.Description;
            model.Note = entity.Note;
            model.Domain = entity.Domain;
            model.Enable = (int)entity.Enable;
        }

        public static void ToEntity(CompaniesModels model, ref  Company entity)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Note = model.Note;
            entity.Domain = model.Domain;
            entity.Enable = (int)model.Enable;
        }
    }
}