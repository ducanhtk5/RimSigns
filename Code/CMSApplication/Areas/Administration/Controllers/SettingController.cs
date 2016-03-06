using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;
using CMS.Common.Mvc;
using CMS.Service.Common;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class SettingController : UTController
    {
        //
        // GET: /Administration/Setting/

        private SettingRepository _setting_srv = new SettingRepository();
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View();
        }
        #region      Setting
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ListSetting()
        {
            List<SettingModel> model = new List<SettingModel>();
            var a = _setting_srv.List();
            foreach (var item in a)
            {
                SettingModel ml = new SettingModel();
                SettingModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }
            return PartialView("_ListSetting", model);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddSetting()
        {
            SettingModel model = new SettingModel();
            model.Id = 0;
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdateSetting(SettingModel model)
        {
            if (string.IsNullOrEmpty(model.SettingName))
            {
                return JsonError("Please enter setting name.");
            }
            Setting entity = new Setting();
            SettingModel.Mapfrom(model, ref entity);
            _setting_srv.CreateOrUpdate(entity);
            return JsonSuccess(Url.Action("Index"));
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult EditSetting(int id)
        {
            SettingModel model = new SettingModel();
            var entity = _setting_srv.GetById(id);
            SettingModel.Mapfrom(entity, ref model);
            return View(model);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteSetting(int id)
        {
            _setting_srv.Delete(id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SettingByName(string SettingName)
        {
            SettingModel model = new SettingModel();
            try
            {
                var entity = _setting_srv.SettingByName(SettingName);
                SettingModel.Mapfrom(entity, ref model);
            }
            catch (Exception)
            {

                return PartialView("_SettingByName", model);
            }

            return PartialView("_SettingByName", model);
        }
        public ActionResult EmailSetting()
        {
            string email_account = ConfigSettings.ReadSetting("Email");
            string emailrecieve = ConfigSettings.ReadSetting("EmailRecive");
            string password_account = ConfigSettings.ReadSetting("Password");
            string host = ConfigSettings.ReadSetting("Host");
            int port = int.Parse(ConfigSettings.ReadSetting("Port"));
            bool enablessl = Convert.ToBoolean(ConfigSettings.ReadSetting("EnableSSL"));
            string DisplayName = ConfigSettings.ReadSetting("DisplayName");
            EmailSetting model = new EmailSetting();
            model.Email = email_account;
            model.Password = password_account;
            model.Host = host;
            model.Port = port;
            model.EnableSSL = enablessl;
            model.EmailRecieve = emailrecieve;
            model.DisplayName = DisplayName;
            return View(model);
        }
        [HttpPost]
        public ActionResult EmailSetting(EmailSetting model)
        {
            string email_account = "Email";
            string emailrecieve = "EmailRecive";
            string password_account = "Password";
            string host = "Host";
            string port = "Port";
            string enablessl = "EnableSSL";
            string displayname = "DisplayName";
            ConfigSettings.UpdateSetting(email_account, model.Email);
            ConfigSettings.UpdateSetting(emailrecieve, model.EmailRecieve);
            ConfigSettings.UpdateSetting(password_account, model.Password);
            ConfigSettings.UpdateSetting(host, model.Host);
            ConfigSettings.UpdateSetting(port, model.Port.ToString());
            ConfigSettings.UpdateSetting(enablessl, model.EnableSSL.ToString());
            ConfigSettings.UpdateSetting(displayname, model.DisplayName);
            return View(model);
        }
        #endregion

    }
}
