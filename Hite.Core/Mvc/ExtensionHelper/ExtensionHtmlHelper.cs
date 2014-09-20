using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System;

using Hite.Model;
using Hite.Common;
namespace Hite.Mvc
{
    public static class ExtensionHtmlHelper
    {
        #region == RenderTemplatesDropdownList ==
        public static IHtmlString RenderTemplatesDropdownList(string name, object value)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = EnumHelper.EnumListTable(typeof(TemplateType));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    items.Add(new SelectListItem() { Text = dr[0].ToString(), Value = Convert.ToInt32(dr[1]).ToString() });
                }
            }
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            if (items != null)
            {
                foreach (SelectListItem item in items)
                {
                    string selected = "";
                    if (value != null && item.Value.Equals(value.ToString())) selected = "selected=\"selected\"";
                    sBuilder.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>", item.Text, item.Value, selected);
                }
            }
            sBuilder.Append("</select>");
            return new HtmlString(sBuilder.ToString());
        }
        #endregion

        #region == RenderLanguageDropdownList ==
        public static HtmlString RenderLanguageDropdownList(string name, string value)
        {
            DataTable dt = EnumHelper.EnumListTable(typeof(WebLanguage));
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<select id=""{0}"" name=""{0}"">",name);
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow item in dt.Rows){
                    string selected = string.Empty;
                    if(!string.IsNullOrEmpty(value) && value == item["Value"].ToString()){
                        selected = "selected=\"selected\"";
                    }
                    sb.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>",item["Text"],item["Value"],selected);
                }
            }
            sb.Append("</select>");
            return new HtmlString(sb.ToString());
        }
        #endregion

        #region == RenderFileExtensionDropdownList ==
        public static HtmlString RenderIndexFileNameOfSiteDropdownList(string name, string value)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = EnumHelper.EnumListTable(typeof(IndexFileNameOfSite));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    items.Add(new SelectListItem() { Text = dr[0].ToString(), Value = Convert.ToInt32(dr[1]).ToString() });
                }
            }

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            if (items != null)
            {
                foreach (SelectListItem item in items)
                {
                    string selected = "";
                    if (value != null && item.Value.Equals(value.ToString())) selected = "selected=\"selected\"";
                    sBuilder.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>", item.Text, item.Value, selected);
                }
            }
            sBuilder.Append("</select>");
            return new HtmlString(sBuilder.ToString());
        }
        #endregion

        #region == RenderFileExtensionDropdownList ==
        public static HtmlString RenderFileExtensionDropdownList(string name,string value) {
            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = EnumHelper.EnumListTable(typeof(FileExtension));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    items.Add(new SelectListItem() { Text = dr[0].ToString(), Value = Convert.ToInt32(dr[1]).ToString() });
                }
            }
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            if (items != null)
            {
                foreach (SelectListItem item in items)
                {
                    string selected = "";
                    if (value != null && item.Value.Equals(value.ToString())) selected = "selected=\"selected\"";
                    sBuilder.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>", item.Text, item.Value, selected);
                }
            }
            sBuilder.Append("</select>");
            return new HtmlString(sBuilder.ToString());
        }
        #endregion

        #region == GetOrderStatusSelectListItem ==
        public static HtmlString RenderOrderStatusDropdownList(string name, string value)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = EnumHelper.EnumListTable(typeof(OrderStatus));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    items.Add(new SelectListItem() { Text = dr[0].ToString(), Value = Convert.ToInt32(dr[1]).ToString() });
                }
            }
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            sBuilder.Append("<option value=\"\">==请选择==</option>");
            if (items != null)
            {
                foreach (SelectListItem item in items)
                {
                    string selected = "";
                    if (value != null && item.Value.Equals(value.ToString())) selected = "selected=\"selected\"";
                    sBuilder.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>", item.Text, item.Value, selected);
                }
            }
            sBuilder.Append("</select>");
            return new HtmlString(sBuilder.ToString());
        }
        #endregion

        #region == RenderUserIndustryDropdownList ==
        public static HtmlString RenderUserIndustryDowndownList(string name) {
            return RenderUserIndustryDowndownList(name,string.Empty);
        }
        public static HtmlString RenderUserIndustryDowndownList(string name,string selectValue) {

            string[] industryArray = { "汽车", "化工", "建筑", "电力", "数据通信", "电话通信", "航空", "海运", "纸业", "纺织", "汽车", "钢材", "电子", "铁路", "医药", "其他" };
            
            StringBuilder sbText = new StringBuilder();
            sbText.AppendFormat(@"<select id=""{0}"" name=""{0}"" class=""required"">", name);
            sbText.Append("<option value=\"\">==请选择所在行业==</option>");
            foreach(var s in industryArray){
                string selected = "";
                if (selectValue != null && s.Equals(selectValue)) selected = "selected=\"selected\"";
                sbText.AppendFormat(@"<option value=""{0}"" {1}>{0}</option>",s, selected);
            }
            sbText.Append("</select>");
            return new HtmlString(sbText.ToString());
        }
        #endregion

    }
}
