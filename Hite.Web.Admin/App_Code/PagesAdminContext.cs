using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PagesAdminContext
/// </summary>
public class PagesAdminContext
{
	private PagesAdminContext()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static readonly string NOTAUTHTEXT = "您没有权限，请联系系统管理员！";
    public static PagesAdminContext Current {
        get {
            return new PagesAdminContext();
        }
    }
    public string UserName
    {
        get {
            string[] s = GetCookieValue().Split(new char[]{'#'},StringSplitOptions.RemoveEmptyEntries);
            if(s.Length > 1){
                return s[0];
            }
            return string.Empty;
        }
    }
    
    public bool IsLogin {
        get {
            if (!string.IsNullOrEmpty(GetCookieValue())) { return true; }
            return false;
        }
    }
    public bool IsInRoles(string roleName) {
        var roles = GetCookieRoles();
        foreach (var item in roles)
        {
            if (item.Value == roleName)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsInSites(int siteId) {
        var roles = GetCookieRoles();

        //检查是否是超级管理员
        if (IsSuperAdmin()) { return true; }

        foreach (var item in roles)
        {
            if (item.Key == siteId)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsSuperAdmin() {
        var roles = GetCookieRoles();
        foreach(var item in roles){
            if(item.Value == "superadmin"){
                return true;
            }
        }
        return false;

    }
    public Dictionary<int, string> GetCookieRoles() {
        //cookie 123123@tom.com#superadmin|0,www|1
        Dictionary<int, string> roles = new Dictionary<int, string>();
        string[] cookieValues = GetCookieValue().Split(new char[]{'#'},StringSplitOptions.RemoveEmptyEntries);
        if(cookieValues.Length >1){
            //superadmin|0,www|1
            var _a = cookieValues[1].Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            foreach(var _b in _a){
                //superadmin|0
                var _c = _b.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
                if(_c.Length >1){
                    //_c[0] : superadmin
                    //_c[1] : 0
                    int _siteId = Convert.ToInt32(_c[1]);
                    if(!roles.ContainsKey(_siteId)){
                        roles.Add(_siteId,_c[0].ToLower());
                    }
                }
            }
        }
        return roles;
    }
    private string GetCookieValue() {
        string cookieValue = Controleng.Common.Utils.GetCookie("pa");
        if(!string.IsNullOrEmpty(cookieValue)){
            
            return Goodspeed.Library.Security.DESCryptography.Decrypt(cookieValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]);
        }
        return string.Empty;
    }
}