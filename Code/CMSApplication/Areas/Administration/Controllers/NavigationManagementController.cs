using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMS.Common.Mvc;
using CMS.Data;
using CMS.Service.Repository;
using CMSApplication.Areas.Administration.Models;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class NavigationManagementController : UTController
    {
        //
        // GET: /Administration/NavigationManagement/

        private NavigationRepository _Navigation_srv = new NavigationRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListNavigation()
        {
            List<NavigationModel> model = new List<NavigationModel>();
            var a = _Navigation_srv.List().OrderBy(m => m.OrderMenu);
            foreach (var item in a)
            {
                NavigationModel ml = new NavigationModel();
                NavigationModel.Mapfrom(item, ref  ml);
                model.Add(ml);
            }
            return PartialView("_ListNavigation", model);
        }

        //private List<MenuList> GetNavigationStructure()
        //{
        //    // {ID, Name, ParentID}
        //    List<MenuList> Navigation = new List<MenuList>();

        //    //Get all Navigation
        //    IEnumerable<Navigation> a = _Navigation_srv.List().Where(m => m.ParentID > 0); // nho : active =1
        //    var b = _Navigation_srv.List().Where(p => p.ParentID == 0);
        //    foreach (var item in b)
        //    {
        //        MenuList ml = new MenuList();
        //        MenuList.Mapfrom(item, ref  ml);
        //        Navigation.Add(ml);
        //    }
        //    int n = a.Count();
        //    while (n > 0)
        //    {
        //        int index = 0;
        //        while (index < Navigation.Count())
        //        {
        //            // get list of sub Navigation of Navigation[index]
        //            MenuList temp = Navigation[index];

        //            var x = a.Where(m => m.ParentID == temp.ID).OrderBy(m => m.OrderMenu);

        //            // add list of x into Navigation
        //            if (x.Count() > 0)
        //            {
        //                int index2 = index;
        //                foreach (var t in x)
        //                {
        //                    MenuList temp2 = new MenuList();
        //                    temp2.ID = t.Id;
        //                    temp2.Name = t.Name;
        //                    temp2.ParentID = t.ParentID;
        //                    Navigation.Insert(index2, temp2);
        //                    index2++;
        //                }
        //            }
        //            index++;
        //            n--;
        //        }
        //        break;
        //    }
        //    return Navigation;
        //}

        ////ajax
        //public ActionResult AllNavigation(int selectedId)
        //{
        //    var Navigations = _Navigation_srv.List().Where( k => k.Active).ToList();
        //    Navigations.Insert(0, new Navigation { Name = "[No parent]", Id = 0 });
        //    var selectList = new List<SelectListItem>();
        //    foreach (var c in Navigations)
        //        selectList.Add(new SelectListItem()
        //        {
        //            Value = c.Id.ToString(),
        //            Text = _Navigation_srv.GetCategoryBreadCrumb(c,_Navigation_srv),
        //            Selected = c.Id == selectedId
        //        });

        //    //var selectList = new SelectList(categories, "Id", "Name", selectedId);
        //    return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        public ActionResult Add()
        {
            var Navigations = _Navigation_srv.List().Where(k => k.Active).ToList();
            Navigations.Insert(0, new Navigation { Name = "[No parent]", Id = 0 });
            var selectList = new List<SelectListItem>();
            foreach (var c in Navigations)
                selectList.Add(new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = _Navigation_srv.GetCategoryBreadCrumb(c, _Navigation_srv),
                });
            ViewData["ParentId"] = new SelectList(selectList, "Value", "Text");
            NavigationModel model = new NavigationModel();
            int OrderMenu = 0;
            if (_Navigation_srv.List().Count==0 || _Navigation_srv.List().Max(m => m.OrderMenu) == 0)
            {
                OrderMenu = 1;
            }
            else
            {
                OrderMenu = _Navigation_srv.List().Max(m => m.Id) + 1;
            }
            model.OrderMenu = OrderMenu;
            model.ParentId = 0;
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdate(NavigationModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return JsonError("Please enter menu name.");
            }
            Navigation entity = new Navigation();
            NavigationModel.Mapfrom(model, ref entity);
            _Navigation_srv.CreateOrUpdate(entity);
            return JsonSuccess(Url.Action("Index"));
        }

        public ActionResult Edit(int id)
        {
            int selectid = _Navigation_srv.GetById(id).ParentID;
            var Navigations = _Navigation_srv.List().Where(k => k.Active && k.Id != id).ToList();
            Navigations.Insert(0, new Navigation { Name = "[No parent]", Id = 0 });
            var selectList = new List<SelectListItem>();
            foreach (var c in Navigations)
                selectList.Add(new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = _Navigation_srv.GetCategoryBreadCrumb(c, _Navigation_srv),
                });
            ViewData["ParentId"] = new SelectList(selectList, "Value", "Text", selectid);

            NavigationModel model = new NavigationModel();
            var entity = _Navigation_srv.GetById(id);
            NavigationModel.Mapfrom(entity, ref model);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            _Navigation_srv.Delete(id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Move(int id, int direction)
        {
            try
            {
                Navigation entity = _Navigation_srv.GetById(id);
                IEnumerable<Navigation> a = new List<Navigation>();
                Navigation temp = new Navigation();

                // get the nearest
                if (direction == 1) // down
                {
                    a = _Navigation_srv.List().OrderBy(m => m.OrderMenu).Where(m => m.OrderMenu < entity.OrderMenu);
                    if (a.Count() > 0)
                        temp = a.Last();
                }
                else
                {
                    a = _Navigation_srv.List().OrderBy(m => m.OrderMenu).Where(m => m.OrderMenu > entity.OrderMenu);
                    if (a.Count() > 0)
                        temp = a.First();
                }

                if (temp.Id > 0)
                {
                    int t = temp.OrderMenu;
                    temp.OrderMenu = entity.OrderMenu;
                    entity.OrderMenu = t;
                    _Navigation_srv.CreateOrUpdate(entity);
                    _Navigation_srv.CreateOrUpdate(temp);
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }
        public ActionResult MapNavigation()
        {
            var a = _Navigation_srv.List().OrderBy(m => m.OrderMenu).Where(m => m.Active).ToList();
            return PartialView(a);
        }
    }
}