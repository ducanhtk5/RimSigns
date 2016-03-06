using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using CMS.Common.Json;
using System.Text.RegularExpressions;
using CMS.Common.Mvc;
using System.Net.Mail;
using System.Web.Routing;

namespace CMS.Common.Mvc
{
    public abstract class UTController : Controller
    {
        public int PageSize = 25;
        private UTContext utContext = null;
        public UTContext UtContext
        {
            get
            {
                if (utContext == null)
                {
                    utContext = Session[UTContext.SessionContextKey] as UTContext;
                    if (utContext == null)
                    {
                        utContext = new UTContext();
                        Session[UTContext.SessionContextKey] = utContext;
                    }
                }
                return utContext;
            }
            private set { utContext = value; }
        }

        // check for mobile
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var has_session = (filterContext.RequestContext.HttpContext.Session["theme"] != null) ? true : false;
            var isMobile = CMS.Service.Helper.MobileHelper.isMobile(filterContext.RequestContext.HttpContext.Request);
            if (isMobile && (filterContext.RequestContext.HttpContext.Session.IsNewSession || !has_session))
            {
                filterContext.RequestContext.HttpContext.Session.Add("theme", "Mobile");
            }
        }



        /// <summary>
        /// Gets an object of type T from Session which is stored with key K.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="key">The key object is associated with</param>
        /// <returns>Object of type T if found, default(T) if the object could not be found</returns>
        public T TryGetTempValue<T>(string key)
        {
            return TryGetTempValue<T>(key, false);
        }

        /// <summary>
        /// Gets an object of type T from Session which is stored with key K. If remove parameter is true, 
        /// it also removes the object from session.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="key">The key object is associated with</param>
        /// <param name="remove">Removes the object from store</param>
        /// <returns>Object of type T if found, default(T) if the object could not be found</returns>
        public T TryGetTempValue<T>(string key, bool remove)
        {
            T tempObject = default(T);
            Type objtype = typeof(T);

            if (string.IsNullOrWhiteSpace(key))
            {
                return default(T);
            }

            string lookupKey = typeof(T).ToString() + "_" + key;

            try
            {
                tempObject = (T)Session[lookupKey];
                if (remove)
                {
                    Session.Remove(lookupKey);
                }
            }
            catch
            {
                tempObject = default(T);
            }

            return tempObject;
        }

        public void SetTempValue<T>(T obj, string key)
        {
            if (obj == null || string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            string lookupKey = typeof(T).ToString() + "_" + key;

            Session[lookupKey] = obj;
        }

        public JsonResult JsonError(string message)
        {
            JsonResponse response = new JsonResponse() { Status = JsonResponse.OperationFailure, Message = message };
            return new JsonResult() { Data = response };
        }

        public JsonResult JsonSuccess(string redirectUrl, string message = null)
        {
            JsonResponse response = new JsonResponse();
            response.Status = JsonResponse.OperationSuccess;
            response.RedirectUrl = redirectUrl;
            response.Message = message;

            return new JsonResult() { Data = response };
        }

        public bool IsValidEmailAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            //Regex re = new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            //if (re.IsMatch(s))
            //    return true;
            //else
            //    return false;
            try
            {
                new MailAddress(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool IsNumber(string text)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(text);
        }

    }
}
