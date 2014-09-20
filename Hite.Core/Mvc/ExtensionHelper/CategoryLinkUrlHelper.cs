using System.Web.Mvc;
using System.Text;

using Hite.Model;

namespace Hite.Mvc
{
    public static class CategoryLinkUrlHelper
    {
        public static string BuildLink(CategoryInfo model) {
            return BuildLink(model,WebLanguage.zh_cn);
        }
        public static string BuildLink(CategoryInfo model,WebLanguage language = WebLanguage.zh_cn) {
            return BuildLink(model,string.Empty,null,language);
        }
        public static string BuildLink(CategoryInfo model, string linkText = "", object htmlAttributes = null,WebLanguage language = WebLanguage.zh_cn) {

            var dic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            StringBuilder sbTag = new StringBuilder("<a");
            sbTag.AppendFormat(" id=\"{0}\"",model.Id);
            sbTag.AppendFormat(" title=\"{0}\"",model.Name);
            foreach(var p in dic){
                sbTag.AppendFormat(" {0}=\"{1}\"",p.Key,p.Value);
            }
            if(string.IsNullOrEmpty(linkText)){
                linkText = model.Name;
            }
            if (!string.IsNullOrEmpty(model.LinkUrl))
            {
                //不用新打开窗口 target=\"_blank\"
                sbTag.AppendFormat(" href=\"{0}\"", model.LinkUrl);
            }
            else
            {
                //TODO
                //这个地方还得做别名Url
                //目前是没做别名处理
                sbTag.AppendFormat(" href=\"{1}/channel/{0}.html\"", 
                    model.Alias == string.Empty ? model.Id.ToString() : model.Alias,
                    language == WebLanguage.zh_cn ? string.Empty : "/"+language.ToString());
            }
            sbTag.AppendFormat(">{0}</a>",linkText);
            return sbTag.ToString();
        }
    }
}
