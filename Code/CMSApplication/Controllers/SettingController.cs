using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Models;
using CMS.Common.Mvc;
using CMS.Service.Common;

namespace CMSApplication.Controllers
{
    public class FESettingController : UTController
    {
        //
        // GET: /Administration/Setting/

        private SettingRepository _setting_srv = new SettingRepository();
        public ActionResult Index()
        {
            return View();
        }
        #region      Setting

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

        #endregion

    }
}
