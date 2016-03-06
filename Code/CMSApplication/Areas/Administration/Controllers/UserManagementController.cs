using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Common.Mvc;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;
using CMS.Common.Helpers;
using CMS.Data;
using System.IO;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class UserManagementController : UTController
    {
        private UserRepositry _user_srv = new UserRepositry();
        private EncryptAndDecrypt _Encrypt = new EncryptAndDecrypt();
        private RolesRepository _role_srv = new RolesRepository();
        private UserInRolesRepository _user_in_role_srv = new UserInRolesRepository();
        private OrderRepository _order_repo = new OrderRepository();
        private FileUploadRepository _file_repo = new FileUploadRepository();

        //
        // GET: /UserManagement/

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            List<Role> c = new List<Role>();
            c.Add(new Role { Id = 0, Name = "-- All --" });
            var ListRoles = _role_srv.List();
            c.AddRange(ListRoles);
            ViewData["ListRoles"] = new SelectList(c, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult List(int rolesId, int page)
        {
            List<UserModel> model = new List<UserModel>();
            var c = _user_srv.GetTableListById(rolesId).Where(m => m.UserName != "demo" && m.UserName != "trungdt").ToList();
            foreach (var item in c)
            {
                UserModel ml = new UserModel();
                UserModel.ToModel(item, ref  ml);
                ml.RoleName = _role_srv.GetById(_user_in_role_srv.GetByUserId(ml.Id).RolesId).Name;
                model.Add(ml);
            }

            var ListNewsPageSize = new PagedData<UserModel>();

            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
                if (ListNewsPageSize.Data.Count() == 0)
                {
                    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                }
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / PageSize));
                ListNewsPageSize.CurrentPage = page;
            }
            else
            {
                ListNewsPageSize.Data = new List<UserModel>();
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
            }

            return PartialView("_List", ListNewsPageSize);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add()
        {
            UserModel model = new UserModel();
            model.Status = true;
            model.rolesId = 0;
            List<Role> c = new List<Role>();
            var ListRoles = _role_srv.List();
            c.AddRange(ListRoles);
            ViewData["rolesId"] = new SelectList(c, "Id", "Name");
            return View(model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            UserModel model = new UserModel();
            var c = _user_srv.GetById(id);
            UserModel.ToModel(c, ref model);
            model.Password = _Encrypt.Decrypt(model.Password, "Click4Restaurants", true);
            var roles = _role_srv.checkExitsRolesiD(id);
            List<Role> List_role_srview = new List<Role>();
            var ListRoles = _role_srv.List();
            List_role_srview.AddRange(ListRoles);
            ViewData["ListRolesAdd"] = new SelectList(List_role_srview, "Id", "Name", roles.RolesId);
            model.rolesId = roles.RolesId;
            return View(model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult UpdateUser(UserModel model, HttpPostedFileBase FileUp)
        {
            model.DateLogin = DateTime.Now;
            model.DateUpdate = DateTime.Now;
            model.DateUpgrade = DateTime.Now;
            model.ExpiredDate = DateTime.Now;
            if (model.rolesId == 0)
            {
                return JsonError("Please select one role");
            }

            if (string.IsNullOrEmpty(model.FullName))
            {
                return JsonError("Please enter your full name");
            }

            if (!string.IsNullOrEmpty(model.EmailChange))
            {
                if (!IsValidEmailAddress(model.EmailChange))
                {
                    return JsonError("Email change is not valid");
                }

                var userEmail = _user_srv.GetUserByEmail(model.EmailChange);
                if (userEmail != null)
                {
                    return JsonError("The email you have choosen already exists. Please try another one.");
                }
                else
                {
                    model.Email = model.EmailChange;
                }
            }

            if (!string.IsNullOrEmpty(model.NameAddUser))
            {
                var userUserName = _user_srv.GetUserByUserName(model.NameAddUser);
                if (userUserName != null)
                {
                    return JsonError("The username you have choosen already exists. Please try another one.");
                }
                else
                {
                    model.UserName = model.NameAddUser;
                }
            }

            if (!string.IsNullOrEmpty(model.PassNews))
            {
                model.Password = model.PassNews;
            }

            model.Password = _Encrypt.Encrypt(model.Password, "Click4Restaurants", true);
            model.DateUpdate = DateTime.Now;
            if (FileUp != null)
                model.CompanyLogo = UploadFile(model.CompanyName, FileUp);

            User u = new User();
            UserModel.ToEntity(model, ref u);
            _user_srv.CreateOrUpdate(u);

            _role_srv.UpdateUserInroles(model.Id, model.rolesId);

            return JsonSuccess("/Administration");
            //return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult AddUser(UserModel model, HttpPostedFileBase FileUp)
        {
            if (model.rolesId == 0)
            {
                return JsonError("Please select one role");
            }

            if (string.IsNullOrEmpty(model.FullName))
            {
                return JsonError("Please enter your full name");
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                return JsonError("Please enter your email");
            }

            if (!IsValidEmailAddress(model.Email))
            {
                return JsonError("Email address not valid");
            }

            if (string.IsNullOrEmpty(model.UserName))
            {
                return JsonError("Please enter your username");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return JsonError("Please enter your password");
            }

            model.Password = _Encrypt.Encrypt(model.Password, "Click4Restaurants", true);
            var userEmail = _user_srv.GetUserByEmail(model.Email);
            if (userEmail != null)
            {
                return JsonError("The email you have chosen already exists. Please try another one.");
            }

            var userUserName = _user_srv.GetUserByUserName(model.UserName);
            if (userUserName != null)
            {
                return JsonError("The username you have chosen already exists. Please try another one.");
            }
            User u = new User();
            model.DataCreate = DateTime.Now;
            model.Detele = false;
            model.DateLogin = DateTime.Now;
            model.DateUpdate = DateTime.Now;
            model.DateUpgrade = DateTime.Now;
            model.ExpiredDate = DateTime.Now;
            model.Id = 0;
            if (FileUp != null)
                model.CompanyLogo = UploadFile(model.CompanyName, FileUp);
            UserModel.ToEntity(model, ref u);
            var us = _user_srv.CreateOrUpdate(u);

            UserInRole uinEnri = new UserInRole();
            uinEnri.Id = 0;
            uinEnri.RolesId = model.rolesId;
            uinEnri.UserId = us;
            _user_in_role_srv.CreateOrUpdate(uinEnri);
            return JsonSuccess("/Administration");
            //return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult deleteUser(int id)
        {
            var u = _user_srv.GetById(id);
            if (u != null && u.Id > 0)
            {

                //_user_in_role_srv.DeleteByUserID(id);
                _user_srv.Delete(id);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return JsonError("User not found");
            }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult FullSearch(string keys)
        {
            List<UserModel> model = new List<UserModel>();
            var c = _user_srv.FullSearch(keys);
            foreach (var item in c)
            {
                UserModel ml = new UserModel();
                UserModel.ToModel(item, ref  ml);
                model.Add(ml);
            }

            var ListNewsPageSize = new PagedData<UserModel>();
            ListNewsPageSize.Data = model;
            return PartialView("_List", ListNewsPageSize);
        }


        private string UploadFile(string galleryName, HttpPostedFileBase FileUp)
        {
            var db_path = "";

            if (string.IsNullOrEmpty(galleryName))
            {
                galleryName = "Default";
            }
            string folder = galleryName.Replace("&", "").Replace("/", "").Replace("?", "");
            string filenamefinal = string.Empty;
            folder = EscapeName.Renamefile(folder);
            if (FileUp != null)
            {
                if (FileUp.ContentLength > 0)
                {
                    var dir = Server.MapPath("~/Content/File/Remsigns/");
                    var path = "";

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    var ext = Path.GetExtension(FileUp.FileName).ToLower();
                    if (ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    {
                        dir = Server.MapPath("~/Content/File/Remsigns");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        var filename = Path.GetFileNameWithoutExtension(FileUp.FileName);
                        //filename = EscapeName.Renamefile(filename);
                        string tmp = Path.GetRandomFileName().Substring(0, 3);
                        filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                        path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                        db_path = "Content/File/Remsigns/" + filenamefinal;
                        //FileUp.SaveAs(path);
                        filenamefinal = CMS.Common.Helpers.ResizeImage.ResizeByMaxWidth(dir, FileUp, 100, 100);
                    }
                }
            }
            return filenamefinal;
        }
        public ActionResult SendEmail(string email, string subject, string body)
        {
            CMS.Service.Common.SendEmail.SendMail(email, subject, body);
            return RedirectToAction("Index");
        }

    }
}
