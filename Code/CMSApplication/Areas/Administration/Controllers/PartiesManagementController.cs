using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;
using CMS.Common.Mvc;
using CMS.Data;
using CMS.Common.Helpers;
using CMS.Service.Common;
using System.Configuration;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class PartiesManagementController : UTController
    {
        //
        // GET: /Administration/PartiesManagement/
        //private PartiesRepository par_srv = new PartiesRepository();
        private UserRepositry _user_srv = new UserRepositry();
        private OrderRepository order_srv = new OrderRepository();

        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Administrator,Manager")]
        //public ActionResult List(int page)
        //{
        //    List<PartiesModels> model = new List<PartiesModels>();
        //    var c = par_srv.List();
        //    foreach (var item in c)
        //    {
        //        PartiesModels ml = new PartiesModels();
        //        PartiesModels.ToModel(item, ref  ml);

        //        ///// For UserName Name
        //        string Name = _user_srv.GetById(ml.UserId).UserName;
        //        if (Name != null)
        //        {
        //           ml.Name = Name;
        //        }
        //        else
        //        {
        //            ml.Name = "";
        //        }
        //        ///// For Order Name
        //        string OrderName = order_srv.GetById(ml.OrderId).Title;
        //        if (OrderName != null)
        //        {
        //           ml.OrderName = OrderName;
        //        }
        //        else
        //        {
        //           ml.OrderName = "";
        //        }                
        //        ///////// Add item to Model
        //        model.Add(ml);
        //    }

        //    var ListNewsPageSize = new PagedData<PartiesModels>();

        //    if (model.Count() > 0)
        //    {
        //        ListNewsPageSize.Data = model.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
        //        if (ListNewsPageSize.Data.Count() == 0)
        //        {
        //            ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
        //        }
        //        ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / PageSize));
        //        ListNewsPageSize.CurrentPage = page;
        //    }
        //    else
        //    {
        //        ListNewsPageSize.Data = new List<PartiesModels>();
        //        ListNewsPageSize.NumberOfPages = 0;
        //        ListNewsPageSize.CurrentPage = 0;
        //    }

        //    return PartialView("_List", ListNewsPageSize);
        //}

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add()
        {
            PartiesModels model = new PartiesModels();
            model.Id = 0;
            /////// Dropdown list user
            var ListUserParties = _user_srv.List();
            ViewData["UserNameID"] = new SelectList(ListUserParties, "Id", "UserName");
            /////

            /////// Dropdown list order
            var ListOrderParties = order_srv.List();
            ViewData["OrderNameID"] = new SelectList(ListOrderParties, "Id", "Title");
            /////

            return View(model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            PartiesModels model = new PartiesModels();
            var entity = par_srv.GetById(id);
            PartiesModels.ToModel(entity, ref model);
            /////// Dropdown list user
            var ListUserParties = _user_srv.List();
            ViewData["UserNameID"] = new SelectList(ListUserParties, "Id", "UserName");
            /////

            /////// Dropdown list order
            var ListOrderParties = order_srv.List();
            ViewData["OrderNameID"] = new SelectList(ListOrderParties, "Id", "Title");
            /////

            return View(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdate(PartiesModels model)
        {
            if (model.Id == 0)
            {
                //// Add
                model.DateAssign = DateTime.Now;
                string Username = HttpContext.User.Identity.Name;
                if (Username != null)
                {
                    var _User = _user_srv.GetUserByUserName(Username);
                    if (_User != null)
                    {
                        model.AssignById = _User.Id;
                        model.AssignByName = _User.UserName;
                    }
                    else { return RedirectToAction("LogOn", "Management"); }
                }
                else { return RedirectToAction("LogOn", "Management"); }
            }
            Party entity = new Party();
            PartiesModels.ToEntity(model, ref entity);
            par_srv.CreateOrUpdate(entity);

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int Id)
        {
            par_srv.Delete(Id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}
