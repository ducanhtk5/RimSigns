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
    public class TopicManagementController : UTController
    {
        private TopicRepository TopicSv = new TopicRepository();
        //
        // GET: /Topic/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult List(int page)
        {
            List<TopicModel> model = new List<TopicModel>();
            var c = TopicSv.List();
            foreach (var item in c)
            {
                TopicModel ml = new TopicModel();
                TopicModel.MapFrom(item, ref  ml);
                model.Add(ml);
            }

            var ListNewsPageSize = new PagedData<TopicModel>();

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
                ListNewsPageSize.Data = new List<TopicModel>();
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
            }

            return PartialView("_List", ListNewsPageSize);
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add()
        {
            TopicModel model = new TopicModel();
            model.Id = 0;
            return View( model);
        }

         [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            TopicModel model = new TopicModel();
            var entity = TopicSv.GetById(id);
            TopicModel.MapFrom(entity, ref model);
            return View( model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdate(TopicModel model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                return JsonError("Please enter topic name");
            }
            if (string.IsNullOrEmpty(model.SystemName))
            {
                return JsonError("Please enter topic system name");
            }
            if (string.IsNullOrEmpty(model.Body))
            {
                return JsonError("Please enter topic content");
            }

            Topic entity = new Topic();
            TopicModel.MapFrom(model, ref entity);
            TopicSv.CreateOrUpdate(entity);

            return JsonSuccess(Url.Action("Index"));
        }
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int Id)
        {
            TopicSv.Delete(Id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopicBlock(string SystemName)
        {
            TopicModel model = new TopicModel();
            try
            {
                var entity = TopicSv.getTopiccByName(SystemName);
                TopicModel.MapFrom(entity, ref model);
            }
            catch (Exception)
            {

                return PartialView();
            }

            return PartialView(model);
        }
        public ActionResult ListTopic()
        {
            List<TopicModel> model = new List<TopicModel>();
            var a = TopicSv.List();
            foreach (var item in a)
            {
                TopicModel ml = new TopicModel();
                TopicModel.MapFrom(item, ref  ml);
                model.Add(ml);
            }
            return PartialView("_ListTopic", model);
        }
    }
}
