using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Service.Common;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Models;
using System.Xml;
using System.Data;
using CMS.Common.Helpers;
using System.Web.Security;
using CMS.Common.Security;
using CMS.Common.Mvc;
using System.IO;
using CMSApplication.Areas.Administration.Controllers;
using MvcReCaptcha;

namespace CMSApplication.Controllers
{
    public class HomeController : UTController
    {
        SlideRepository _slide_srv = new SlideRepository();
        private EncryptAndDecrypt encrypt = new EncryptAndDecrypt();
        private UserRepositry userSv = new UserRepositry();
        private UserInRolesRepository UsInRlSv = new UserInRolesRepository();
        private UTAuthenticationService utAuSv = new UTAuthenticationService();
        private MaillingListRepository _mail_srv = new MaillingListRepository();
        private OrderActivitiesRepository orderAct_services = new OrderActivitiesRepository();
        private UserRepositry _user_srv = new UserRepositry();
        private TopicRepository topic_services = new TopicRepository();
        private SettingRepository _setting_srv = new SettingRepository();

        public ActionResult Index()
        {
            List<SlideModel> model = new List<SlideModel>();
            var a = _slide_srv.List().OrderBy(m => m.OrderImage).ToList();
            foreach (var item in a)
            {
                SlideModel ml = new SlideModel();
                SlideModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }

            return View(model);
        }

        public ActionResult AboutUs()
        {
            TopicModel model = new TopicModel();
            var entity = topic_services.getTopiccByName("AboutUs");
            TopicModel.MapFrom(entity, ref model);
            return View(model);
        }

        public ActionResult ContactUs()
        {
            TopicModel model = new TopicModel();
            var entity = topic_services.getTopiccByName("contactus");
            TopicModel.MapFrom(entity, ref model);
            return View(model);
        }

        public ActionResult GetCaptcha()
        {
            string rs = "";
            Random ran = new Random();
            rs = ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString();
            return Content(rs);
        }

        public ActionResult SendContactMail(ContactUsModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Comment))
            {
                return Content("Please enter all required fields.");
            }

            if (!IsValidEmailAddress(model.Email))
            {
                return Content("Email address not valid");
            }

            if (CaptchaController.CheckCaptcha(model.CaptchaValue, Session["CaptchaHash"].ToString()))
            {
                Session["CaptchaHash"] = DateTime.Now.Ticks.ToString();
            }
            else
            {
                return Content("You enterted wrong captcha");
            }

            string subject = "Real Estate Media Contact System";
            string body = "";
            body += "Hi {Remsigns}";

            body += "<br/><br/>On " + DateTime.Now + ", there is contact message from visitor on our site.";

            body += "<br/>Here is the contact information:";
            body += "<br/><br/>Name: " + model.Name;
            body += "<br/>Email: " + model.Email;
            body += "<br/>Phone: " + model.Phone;
            //body += "<br/>State: " + model.State;
            body += "<br/><br/>Contact Message:" + model.Comment;

            body += "<br/><br/>Please dont reply this email.";
            body += "<br/>Regards";
            SendEmail.SendMail("", subject, body);

            body = "";
            body += "Hi " + model.Name;

            body += "<br/>Thanks for your contact message on " + DateTime.Now;

            body += "<br/>The message you submited on our website Real Estate Media:";
            body += "<br/><br/>Name: " + model.Name;
            body += "<br/>Email: " + model.Email;
            body += "<br/>Phone: " + model.Phone;
            //body += "<br/>State: " + model.State;
            body += "<br/><br/>Contact Message:" + model.Comment;

