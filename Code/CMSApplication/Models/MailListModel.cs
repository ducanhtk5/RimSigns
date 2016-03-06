using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class MailListModel
    {
            public int Id { get; set; }
            public string Email { get; set; }
            public bool Block { get; set; }
            public int Num { get; set; }
            public DateTime DateTime { get; set; }

            public static void ToModel(ListMail entity, ref MailListModel model)
            {
                model.Id = entity.Id;
                model.Email = entity.Email;
                model.Block = (bool)entity.Block;
                model.Num = (int)entity.Num;
                model.DateTime = (DateTime)entity.Date;
            }

            public static void ToEntity(MailListModel model, ref ListMail entity)
            {
                entity.Id = model.Id;
                entity.Email = model.Email;
                entity.Block = model.Block;
                entity.Num = model.Num;
                entity.Date = model.DateTime;
            }
    }
}