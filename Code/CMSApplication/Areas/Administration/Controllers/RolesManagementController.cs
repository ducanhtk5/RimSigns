using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMS.Common.Mvc;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class RolesManagementController : UTController
    {
        private RolesRepository _role_srv = new RolesRepository();
        private UserInRolesRepository _UserRoles = new UserInRolesRepository();

        //
        // GET: /RolesManager/
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult List()
        {
            List<RolesModel> model = new List<RolesModel>();
            var c = _role_srv.List();
            foreach (var item in c)
            {
                RolesModel ml = new RolesModel();
                RolesModel.MapFrom(item, ref  ml);
                model.Add(ml);
            }

            return PartialView("_List", model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add()
        {
            RolesModel model = new RolesModel();
            model.Active = true;
            model.Id = 0;
            model.checkAddOrUdate = false;
            model.Active = true;
            model.DateCreate = DateTime.Now;
            model.Roles_persmission = 7;
            return View(model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            RolesModel model = new RolesModel();
            var entity = _role_srv.GetById(id);
            RolesModel.MapFrom(entity, ref model);
            model.checkAddOrUdate = true;
            model.systemnameOld = model.Name;
            return View( model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult AddOrUpdate(RolesModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return JsonError(" Please enter roles name");
            }

            if (model.checkAddOrUdate)
            {
                if (model.systemnameOld != model.Name)
                {
                    var checkExitsSystemName = _role_srv.checkRolesExits(model.Name);
                    if (checkExitsSystemName)
                    {
                        return JsonError("Roles name exist.Please enter orther name.");
                    }
                }
            }
            else
            {
                var checkExitsSystemName = _role_srv.checkRolesExits(model.Name);
                if (checkExitsSystemName)
                {
                    return JsonError("Roles name exist.Please enter orther name.");
                }
            }

            Role entity = new Role();
            RolesModel.MapFrom(model, ref entity);

            _role_srv.CreateOrUpdate(entity);

            return JsonSuccess(Url.Action("Index"));
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int id)
        {
            _UserRoles.DeleteByRolesID(id);
            _role_srv.Delete(id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}