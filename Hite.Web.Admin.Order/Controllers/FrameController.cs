using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Hite.Web.Admin.Order.Models;
using System.Collections;

namespace Hite.Web.Admin.Order.Controllers
{
    [OrdersAdminAuthorize]
    public class FrameController : Controller
    {
        //
        // GET: /SiteMap/
        public ActionResult Index() {
            return View();
        }
        public ActionResult Left() {
            return View();
        }
        public ActionResult Bottom()
        {
            return View();
        }
        public ActionResult Top() {
            return View();
        }
        public ActionResult Right() {
            return RedirectToAction("List","OrderUsers");
        }
        public ActionResult Center() {
            return View();
        }

        public ActionResult TreeViewRender()
        {
            return Content(TreeView());
        }
        

        #region == 生成后台菜单树HTML ==
        /// <summary>
        /// 生成后台菜单树HTML
        /// </summary>
        /// <param name="helper">待扩展的HtmlHelper</param>
        /// <param name="roles">用户角色数组</param>
        /// <returns>特定于用户角色的菜单树HTML</returns>
        public static string TreeView()
        {
            return BuildNode(SiteMap.RootNode);
        }
        private static string BuildNode(SiteMapNode childnode)
        {
            var nodes = childnode.ChildNodes;
            if (nodes.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (SiteMapNode n in nodes)
            {
                if (IsInRoles(n.Roles))
                {
                    int level = Controleng.Common.TypeConverter.StrToInt(n.Description, 0);
                    sb.AppendFormat("<li{0}>", level < 2 ? "" : "");

                    sb.Append("<a");
                    if (!string.IsNullOrEmpty(n.Url))
                    {
                        sb.AppendFormat(" href=\"{0}\" target=\"right\"", n.Url);
                    }
                    else
                    {
                        sb.Append(" onclick=\"return false;\"");
                    }
                    sb.AppendFormat(" title=\"{0}\">{0}</a>",
                                    n.Title);
                    //递归
                    sb.Append(BuildNode(n));
                    sb.AppendLine("</li>");
                }
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        private static bool IsInRoles(IList nodeRoles)
        {
            foreach (string role in nodeRoles)
            {
                if (role.Equals("*")) return true;
                if (OrdersAdminContext.Current.IsInRole(role)) { return true; }
            }
            return false;
        }
        #endregion

    }
}
