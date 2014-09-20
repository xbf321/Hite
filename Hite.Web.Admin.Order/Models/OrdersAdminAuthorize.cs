using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hite.Web.Admin.Order.Models
{
    public class OrdersAdminAuthorize : FilterAttribute, IAuthorizationFilter
    {

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if(!OrdersAdminContext.Current.IsLogin){
                filterContext.Result = new RedirectResult("/Login");
            }
        }

        #endregion
    }
}