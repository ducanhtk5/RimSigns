using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Common.Helpers;
using CMS.Common.Json;
using CMS.Common.Mvc;
using CMSApplication.Areas.Administration.Models;
using System.Web.Security;

namespace CMSApplication.Areas.Administration.Controllers
{
    public class ManagementController : UTController
    {
        //
        // GET: /Administration/Management/
        private EncryptAndDecrypt encrypt = new EncryptAndDecrypt();
         [Authorize(Roles = "Administrator,User")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logon()
        {
            return View(); 
        }
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.Password = encrypt.Encrypt(model.Password, "Click4Restaurants", true);
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Management", new { area="Administration"});
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
         [Authorize(Roles = "Administrator,Manager")]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Management", new { area = "Administration" });
        }
        #region Images

        public string UploadImage(HttpPostedFileBase file)
        {
            string basePath = "~/Content/img";
            FileHelper fileHelper = new FileHelper(basePath);
            string fileName = fileHelper.SaveFile(null, file.FileName, file.InputStream);
            string filePathStr = Url.Content(basePath) + @"/" + fileName;
            return filePathStr;
        }

        [HttpPost]
        public ActionResult UploadImageStr(string qqfile)
        {
            string basePath = "~/Content/File/ImagesUpload";
            string[] fileNameArray = qqfile.Split(new char[] { '.' });
            if (fileNameArray.Length == 1)
            {
                return JsonError("Error!");
            }
            FileHelper fileHelper = new FileHelper(basePath);
            string fileName = fileHelper.SaveFile(null, fileNameArray[1], Request.InputStream);
            string filePathStr = Url.Content(basePath) + @"/" + fileName;

            return Json(new { success = true,  filePath = fileName });
        }

        [HttpPost]
        public ActionResult UploadImageStrFIle(string qqfile)
        {
            string basePath = "~/Content/File/FileUpload";
            string[] fileNameArray = qqfile.Split(new char[] { '.' });
            if (fileNameArray.Length == 1)
            {
                return JsonError("Error!");
            }
            FileHelper fileHelper = new FileHelper(basePath);
            string fileName = fileHelper.SaveFile(null, fileNameArray[1], Request.InputStream);
            string filePathStr = Url.Content(basePath) + @"/" + fileName;
            return Json(new { success = true, filePath = fileName });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum,
                                      string CKEditor, string langCode)
        {
            string url = string.Empty; // url to return
            string message = string.Empty; // message to display (optional)

            // here logic to upload image
            // and get file path of the image

            // path of the image
            string path = UploadImage(upload);

            // will create http://localhost:1457/Content/Images/my_uploaded_image.jpg

            url = Request.Url.GetLeftPart(UriPartial.Authority) + path;

            // passing message success/failure
            message = "Upload image success.";

            // since it is an ajax request it requires this string
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }

        #endregion


        #region Site Security
        /// <summary>
        /// Take down the site 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string SiteDown(string password)
        {
            return CMS.Common.Security.ABSecurity.TakeDown(encrypt.Encrypt(password, "absoft.vn", true)).ToString();
        }

        /// <summary>
        /// Site up to normal state
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string SiteUp(string password)
        {
            return CMS.Common.Security.ABSecurity.TakeUp(encrypt.Encrypt(password, "absoft.vn", true)).ToString();
        }
        #endregion
    }
}
