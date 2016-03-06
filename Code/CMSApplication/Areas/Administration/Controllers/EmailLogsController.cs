using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Common.Mvc;
using CMS.Data;
using CMSApplication.Areas.Administration.Models;
using CMS.Service.Repository;
using CMS.Common.Helpers;
using System.IO;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class EmailLogsController : UTController
    {
        //
        // GET: /Administration/Slide/
        EmailLogRepository _email_log_repo = new EmailLogRepository();

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult _List()
        {
            var model = _email_log_repo.List().OrderByDescending(m => m.Id).Take(300).ToList();
            return PartialView("_List",model);
        }

    }
}
