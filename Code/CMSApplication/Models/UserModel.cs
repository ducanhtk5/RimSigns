using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Data;

namespace CMSApplication.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public bool Detele { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateLogin { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string CodeActive { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public List<MoreInfoAdmin> ModelCheck { get; set; }
        // change Info
        public string EmailChange { get; set; }
        public string Emailchange2 { get; set; }
        public string Tendangnhapchange { get; set; }
        public string NameAddUser { get; set; }

        //extra
        public int rolesId { get; set; }
        public static void ToEntity(UserModel model, ref User entity)
        {
            entity.Id = model.Id;
            entity.FullName = model.FullName;
            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.Password = model.Password;
            entity.Status = model.Status;
            entity.Description = model.Description;
            entity.Detele = model.Detele;
            entity.DateCreate = model.DataCreate;
            entity.DateUpdate = model.DateUpdate;
            entity.DateLogin = model.DateLogin;
            entity.Address = model.Address;
            entity.Zipcode = model.Zipcode;
            entity.Country = model.Country;
            entity.CodeActive = model.CodeActive;
            entity.CompanyName = model.CompanyName;
            entity.CompanyLogo = model.CompanyLogo;
            entity.Phone = model.Phone;
        }

        public static void ToModel(User entity, ref UserModel model)
        {
            model.Id = entity.Id;
            model.FullName = entity.FullName;
            model.UserName = entity.UserName;
            model.Email = entity.Email;
            model.Password = entity.Password;
            model.Status = entity.Status;
            model.Description = entity.Description;
            model.Detele = entity.Detele;
            model.DataCreate = entity.DateCreate;
            model.DateUpdate = entity.DateUpdate;
            model.DateLogin = entity.DateLogin;
            model.Address = entity.Address;
            model.Zipcode = entity.Zipcode;
            model.Country = entity.Country;
            model.CodeActive = entity.CodeActive;
            model.CompanyName = entity.CompanyName;
            model.CompanyLogo = entity.CompanyLogo;
            model.Phone = entity.Phone;
        }

    }

    public class MoreInfoAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checkbox { get; set; }
    }
}