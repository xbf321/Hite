using System;
using System.Web.Mvc;

namespace Hite.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SilenceHandleErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.Url.Host.IndexOf(".3721.com") <= 0)
            {
                string controller = filterContext.RouteData.Values["controller"] as string;
                string action = filterContext.RouteData.Values["action"] as string;

                string msg = string.Format("Controller:{0},Action:{1},IP:{4},UserName:{5}发生异常!\r\n{2}\r\nUserAgent:{3}\r",
                    controller,
                    action,
                    filterContext.HttpContext.Request.Url,
                    filterContext.HttpContext.Request.UserAgent,
                    filterContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"],
                    filterContext.HttpContext.User.Identity.Name
                    );

                if (filterContext.IsChildAction)
                {
                    filterContext.Result = new EmptyResult();
                }
                else
                {
                    filterContext.Result = new ViewResult { ViewName = "Error" };
                }
                filterContext.ExceptionHandled = true;

                log4net.LogManager.GetLogger(typeof(SilenceHandleErrorAttribute))
                            .Error(msg, filterContext.Exception);
            }
        }

        #endregion
    }
}
