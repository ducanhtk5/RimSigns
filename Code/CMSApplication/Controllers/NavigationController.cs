using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Service.Repository;
using CMS.Data;

namespace CMSApplication.Controllers
{
    public class NavigationController : Controller
    {
        //
        // GET: /Menu/
        NavigationRepository _m_srv = new NavigationRepository();
        public ActionResult Index()
        {
            string _absoft = Request.Url.Host;
            if (0<1)
            {
                try
                {
                    var menu = _m_srv.List().Where(x => x.Active).OrderBy(x => x.OrderMenu).ToList();
                    return PartialView(menu);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

        public ActionResult FooterMenu()
        {
            string _absoft = Request.Url.Host;
            if (0 < 1)
            {
                try
                {
                    var menu = _m_srv.List().Where(x => x.Active).OrderBy(x => x.OrderMenu).ToList();
                    return PartialView("_FooterMenu",menu);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }

    }
}
