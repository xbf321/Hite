using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for SiteHelper
/// </summary>
public class SiteHelper
{
    public static HtmlString RenderDropdownList(string name,string value) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<select id=\"{0}\" name=\"{0}\">",name);
        var siteList = Hite.Services.SiteService.List();
        sb.Append("<option value=\"0\">==请选择==</option>");
        foreach(var item in siteList){            ;
            sb.AppendFormat("<option value=\"{0}\" {2}>{1}</option>", item.Id, item.Name, item.Id.ToString() == value ? "selected=\"selected\"" : string.Empty);
        }
        sb.Append("</select>");
        return new HtmlString(sb.ToString());
    }
}