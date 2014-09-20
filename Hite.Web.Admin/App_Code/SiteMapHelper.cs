using System.Linq;
using System.Web;
using System.Text;
using System.Collections;
using Hite.Services;


/// <summary>
/// Summary description for SiteMap
/// </summary>
public static class SiteMapHelper
{
    /// <summary>
    /// 生成后台菜单树HTML
    /// </summary>
    /// <param name="helper">待扩展的HtmlHelper</param>
    /// <param name="roles">用户角色数组</param>
    /// <returns>特定于用户角色的菜单树HTML</returns>
    public static HtmlString TreeView()
    {
        return new HtmlString(BuildNode(SiteMap.RootNode));
    }
    private static string BuildNode(SiteMapNode childnode)
    {
        var nodes = childnode.ChildNodes;
        if (nodes.Count == 0) return string.Empty;
        StringBuilder sb = new StringBuilder();
        sb.Append("<ul>");
        foreach (SiteMapNode n in nodes)
        {
            if (n.Roles.Count == 0 || IsInRoles(n.Roles))
            {
                int level = Controleng.Common.TypeConverter.StrToInt(n.Description, 0);
                sb.AppendFormat("<li{0}>", level < 2 ? " class=\"open\"" : "");
                
                sb.Append("<a");
                if (!string.IsNullOrEmpty(n.Url))
                {
                    sb.AppendFormat(" href=\"{0}\"", n.Url);
                }
                else
                {
                    sb.Append(" onclick=\"return false;\"");
                }
                sb.AppendFormat(" title=\"{0}\" target=\"right\">{0}</a>",
                                n.Title);
                if(n.Title == "网站管理"){
                    var sites = SiteService.List();
                    foreach(var siteInfo in sites){
                        
                    }
                }
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
            if (role.Contains("*")) { return true; }
            if(PagesAdminContext.Current.IsInRoles(role)){
                return true;
            }
        }
        return false;
    }
}