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
using System.IO;
using CMS.Service.Enum;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class OrderManagementController : UTController
    {
        //
        // GET: /Administration/Order/
        private OrderRepository _order_srv = new OrderRepository();
        private UserRepositry _user_srv = new UserRepositry();
        private FileUploadRepository file_srv = new FileUploadRepository();
        private OrderActivitiesRepository _orderAct_services = new OrderActivitiesRepository();
        private CommentRepository _cmm_srv = new CommentRepository();

        public ActionResult Index(int? userId)
        {
            if (userId.HasValue)
                ViewData.Add("user_id", userId.Value);
            else
                ViewData.Add("user_id", 0);
            return View();
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult List(int page, int userId)
        {
            List<OrderModels> model = new List<OrderModels>();
            var c = new List<Order>();
            if (userId == 0)
                c = _order_srv.List();
            else
                c = _order_srv.List().Where(m => m.OrderById == userId).ToList();
            foreach (var item in c)
            {
                OrderModels ml = new OrderModels();
                OrderModels.ToModel(item, ref  ml);
                model.Add(ml);
            }

           /* var ListNewsPageSize = new PagedData<OrderModels>();

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
            */

            return PartialView("_List", model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Details(int id)
        {
            OrderModels model = new OrderModels();
            var order = _order_srv.GetById(id);
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

        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult SortByDay(DateTime day_to, DateTime day_from)
        {
            var ListNewsPageSize = new PagedData<OrderModels>();
            List<Order> orderEntityList = new List<Order>();
            if (day_to > day_from)
            {
                var rs = _order_srv.SortByDay(day_from, day_to);
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
            return PartialView("_List", ListNewsPageSize.Data);
        }


        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult FullSearch(string keys)
        {
            var ListNewsPageSize = new PagedData<OrderModels>();
            List<Order> orderEntityList = new List<Order>();
            var rs = _order_srv.FullSearch(keys);
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
            return PartialView("_List", ListNewsPageSize.Data);
        }


        [Authorize(Roles = "Administrator,Manager")]
        [HttpPost]
        public ActionResult UpdateInProgress(OrderModels orderModel)
        {
            User _User = new User();
            string Username = HttpContext.User.Identity.Name;
            if (Username != null)
            {
                _User = _user_srv.GetUserByUserName(Username);
            }
            else { return RedirectToAction("LogOn", "Management"); }

            Order entity = new Order();
            entity = _order_srv.GetById(orderModel.Id);

            if (!entity.Status.Equals(orderModel.Status))
            {
                string action = "Change status from " + entity.Status + " to " + orderModel.Status + " on " + "<a href='/Order/Details/" + orderModel.Id + "'>" + orderModel.Title + "</a>";
                UpdateOrderActivities(OrderActions.ChangeStatus, orderModel, action, _User);
            }

            //if (!entity.OrderPercent.Equals((int)orderModel.OrderPercent))
            //{
            //    string action = "Change percent from " + entity.OrderPercent + " to " + orderModel.OrderPercent + " on " + "<a href='/Order/Details/" + orderModel.Id + "'>" + orderModel.Title + "</a>";
            //    UpdateOrderActivities(OrderActions.UpdatePercent, orderModel, action, _User);
            //}

            entity.StaffID = CMS.Common.Helpers.StringHelpers.ConvertListToString(orderModel.StaffID);
            entity.Status = orderModel.Status;
            //entity.OrderPercent = (int)orderModel.OrderPercent;
            _order_srv.CreateOrUpdate(entity);
            return RedirectToAction("Details", new { id = entity.Id});
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Add(int userId)
        {
            OrderModels model = new OrderModels();
            model.Id = 0;
            model.OrderById = userId;
            model.GetDataForDropdownList();
            return View(model);
        }

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int id)
        {
            OrderModels model = new OrderModels();
            var entity = _order_srv.GetById(id);
            OrderModels.ToModel(entity, ref model);
            model.GetDataForDropdownList();
            return View(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult AddOrUpdate(OrderModels model, IEnumerable<HttpPostedFileBase> FileUp)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                return Redirect(Url.RequestContext.HttpContext.Request.UrlReferrer.ToString());
            }
            User _User = new User();
            _User = _user_srv.GetById(model.OrderById);
            if (model.Id == 0)
            {
                //// Add
                model.OrderDate = DateTime.Now;
                if (_User != null)
                {
                    model.OrderById = _User.Id;
                    model.OrderByName = _User.UserName;
                    ///////Upload
                    UploadFile(model, FileUp, _User, "");
                }
                else { return RedirectToAction("LogOn", "Management"); }
            }
            else
            {
                Order oldOrder = _order_srv.GetById(model.Id);
                model.StaffID = (List<string>)CMS.Common.Helpers.StringHelpers.ConvertStringToList(oldOrder.StaffID);
                string host = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                Uri uri = new Uri(host);
                string host2 = uri.AbsolutePath;
                if (!oldOrder.Status.Equals(model.Status))
                {
                    string action = "Change status from " + oldOrder.Status + " to " + model.Status + " on " + "<a href='/Order/Details/"+ model.Id + "'>" + model.Title + "</a>";
                    UpdateOrderActivities(OrderActions.ChangeStatus, model, action, _User);
                }

                if (!oldOrder.OrderPercent.Equals(model))
                {
                    string action = "Change percent from " + oldOrder.OrderPercent + " to " + model.OrderPercent + " on " + "<a href='/Order/Details/" + model.Id + "'>" + model.Title + "</a>";
                    UpdateOrderActivities(OrderActions.UpdatePercent, model, action, _User);
                }
            }
            ///////Upload
            UploadFile(model, FileUp, _User, "");

            Order entity = new Order();
            OrderModels.ToEntity(model, ref entity);
            entity.StaffID = CMS.Common.Helpers.StringHelpers.ConvertListToString(model.StaffID);
            _order_srv.CreateOrUpdate(entity);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int Id)
        {
            _order_srv.Delete(Id);
            var list = _orderAct_services.GetTableListByOrderId(Id);
            if (list != null)
            {
                foreach (var item in list)
                {
                    _orderAct_services.Delete(item.Id);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFile(OrderModels model, IEnumerable<HttpPostedFileBase> FileUp, string fileNote)
        {
            User _User = new User();
            string Username = HttpContext.User.Identity.Name;
            if (Username != null)
            {
                _User = _user_srv.GetUserByUserName(Username);
            }
            else { return RedirectToAction("LogOn", "Management"); }

            //Uplpoad
            UploadFile(model, FileUp, _User, fileNote);

            //Update order activities
            string action = "Upload " + FileUp.Count() + " files on " + "<a href='" + System.Web.HttpContext.Current.Request.UrlReferrer + "'>" + model.Title + "</a>";
            UpdateOrderActivities(OrderActions.UploadFile, model, action, _User);
            return RedirectToAction("Details", new { id = model.Id });
        }

         [Authorize(Roles = "Administrator,Manager")]
        public ActionResult SendComment(CommentModel cmmModel)
        {
            User _User = new User();
            string Username = HttpContext.User.Identity.Name;
            if (Username != null)
            {
                _User = _user_srv.GetUserByUserName(Username);
            }
            else { return RedirectToAction("LogOn", "Management"); }

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
                _cmm_srv.CreateOrUpdate(cmmEntity);
                string action = "Comment on " + "<a href='" + System.Web.HttpContext.Current.Request.UrlReferrer + "'>" + orderModel.Title + "</a>";
                UpdateOrderActivities(OrderActions.Comment, orderModel, action, _User);
            }
            return RedirectToAction("Details", new { id = cmmModel.OrderId });
        }

         public ActionResult DeleteFile(int id, int orderID)
         {
             file_srv.Delete(id);
             return RedirectToAction("Details", new { id = orderID });
         }

         public ActionResult DeleteComment(int id, int orderID)
         {
             _cmm_srv.Delete(id);
             return RedirectToAction("Details", new { id = orderID });
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
                                var dir = Server.MapPath("~/Content/File/Remsigns/" );
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
                                    filenamefinal = EscapeName.Renamefile(folder + "-" + filename + "-" + tmp )+ ext;
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns/"), filenamefinal);
                                    db_path = "Content/File/Remsigns/" + filenamefinal;
                                    file.SaveAs(path);
                                    isImage = true;
                                }
                                else if (ext == ".pdf")
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
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns/"), filenamefinal);
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
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns/"), filenamefinal);
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
                                    path = Path.Combine(Server.MapPath("~/Content/File/Remsigns/"), filenamefinal);
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

        private void UpdateOrderActivities(CMS.Service.Enum.OrderActions orderAction, OrderModels model, string action, User _User)
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
            if (model.Parties != null)
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

            var _order_user=_user_srv.GetById(model.OrderById);
            if (_order_user!=null)
            {
                mailList.Add(_order_user.Email);
            }

            Order order = new Order();
            OrderModels.ToEntity(model, ref order);

            string host = System.Web.HttpContext.Current.Request.Url.Host;
            SendEmail.SendEmailOrderActivities(order, orderAction, mailList, host);
        }
    }
}
