using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Data;
using CMS.Service;
using CMS.Service.Repository;
using CMSApplication.Models;
using CMS.Common.Helpers;
using CMS.Common.Mvc;
using System.IO;
using CMS.Service.Common;
using CMS.Service.Enum;

namespace CMSApplication.Controllers
{
    public class OrderController : UTController
    {
        //
        // GET: /Order/
        private OrderRepository _order_srv = new OrderRepository();
        private UserRepositry _user_srv = new UserRepositry();
        private FileUploadRepository file_srv = new FileUploadRepository();
        private CommentRepository _cmm_srv = new CommentRepository();
        private OrderActivitiesRepository _orderAct_services = new OrderActivitiesRepository();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Render a partical view into string
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult Details(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                OrderModels model = new OrderModels();
                var order = _order_srv.GetById(id);
                if (order == null)
                {
                    ViewBag.Message = "This order is not exist.";
                    return View("Error");
                }
                OrderModels.ToModel(order, ref model);

                List<CommentAndFileUploadModel> cmmFileModelList = new List<CommentAndFileUploadModel>();

                foreach (var item in _cmm_srv.GetTableListById(id))
                {
                    CommentAndFileUploadModel cmmFileModel = new CommentAndFileUploadModel();
                    cmmFileModel.CreatedDate = (DateTime)item.CreatedTime;
                    cmmFileModel.UserName = item.UserName;
                    cmmFileModel.UserID = (int)item.UserId;
                    cmmFileModel.Contents = item.Contents;
                    cmmFileModel.Id = item.Id;
                    cmmFileModel.FileUrl = "";
                    cmmFileModel.UserImage = _user_srv.GetById((int)item.UserId).CompanyLogo;
                    if (cmmFileModel.UserImage == "")
                        cmmFileModel.UserImage = "default_icon.png";
                    cmmFileModelList.Add(cmmFileModel);
                }

                foreach (var item in file_srv.GetTableListById(id))
                {
                    CommentAndFileUploadModel cmmFileModel = new CommentAndFileUploadModel();
                    cmmFileModel.CreatedDate = (DateTime)item.DateUpload;
                    cmmFileModel.UserName = item.UploadByName;
                    cmmFileModel.UserID = (int)item.UploadById;
                    cmmFileModel.Contents = item.FileName;
                    cmmFileModel.FileUrl = item.FileLink;
                    cmmFileModel.UserImage = _user_srv.GetById((int)item.UploadById).CompanyLogo;
                    cmmFileModel.Id = item.Id;
                    cmmFileModel.IsImage = (bool)item.IsImage;
                    cmmFileModel.Note = item.Note;
                    if (cmmFileModel.UserImage == "")
                        cmmFileModel.UserImage = "default_icon.png";
                    cmmFileModelList.Add(cmmFileModel);
                }

                ViewBag.CommentFileList = cmmFileModelList.OrderByDescending(c => c.CreatedDate);

                model.GetDataForDropdownList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        public ActionResult List(int page)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<OrderModels> model = new List<OrderModels>();
                var u = _user_srv.GetUserByUserName(User.Identity.Name);
                var c = _order_srv.GetTableListById(u.Id).OrderByDescending(m=>m.OrderDate);
                foreach (var item in c)
                {
                    OrderModels ml = new OrderModels();
                    OrderModels.ToModel(item, ref  ml);
                    model.Add(ml);
                }

                var ListNewsPageSize = new PagedData<OrderModels>();

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
                    ListNewsPageSize.Data = new List<OrderModels>();
                    ListNewsPageSize.NumberOfPages = 0;
                    ListNewsPageSize.CurrentPage = 0;
                }