            body += "<br/><br/>We will check your message and contact you very soon.";
            body += "<br/>Regards";
            body += "<br/>Real Estate Media";
            SendEmail.SendMail(model.Email, subject, body);
            return Content("ok");
        }

        public ActionResult Dashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ListOrderActivities(int page)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<OrderActivitiesModels> orderActModel = new List<OrderActivitiesModels>();
                User _User = new User();
                string Username = HttpContext.User.Identity.Name;
                if (Username != null)
                {
                    _User = _user_srv.GetUserByUserName(Username);
                }
                else { return RedirectToAction("LogOn", "Management"); }

                var orderActEntity = orderAct_services.GetActivityByPage(_User.Id, page, 10);

                foreach (var item in orderActEntity)
                {
                    OrderActivitiesModels oModel = new OrderActivitiesModels();
                    OrderActivitiesModels.ToModel(item, ref oModel);

                    // get user for this activity
                    var _u = _user_srv.GetById(oModel.UpdateByUserID);
                    if (_u != null && _u.CompanyLogo != "")
                        oModel.UserImage = _u.CompanyLogo;
                    else
                        oModel.UserImage = "default_icon.png";

                    orderActModel.Add(oModel);
                }
                ViewBag.CompanyLogo = _User.CompanyLogo;
                return PartialView("_ListOrderActivities", orderActModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logon()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = encrypt.Encrypt(model.Password, "Click4Restaurants", true);
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    var _ucon = userSv.GetUserByUserName(model.UserName);
                    if (_ucon == null)
                    {
                        _ucon = new User();
                        _ucon.Status = false;
                    }
                    if (_ucon.Status == true)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your Account is not active !!!");
                        ViewBag.NotActiveErr = "Your Account is not active !!! Please check your email, and click on confirmation link to active your account!";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    ViewBag.NotActiveErr = "The user name or password provided is incorrect.";
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }

        [CaptchaValidator]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(RegisterModel model, HttpPostedFileBase FileUp, bool captchaValid)
        {
            if (String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.Email))
            {
                ViewBag.Message = "Error : Please enter all required fields";
                return View();
            }
            if (!IsValidEmailAddress(model.Email))
            {
                ViewBag.Message = "Error : Email is not valid.";
                ViewBag.EmailError = "Email is not valid.";
                return View();
            }
            // check email exist or username exist
            var temp = userSv.GetUserByEmail(model.Email);
            if (temp != null && temp.Id > 0)
            {
                ViewBag.Message = "Error : There is an user with same email as you entered. Please use difference  email";
                ViewBag.EmailError = "Email exists.";
                return View();
            }
            temp = userSv.GetUserByUserName(model.UserName);
            if (temp != null && temp.Id > 0)
            {
                ViewBag.Message = "Error : There is an user with same username as you entered. Please use difference username";
                ViewBag.EmailError = "Username exists.";
                return View();
            }

            //
            string password = model.Password;
            model.Password = encrypt.Encrypt(model.Password, "Click4Restaurants", true);
            var userEmail = userSv.GetUserByEmail(model.Email);

            if (userEmail != null)
            {
                ViewBag.Message = "Error : Email you enter is already used. Please select difference email.";
                return View();
            }

            if (password != model.ConfirmPassword)
            {
                ViewBag.Message = "Error : Please enter same password and confirmation password fields";
                return View();
            }
            var userUserName = userSv.GetUserByUserName(model.Email);
            //var userUserName = userSv.GetUserByUserName(model.UserName);
            if (userUserName != null)
            {
                ViewBag.Message = "Error : Username you enter is already used. Please select difference username.";
                return View();
            }

            if (!captchaValid)
            {
                ViewBag.Message = "Error: Your captcha is not match.";
                return View();
            }

            string EnUser = encrypt.GetMD5HashData(model.Email);
            User u = new User();
            u.DateCreate = DateTime.Now;
            u.DateLogin = DateTime.Now;
            u.DateUpdate = DateTime.Now;
            u.Email = model.Email;
            u.Password = model.Password;
            u.UserName = model.UserName;
            u.FullName = model.FullName;
            u.Status = false;
            u.CompanyName = model.CompanyName;
            u.Address = model.Address;
            u.Phone = model.Phone;
            u.CodeActive = EnUser;
            u.CompanyLogo = UploadFile(model.CompanyName, FileUp);
            var us = userSv.CreateOrUpdate(u);

            if (us != 0)
            {
                UserInRole uinEnri = new UserInRole();
                uinEnri.Id = 0;
                uinEnri.RolesId = 3;
                uinEnri.UserId = us;
                UsInRlSv.CreateOrUpdate(uinEnri);
                string host = Request.Url.Host;

                string subject = "Thanks " + u.FullName + " for registering at Real Estate Media.";
                string body = "Hi " + u.FullName + "<br>Thank you for registering at Real Estate Media. <br>";
                body += "Please click this link to active your account : ";
                body += "<a href=\"" + host + "/Home/ConfirmRegister?code=" + EnUser + "\" target=\"_blank\">Confirmation link</a>";
                body += "<br/> If you can not click the link above, please copy the url <a>" + host + "/Home/ConfirmRegister?code=" + EnUser + "</a> and paste into your browser URL bar.";
                body += "<br/><br/> Regards,<br/>Real Estate Media";

                SendEmail.SendMail(model.Email, subject, body);

                // send email to Administrator
                subject = "New User Registration";
                body = "Hi Administrator<br><br>On " + DateTime.Now.ToString() + ", Real Estate Media received new registration from " + u.FullName + "(" + u.Email + ") <br>";
                //http://remsigns.com/Administration/UserManagement/Edit/27
                body += "You can review this user account by click on this link : ";
                body += "<a href=\"" + host + "/Administration/UserManagement/Edit/" + us.ToString() + "\" target=\"_blank\">Review " + u.FullName + "</a>";

                body += "<br/><br/> Regards,<br/>Real Estate Media";

                SendEmail.SendMail("", subject, body);
            }
            ViewBag.Message = "Your Account has been created! Please check your email to active account. Please make sure to check your spam folder.";
            return View("Message");
            //return RedirectToAction("Index");
        }

        public ActionResult ConfirmRegister(string code)
        {
            //string username = encrypt.Decrypt(code, "manhlan3013", true);
            var _passsas = userSv.GetUserByEnCodeActive(code);
            UserModel model = new UserModel();
            UserModel.ToModel(_passsas, ref model);
            if (_passsas != null)
            {
                _passsas.Status = true;
                userSv.CreateOrUpdate(_passsas);
                /////for Mailling List               
                var _maillist = _mail_srv.GetByName(_passsas.Email);
                if (_maillist == null)
                {
                    MailListModel mailmodel = new MailListModel();
                    ListMail mailentity = new ListMail();
                    mailmodel.Id = 0;
                    mailmodel.DateTime = DateTime.Now;
                    mailmodel.Block = true;
                    mailmodel.Email = _passsas.Email;
                    mailmodel.Num = 0;
                    MailListModel.ToEntity(mailmodel, ref mailentity);
                    _mail_srv.CreateOrUpdate(mailentity);
                }

                ///// End

                string password = encrypt.Decrypt(_passsas.Password, "Click4Restaurants", true);
                string subject = "Thank you for registering at Real Estate Media. Your Account has been actived";
                string body = "Hi<br>Thank you for registering at Real Estate Media.<br> We would like to confirm your account information with you.<br>Account Information : <br><br> Username: " + _passsas.UserName + "<br>Password : " + password + "<br>Company Name : " + _passsas.CompanyName + "<br>Full Name : " + _passsas.FullName + "<br>Phone : " + _passsas.Phone + "<br>Address : " + _passsas.Address;
                body += "<br/><br/> Regards,<br/>Real Estate Media";
                SendEmail.SendMail(_passsas.Email, subject, body);
                return View("SuccessfulActive", model);
                //return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePass(ChangePasswordModel model)
        {
            bool _flag = true;
            if (String.IsNullOrEmpty(model.OldPassword) || String.IsNullOrEmpty(model.NewPassword) || String.IsNullOrEmpty(model.ConfirmPassword))
            {
                ViewBag.Message = "Error : Please enter required field";
                _flag = false;
                return View();
            }
            if (model.NewPassword.Length < 6)
            {
                ViewBag.Message = "Error : The new password must be at least 6 characters long.";
                _flag = false;
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.Message = "Error : The new password and confirmation password do not match";
                _flag = false;
            }
            string username = HttpContext.User.Identity.Name;
            string _pp = encrypt.Encrypt(model.OldPassword, "Click4Restaurants", true);
            var _UserN = userSv.GetUserByUserEnCodePass(username, _pp);
            if (_UserN == null)
            {
                ViewBag.Message = "Error : Your input password is not same with your password";
                _flag = false;
            }
            if (_flag == true)
            {
                string oldpass = encrypt.Encrypt(model.OldPassword, "Click4Restaurants", true);
                string newpass = encrypt.Encrypt(model.NewPassword, "Click4Restaurants", true);
                userSv.ChangePassword(username, oldpass, newpass);
                return View("SuccessfulChangePass");
            }
            else
            {
                return View();
            }

        }

        public ActionResult ProfileAccount()
        {
            if (User.Identity.IsAuthenticated)
            {
                string username = HttpContext.User.Identity.Name;
                var entityUser = userSv.GetUserByUserName(username);
                UserModel model = new UserModel();
                UserModel.ToModel(entityUser, ref model);
                return View(model);
            }
            else
            {
                return RedirectToAction("LogOn");
            }
        }

        public ActionResult EditProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                UserModel model = new UserModel();
                var enti = userSv.GetUserByUserName(HttpContext.User.Identity.Name);
                UserModel.ToModel(enti, ref model);
                return View(model);
            }
            else
            {
                return RedirectToAction("LogOn");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(UserModel model, HttpPostedFileBase FileUp)
        {
            if (User.Identity.IsAuthenticated)
            {
                //var entity = userSv.GetById(model.Id);
                var entity = userSv.GetUserByUserName(User.Identity.Name);
                entity.FullName = model.FullName;
                entity.CompanyName = model.CompanyName;
                entity.Address = model.Address;
                entity.Phone = model.Phone;
                if (string.IsNullOrEmpty(model.CompanyName))
                {
                    model.CompanyName = "Default";
                }
                entity.Description = model.Description;
                if (FileUp != null)
                {
                    entity.CompanyLogo = UploadFile(model.CompanyName, FileUp);
                }
                userSv.CreateOrUpdate(entity);
                return RedirectToAction("ProfileAccount");
            }
            else
            {
                return RedirectToAction("LogOn");
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(UserModel model)
        {
            if (model.Email == null)
            {
                ViewBag.ErrRequired = "Error : Please enter required field";
                return View();
            }
            var _Use = userSv.GetUserByUserName(model.Email);
            if (_Use == null)
            {
                ViewBag.ErrRequired = "Error : Email you entered is not exist";
                return View();
            }
            string code = model.Email + _Use.Password;
            string host = Request.Url.Host;
            string EnUser = encrypt.GetMD5HashData(code);
            string subject = "Real Estate Media - Forgot password request.";
            string body = "Hi <br>We received request to get new passowrd for account which is linked with your email If you did not ask for new password, please forget this email.<br>";
            body += "Click the link below to get new password for your account !<br />";
            body += "<a href=\"" + host + "/Home/GetNewPass?name=" + model.Email + "&code=" + EnUser + "\" target=\"_blank\">Generate new password for me</a>";
            body += "<br/> If you can not open the link above, please copy the url and paste into your browser: <br /><a>" + host + "/Home/GetNewPass?name=" + model.Email + "&code=" + EnUser + "</a>";
            body += "<br/><br/> Thanks,<br/>Real Estate Media";
            SendEmail.SendMail(model.Email, subject, body);
            return View("MessageCheckMailForgotPass");
        }

        public ActionResult GetNewPass(string name, string code)
        {
            var _passsas = userSv.GetUserByUserName(name);
            if (_passsas != null)
            {
                string newcode = encrypt.GetMD5HashData(name + _passsas.Password);
                if (code == newcode)
                {
                    UserModel model = new UserModel();
                    UserModel.ToModel(_passsas, ref model);

                    return View("ChangeForgotPass", model);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetNewPass(UserModel model)
        {
            bool _flag = true;
            if (String.IsNullOrEmpty(model.NewPassword) || String.IsNullOrEmpty(model.ConfirmPassword))
            {
                ViewBag.ErrRequired = "Error : Required field";
                return View("ChangeForgotPass");
            }
            if (model.NewPassword.Length < 6)
            {
                ViewBag.ErrNotlong = "Error : The new password must be at least 6 characters long.";
                _flag = false;
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.ErrNotmatch = "Error : The new password and confirmation password do not match";
                _flag = false;
            }
            if (_flag == true)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var _U = userSv.GetUserByUserName(User.Identity.Name);
                    string username = _U.Email;
                    string oldpass = _U.Password;
                    string newpass = encrypt.Encrypt(model.NewPassword, "Click4Restaurants", true);
                    userSv.ChangePassword(username, oldpass, newpass);
                    return View("SuccessfulChangePass");
                }
                else
                {
                    return Content("UnAuthorized");
                }
            }
            return View("ChangeForgotPass");
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


        #region Homepage Contact Block
        public ActionResult ContactBlockHomepage()
        {
            return PartialView();
        }

        #endregion


        #region For Contact us - Later should move to contact us controller
        public ActionResult RealContactUsBody()
        {
            string _Documentalignment = _setting_srv.SettingByName("contactus_setting").Body;
            return Content(_Documentalignment);
        }
        public ActionResult RealContactUsLatLng()
        {
            string _Documentalignment = _setting_srv.SettingByName("contactus_setting").Title;
            return Content(_Documentalignment);
        }

        #endregion

        #region Estimate Page
        public ActionResult EstimatePage()
        {
            return View();
        }

        public ActionResult EstimatePage_Submit(EstimatePage model, IEnumerable<HttpPostedFileBase> FileUp)
        {
            if (CaptchaController.CheckCaptcha(model.CaptchaValue, Session["CaptchaHash"].ToString()))
            {
                Session["CaptchaHash"] = DateTime.Now.Ticks.ToString();
            }
            else
            {
                return Content("You enterted wrong captcha");
            }

            // file upload process
            string file_upload_path = "";
            string host = Request.Url.Host;

            if (FileUp != null)
            {
                foreach (var file in FileUp)
                {
                    if (file.ContentLength > 0)
                    {
                        var dir = Server.MapPath("~/Content/File/Estimate/");
                        var path = "";

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        var ext = Path.GetExtension(file.FileName).ToLower();
                        if (ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".doc" || ext == ".zip" || ext == ".rar" || ext == ".pdf" || ext == ".docx" || ext == ".ppt" || ext == ".pptx")
                        {
                            try
                            {

                                dir = Server.MapPath("~/Content/File/Estimate");
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                var filename = Path.GetFileNameWithoutExtension(file.FileName);
                                filename = EscapeName.Renamefile(filename);
                                string tmp = Path.GetRandomFileName().Substring(0, 3);
                                var filenamefinal = DateTime.Now.Ticks.ToString() + "-" + filename + "-" + tmp + ext;
                                path = Path.Combine(Server.MapPath("~/Content/File/Estimate"), filenamefinal);
                                var db_path = "Content/File/Estimate/" + filenamefinal;
                                file.SaveAs(path);
                                file_upload_path += string.Format("<a href='{0}/{1}'>{0}/{1}</a><br>", host, db_path);
                            }
                            catch
                            {
                            }
                            //
                        }
                    }
                }
            }

            string subject = "Real Estate Media Contact System";
            string body = "";
            body += "Hi Brian";

            body += "<br/><br/>On " + DateTime.Now + ", You have new estimation request from visitor.";

            body += "<br/>Here is the information:";
            body += "<br/><br/>Name: " + model.Name;
            body += "<br/>Email: " + model.Email;
            body += "<br/>Phone: " + model.Phone;
            body += "<br/>Company Name: " + model.CompanyName;
            body += "<br/><br/>Message:" + model.Comment;
            if (file_upload_path != "")
            {
                body += "<br/><br/>Attached files:" + file_upload_path;
            }

            body += "<br/><br/>Please dont reply this email.";
            body += "<br/>Regards";
            SendEmail.SendMail("", subject, body);

            body = "";
            body += "Hi " + model.Name;

            body += "<br/>Thanks for your sign estimation request on " + DateTime.Now;

            body += "<br/>Here is the confirmation of message you submited on our website Real Estate Media:";
            body += "<br/><br/>Name: " + model.Name;
            body += "<br/>Email: " + model.Email;
            body += "<br/>Phone: " + model.Phone;
            body += "<br/>Company Name: " + model.CompanyName;
            body += "<br/><br/>Message:" + model.Comment;
            if (file_upload_path != "")
            {
                body += "<br/><br/>Attached files:" + file_upload_path;
            }

            body += "<br/><br/>We will check your message and contact you very soon.";
            body += "<br/>Regards";
            body += "<br/>Real Estate Media";
            SendEmail.SendMail(model.Email, subject, body);
            return View();
        }

        #endregion

    }
}
