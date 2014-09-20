using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;

namespace Hite.Common
{
    public static class PagerBarHelper
    {
        public static string Render(int pageIndex, int displaySize, int totalNumber) {
            return Render(pageIndex,displaySize,totalNumber,null);
        }
        public static string Render(int pageIndex,int displaySize,int totalNumber,object htmlAttributes){
            if (pageIndex <= 0) { pageIndex = 1; }
            int pages = totalNumber % displaySize == 0 ? totalNumber / displaySize : totalNumber /displaySize +1;
            if (pages < 2) { return string.Empty; }
            if (pageIndex >= pages) { pageIndex = pages; }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div");
            if(htmlAttributes != null){
                foreach(PropertyInfo pi in htmlAttributes.GetType().GetProperties()){
                    sb.AppendFormat(" {0}=\"{1}\"",pi.Name,pi.GetValue(htmlAttributes,null));
                }
            }
            sb.Append(">");
            sb.AppendFormat("共<em>{0}</em>条记录,&nbsp;<em>{1}</em>&nbsp;/&nbsp;{2}&nbsp;",totalNumber,pageIndex,pages);
            if(pageIndex != 1){
                //显示第一页
                sb.AppendFormat("<span>{0}</span>",BuildUrl(1,"第一页"));
            }
            if (pageIndex - 1 > 0)
            {
                sb.AppendFormat("<span>{0}</span>", BuildUrl(pageIndex - 1, "上一页"));
            }
            else {
                sb.Append("<span>上一页</span>");
            }
            if(pageIndex +1 <=pages){
                sb.AppendFormat("<span>{0}</span>", BuildUrl(pageIndex + 1, "下一页"));
            }
            else
            {
                sb.Append("<span>下一页</span>");
            }
            
            if(pageIndex != pages){
                //显示最后一页
                sb.AppendFormat("<span>{0}</span>",BuildUrl(pages, "最后一页"));
            }
            sb.Append("</div>");
            return sb.ToString();
        }
        private static string BuildUrl(int pageIndex,string text) {
            string localPath = HttpContext.Current.Request.Url.LocalPath;
            StringBuilder sbLocalPath = new StringBuilder(localPath);
            sbLocalPath.Append("?");
            var querys = HttpContext.Current.Request.QueryString;
            if (querys != null)
            {
                foreach (string key in querys.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.Equals("page")) { continue; }
                    string value = HttpUtility.UrlEncode(querys[key]);
                    sbLocalPath.AppendFormat("{0}{1}&", string.IsNullOrEmpty(key) ? string.Empty : string.Concat(key, "="), value);
                }
            }
            sbLocalPath.AppendFormat("page={0}", pageIndex);
            return string.Format("<a href=\"{0}\" target=\"_self\">{1}</a>",sbLocalPath.ToString(),text);
        }
    }
}
