using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hite.Web.Admin.Order.Models
{
    public class OrdersAdminContext
    {
        public static readonly string LOGINCOOKIEKEY = "oa";
        private OrdersAdminContext() { }
        public static OrdersAdminContext Current
        {
            get { return new OrdersAdminContext(); }
        }
        public bool IsInRole(string roleName) {
            if (IsLogin)
            {
                string cookieRole = GetCookieList()[1];
                if (cookieRole.ToLower() == Hite.Model.OrderAdminRoleType.SuperAdmin.ToString().ToLower()) { return true; }
                if (cookieRole.ToLower() == roleName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsInRole(Hite.Model.OrderAdminRoleType roleType) { 
            if(IsLogin){
                string cookieRole = GetCookieList()[1];
                if (cookieRole.ToLower() == Hite.Model.OrderAdminRoleType.SuperAdmin.ToString().ToLower()) { return true; }
                if (cookieRole.ToLower() == roleType.ToString().ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsSuperAdmin {
            get {
                if (IsLogin)
                {
                    string cookieRole = GetCookieList()[1];
                    if (cookieRole.ToLower() == Hite.Model.OrderAdminRoleType.SuperAdmin.ToString().ToLower()) { return true; }
                }
                return false;
            }
        }
        public bool IsLogin
        {
            get {
                var _cookieList = GetCookieList();
                if (_cookieList.Count == 2) { return true; }
                return false;
            }
        }
        public string UserName
        {
            get { 
                var _cookieList = GetCookieList();
                if (_cookieList.Count == 2) { return _cookieList[0]; }
                return string.Empty;
            }
        }
        private List<string> GetCookieList() {
            string[] _value = GetCookieValue().Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            foreach(string s in _value){
                if (!string.IsNullOrEmpty(s))
                {
                    list.Add(s);
                }
            }
            return list;
        }
        private string GetCookieValue() {
            string _cookieValue = Controleng.Common.Utils.GetCookie(LOGINCOOKIEKEY);
            if(!string.IsNullOrEmpty(_cookieValue)){
                return Goodspeed.Library.Security.DESCryptography.Decrypt(_cookieValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]);
            }
            return _cookieValue;
        }
    }
}