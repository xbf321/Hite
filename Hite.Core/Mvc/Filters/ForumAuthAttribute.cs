using System;
using System.Web.Mvc;

namespace Hite.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ForumAuthAttribute : FilterAttribute,IAuthorizationFilter
    {

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string loginUrl = System.Configuration.ConfigurationManager.AppSettings["LOGINURL"];
            string currentUrl = filterContext.HttpContext.Request.Url.ToString();
            //1,判断登陆
            
            if(!filterContext.HttpContext.Request.IsAuthenticated){
                filterContext.HttpContext.Response.Redirect(string.Concat(loginUrl,currentUrl));
                filterContext.HttpContext.Response.End();
            }
            //2,判断当前用户是否通过审核,放在当前Controller自身检查
            
            //filterContext.Result = new ViewResult() { ViewName = "Tip", ViewData = new ViewDataDictionary(new TipModel { Msg = "对不起，该类目不存在", Url = "/" }) };
        }

        #endregion
    }
}