                return PartialView("_ListOrder", ListNewsPageSize);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Add()
        {
            if (User.Identity.IsAuthenticated)
            {

                OrderModels model = new OrderModels();
                model.Id = 0;
                model.GetDataForDropdownList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            if (User.Identity.IsAuthenticated)
            {

                OrderModels model = new OrderModels();
                var entity = _order_srv.GetById(id);
                OrderModels.ToModel(entity, ref model);
                model.GetDataForDropdownList();

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [ValidateInput(false)]
        public ActionResult AddOrUpdate(OrderModels model, IEnumerable<HttpPostedFileBase> FileUp)
        {
            if (User.Identity.IsAuthenticated)
            {
                // validation
                if (string.IsNullOrEmpty(model.Title))
                {
                    //return Redirect(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
                    ViewBag.Message = "Error : Project name is empty";
                    if (model.Id == 0)
                    {
                        // add new 
                        return View("Add", model);
                    }
                    else
                    {
                        // edit
                        return View("Edit", model);
                    }
                }

                if (string.IsNullOrEmpty(model.Address))
                {
                    //return Redirect(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
                    ViewBag.Message = "Error : Address is empty";
                    if (model.Id == 0)
                    {
                        // add new 
                        return View("Add", model);
                    }
                    else
                    {
                        // edit
                        return View("Edit", model);
                    }
                }

                // check permission
                var temp = _order_srv.GetById(model.Id);
                if (model.Id >0 && temp != null && temp.OrderById != UtContext.UserId)
                {
                    return Content("Unauthorized...");
                }
                else
                {
                    // if add, then should put ID = 0
                    model.Id = 0;
                }

                User _User = new User();
                string Username = HttpContext.User.Identity.Name;
                if (Username != null)
                {
                    _User = _user_srv.GetUserByUserName(Username);
                }
                else { return RedirectToAction("Logon", "Home"); }

                if (model.Id == 0)
                {
                    //// Add
                    model.OrderDate = DateTime.Now;

                    if (string.IsNullOrEmpty(model.Size))
                    {
                        model.Size = "";
                    }

                    if (string.IsNullOrEmpty(model.Type))
                    {
                        model.Type = "";
                    }

                    if (string.IsNullOrEmpty(model.Status))
                    {
                        model.Status = "";
                    }


                    if (_User != null)
                    {
                        model.OrderById = _User.Id;
                        model.OrderByName = _User.UserName;

                        // send mail to client to let them know
                        //*** Your order has been receive and process. Thank you ****
                        ViewData.Add("name", _User.FullName);
                        string subject = "Your order has been receive and process";
                        string body = RenderPartialViewToString("NewOrder_Confirm", model);
                        SendEmail.SendMail(_User.Email, subject, body);

                        // send admin
                        subject = "You have new order from "+_User.FullName;
                        body = RenderPartialViewToString("NewOrder_Admin", model);
                        SendEmail.SendMail("", subject, body);
                    }
                    else { return RedirectToAction("Logon", "Home"); }
                }
                else
                {
                    Order oldOrder = _order_srv.GetById(model.Id);
                    string host = System.Web.HttpContext.Current.Request.Url.Host;
                    if (oldOrder.Status.Equals(model.Status))
                    {
                        string action = "Change status from " + oldOrder.Status + " to " + model.Status + " on " + "<a href='/Order/Details/" + model.Id + "'>" + model.Title + "</a>";
                        UpdateOrderActivities(OrderActions.ChangeStatus, model, action, _User);
                    }

                }


                Order entity = new Order();
                OrderModels.ToEntity(model, ref entity);
                if (model.Id==0)
                    entity.Status = "Recieved";

                entity.StaffID = CMS.Common.Helpers.StringHelpers.ConvertListToString(model.StaffID);
                var x=_order_srv.CreateOrUpdate(entity);

                // add images if nessessary
                if (FileUp != null && FileUp.First() != null)
                {
                    model.Id = x;
                    UploadFile(model, FileUp, _User, "");
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddParties(OrderModels orderModel)
        {
            if (User.Identity.IsAuthenticated)
            {

                Order entity = new Order();
                entity = _order_srv.GetById(orderModel.Id);
                entity.Parties = orderModel.Parties;
                _order_srv.CreateOrUpdate(entity);
                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult OrderComplete(OrderModels model)
        {
            if (User.Identity.IsAuthenticated)
            {

                Order entity = new Order();
                entity = _order_srv.GetById(model.Id);
                entity.Status = model.Status;
                _order_srv.CreateOrUpdate(entity);
                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Delete(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {


                _order_srv.Delete(Id);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UploadFile(OrderModels model, IEnumerable<HttpPostedFileBase> FileUp, string fileNote)
        {
            if (User.Identity.IsAuthenticated)
            {

                User _User = new User();
                string Username = HttpContext.User.Identity.Name;
                if (Username != null)
                {
                    _User = _user_srv.GetUserByUserName(Username);
                }
                else { return RedirectToAction("Logon", "Home"); }

                //Uplpoad
                UploadFile(model, FileUp, _User, fileNote);

                //Update order activities
                string action = "Upload " + FileUp.Count() + " files on " + "<a href='" + System.Web.HttpContext.Current.Request.UrlReferrer + "'>" + model.Title + "</a>";
                UpdateOrderActivities(OrderActions.UploadFile, model, action, _User);
                return RedirectToAction("Details", new { id = model.Id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void UpdateOrderActivities(CMS.Service.Enum.OrderActions orderAction, OrderModels model, string action, User _User)
        {
            if (User.Identity.IsAuthenticated)
            {

                OrderActivitiesModels orderActModel = new OrderActivitiesModels();
                orderActModel.Id = 0;
                orderActModel.CreatedTime = DateTime.Now;
                orderActModel.Enable = 1;
                orderActModel.OrderId = model.Id;
                orderActModel.OrderUserID = model.OrderById;
                orderActModel.OrderUserName = model.OrderByName;
                orderActModel.UpdateByUserID = _User.Id;
                orderActModel.UpdateByUserName = _User.FullName;
                orderActModel.Actions = action;
                OrderActivity orderActEntity = new OrderActivity();
                OrderActivitiesModels.ToEntity(orderActModel, ref orderActEntity);
                _orderAct_services.CreateOrUpdate(orderActEntity);


                var this_user = _user_srv.GetById(_User.Id);
                var user_role = _user_srv.GetRolesForUser(this_user.UserName);
                var is_admin = user_role.Where(m => m == "Administrator").Count() > 0;
                List<string> mailList = new List<string>();

                if (!string.IsNullOrEmpty(model.Parties))
                {
                    List<string> PartiesMail = (List<string>)CMS.Common.Helpers.StringHelpers.ConvertStringToList(model.Parties);
                    mailList.AddRange(PartiesMail);
                }

                if (model.StaffID != null)
                {

                    List<string> StaffMail = model.StaffID;
                    List<string> StaffMail_edited = StaffMail;
                    /*foreach (var x in StaffMail)
                        if (is_admin && x == "")
                        {
                        }
                        else
                        {
                            StaffMail_edited.Add(x);
                        }
                    */
                    if (is_admin)
                        StaffMail_edited = StaffMail.Where(m => m != "").ToList();

                    mailList.AddRange(StaffMail_edited);
                }

                /*
                if (!string.IsNullOrEmpty(model.OrderByName))
                {
                    mailList.Add(model.OrderByName);
                }*/
                var orderby_user = _user_srv.GetById(model.OrderById);
                if (orderby_user != null)
                {
                    mailList.Add(orderby_user.Email);
                }

                Order order = new Order();
                OrderModels.ToEntity(model, ref order);
                string host = System.Web.HttpContext.Current.Request.Url.Host;
                
                // now we check the permission. if admin then no need to send email
                SendEmail.SendEmailOrderActivities(order, orderAction, mailList, host);
            }
           
        }

        public ActionResult SortByDay(DateTime day_to, DateTime day_from)
        {
            if (User.Identity.IsAuthenticated)
            {

                string Username = HttpContext.User.Identity.Name;
                var ListNewsPageSize = new PagedData<OrderModels>();
                List<Order> orderEntityList = new List<Order>();
                if (day_to > day_from)
                {
                    var rs = _order_srv.SortByDay(day_from, day_to, Username);
                    if (rs != null)
                    {
                        orderEntityList = rs;
                        ListNewsPageSize.Data = new List<OrderModels>();
                    }
                    foreach (var order in orderEntityList)
                    {
                        OrderModels orderModel = new OrderModels();
                        OrderModels.ToModel(order, ref orderModel);
                        ListNewsPageSize.Data.Add(orderModel);
                    }
                }
                return PartialView("_ListOrder", ListNewsPageSize);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult FullSearch(string keys)
        {
            if (User.Identity.IsAuthenticated)
            {


                string Username = HttpContext.User.Identity.Name;
                var ListNewsPageSize = new PagedData<OrderModels>();
                List<Order> orderEntityList = new List<Order>();
                var rs = _order_srv.FullSearch(keys, Username);
                if (rs != null)
                {
                    orderEntityList = rs;
                    ListNewsPageSize.Data = new List<OrderModels>();
                }
                foreach (var order in orderEntityList)
                {
                    OrderModels orderModel = new OrderModels();
                    OrderModels.ToModel(order, ref orderModel);
                    ListNewsPageSize.Data.Add(orderModel);
                }
                return PartialView("_ListOrder", ListNewsPageSize);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SendComment(CommentModel cmmModel)
        {
            if (User.Identity.IsAuthenticated)
            {


                User _User = new User();
                string Username = HttpContext.User.Identity.Name;
                if (Username != null)
                {
                    _User = _user_srv.GetUserByUserName(Username);
                }
                else { return RedirectToAction("Logon", "Home"); }

                Order orderEntity = _order_srv.GetById(cmmModel.OrderId);
                OrderModels orderModel = new OrderModels();
                OrderModels.ToModel(orderEntity, ref orderModel);

                if (!string.IsNullOrEmpty(cmmModel.Contents))
                {
                    var user = _user_srv.GetUserByUserName(User.Identity.Name);
                    Comment cmmEntity = new Comment();
                    CommentModel.ToEntity(cmmModel, ref cmmEntity);
                    cmmEntity.Id = 0;
                    cmmEntity.CreatedTime = DateTime.Now;
                    cmmEntity.UserId = user.Id;
                    cmmEntity.UserName = user.FullName;
                    cmmEntity.Logo = user.CompanyLogo;
                    _cmm_srv.CreateOrUpdate(cmmEntity);
                    string action = "Comment on " + "<a href='" + System.Web.HttpContext.Current.Request.UrlReferrer + "'>" + orderModel.Title + "</a>";
                    UpdateOrderActivities(OrderActions.Comment, orderModel, action, _User);
                }
                return RedirectToAction("Details", "Order", new { id = cmmModel.OrderId });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void UploadFile(OrderModels model, IEnumerable<HttpPostedFileBase> FileUp, User _User, string fileNote)
        {
            if (FileUp != null)
            {
                if (FileUp.Count() > 0)
                {
                    string folder = model.Title;
                    string filenamefinal = string.Empty;
                    folder = EscapeName.Renamefile(folder);
                    foreach (var file in FileUp)
                    {
                        if (file != null)
                        {
                            if (file.ContentLength > 0)
                            {
                                var dir = Server.MapPath("~/Content/File/Remsigns/");
                                var path = "";
                                var db_path = "";
                                bool isImage = false;
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                var ext = Path.GetExtension(file.FileName);
                                if (ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                                {
                                    dir = Server.MapPath("~/Content/File/Remsigns/");
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                    var filename = Path.GetFileNameWithoutExtension(file.FileName);
                                    //filename = EscapeName.Renamefile(filename);
                                    string tmp = Path.GetRandomFileName().Substring(0, 3);
                                    filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                                    db_path = "Content/File/Remsigns/" + filenamefinal;
                                    file.SaveAs(path);
                                    isImage = true;
                                    file.SaveAs(path);
                                }
                                else if (ext == ".pdf")
                                {
                                    dir = Server.MapPath("~/Content/File/Remsigns/" + folder + "/Pdf");
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                    var filename = Path.GetFileNameWithoutExtension(file.FileName);
                                    //filename = EscapeName.Renamefile(filename);
                                    string tmp = Path.GetRandomFileName().Substring(0, 3);
                                    filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                                    db_path = "Content/File/Remsigns/" + filenamefinal;
                                    file.SaveAs(path);
                                }
                                else if (ext == ".doc" || ext == ".docx" || ext == ".xls" || ext == ".xlsx" || ext == ".ppt" || ext == ".pptx")
                                {
                                    dir = Server.MapPath("~/Content/File/Remsigns/");
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                    var filename = Path.GetFileNameWithoutExtension(file.FileName);
                                    //filename = EscapeName.Renamefile(filename);
                                    string tmp = Path.GetRandomFileName().Substring(0, 3);
                                    filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                                    db_path = "Content/File/Remsigns/" + filenamefinal;
                                    file.SaveAs(path);
                                }
                                else
                                {
                                    dir = Server.MapPath("~/Content/File/Remsigns/");
                                    if (!Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                    var filename = Path.GetFileNameWithoutExtension(file.FileName);
                                    //filename = EscapeName.Renamefile(filename);
                                    string tmp = Path.GetRandomFileName().Substring(0, 3);
                                    filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp) + ext;
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns"), filenamefinal);
                                    db_path = "Content/File/Remsigns/" + filenamefinal;
                                    file.SaveAs(path);
                                }

                                FileUpload pt = new FileUpload()
                                {
                                    FileName = filenamefinal,
                                    OrderId = model.Id,
                                    OrderTitle = model.Title,
                                    UploadById = _User.Id,
                                    UploadByName = _User.FullName,
                                    DateUpload = DateTime.Now,
                                    ModifiedById = _User.Id,
                                    ModifiedByName = _User.FullName,
                                    DateModified = DateTime.Now,
                                    CountModified = 0,
                                    FileLink = db_path,
                                    Filekind = ext,
                                    Published = true,
                                    IsImage = isImage,
                                    Note = fileNote
                                };
                                file_srv.CreateOrUpdate(pt);
                            }

                        }
                    }
                }
            }
        }
    }

}
