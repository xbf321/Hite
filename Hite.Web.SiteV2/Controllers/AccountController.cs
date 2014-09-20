/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-19 14:35:15
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-19 14:35:15
 * Description: 用户账户控制器
 * ********************************************************************/  
using System;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.Security;
using System.Web;

using Hite.Model;
using Hite.Services;
using Hite.Mvc.Filters;
using Hite.Mvc;
using Controleng.Common;



namespace Hite.Web.Controllers.Site
{
    [SilenceHandleError]
    public class AccountController : HiteController
    {
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("modify");
        }

        #region == 修改信息 ==
        [Authorize]
        public ActionResult Modify() {
            UserInfo userInfo = UserService.Get(User.Identity.Name);

            return View(userInfo);
        }
        [Authorize]
        [HttpPost]        
        public ActionResult Modify(UserInfo oldModel,string Industry) {

            oldModel.Industry = Industry;
            //判断Email是否存在
            if(UserService.IsExistsEmail(oldModel.Email,oldModel.Id)){
                ViewBag.Msg = "更新失败，Email存在，请选择其他Email！";
                return View(oldModel);
            }
            UserService.Update(oldModel);
            
            ViewBag.Msg = "更新成功！";


            return View(oldModel);
        }
        #endregion

        #region == 修改密码 ==
        [Authorize]
        public ActionResult SetPwd() {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult SetPwd(FormCollection fc) {
            string userName = User.Identity.Name;
            string oldPwd = fc["txtOldPwd"];
            oldPwd = Controleng.Common.Utils.MD5(oldPwd);
            if(!UserService.ValidateUser(userName,oldPwd)){
                ViewBag.Msg = "修改失败，原密码不正确，请重试！";
                return View();
            }
            string newPwd = fc["txtNewPwdConfirm"];
            newPwd = Controleng.Common.Utils.MD5(newPwd);
            UserService.ChangePwd(userName,newPwd);
            ViewBag.Msg = "修改成功！";
            return View();
        }
        #endregion

        #region == 登录 ==
        public ActionResult Login()
        {
            
            //判断是否是hite.com.cn域下
            //如果不是，则跳转到PassportUrl下登录
            if(Request.Url.Host.IndexOf(FormsAuthentication.CookieDomain)<0){
                if(Request.IsAuthenticated){
                    Response.Redirect("/",true);
                }
                Response.Redirect(string.Format("{0}/accounts/login?ReturnUrl={1}", System.Configuration.ConfigurationManager.AppSettings["PassportUrl"], HttpUtility.UrlEncode(Request.Url.ToString().Replace("account/login",""))), true);
            }
            if (Request.IsAuthenticated)
            {
                //已经登录,直接跳到首页
                Response.Redirect("/");
                Response.End();
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {

            string userName = fc["txtUserName"];
            string userPwd = fc["txtUserPwd"];
            userPwd = Controleng.Common.Utils.MD5(userPwd);

            if (UserService.ValidateUser(userName, userPwd))
            {
                Login(userName);
            }
            ModelState.AddModelError("Error", "用户名或密码错误，请重试！");

            return View();
        }
        [NonAction]
        private void Login(string userName) {
            //更新最后登录时间
            var userInfo = UserService.Get(userName);
            UserService.UpdateLastLoginDateTime(userInfo.Id);

            int expiresTime = 1;//半小时
            FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddHours(expiresTime), true, string.Empty/*UserRoles*/, FormsAuthentication.FormsCookiePath);
            string HashTicket = FormsAuthentication.Encrypt(Ticket);
            HttpCookie lcookie = new HttpCookie(FormsAuthentication.FormsCookieName, HashTicket);
            lcookie.Expires = DateTime.Now.AddHours(expiresTime);
            lcookie.Domain = FormsAuthentication.CookieDomain;

            Response.Cookies.Add(lcookie);
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddHours(expiresTime);

            //以下信息为单点登录所使用
            
            //加密过的随机数
            string randomNumber = Utils.MD5((new Random()).Next().ToString());

            //格式 xbf321:XXX:DesKey
            //其中xxx是加密过的随机数
            //DesKey为salt
            //写入Hite Token 过期时间 4个月
            Response.Cookies.Add(new HttpCookie("ht", Utils.MD5(string.Format("{0}:{1}:{2}", userName, randomNumber, System.Configuration.ConfigurationManager.AppSettings["DESKey"]))) { Domain = FormsAuthentication.CookieDomain, Expires = DateTime.Now.AddMonths(4) });
            //把随机数写入Cookie
            Response.Cookies.Add(new HttpCookie("hr",randomNumber) { Domain = FormsAuthentication.CookieDomain,Expires = DateTime.Now.AddMonths(4) });

            //把用户名写入Cookie 过期时间 1年
            Response.Cookies.Add(new HttpCookie("username",userName) { Domain = FormsAuthentication.CookieDomain, Expires = DateTime.Now.AddYears(1) });

            
            //判断返回地址是否是本域下的
            //如果不是，则返回根站点，反之跳转
            string returnUrl = CECRequest.GetQueryString("ReturnUrl");
            ToRedirectHome(returnUrl);            
        }
        #endregion

        private void ToRedirectHome(string returnUrl)
        { 
            bool isRedirectReturnUrl = false;
            if(!string.IsNullOrEmpty(returnUrl)){
                var siteList = HiteContext.Current.SiteAllUrlList;
                string pattern = @"(?:[-\w]+)\.(.*)";
                string domain = "hite.com.cn";
                //检测是否是自己的站点
                foreach(string url in siteList){                    
                    if(Regex.IsMatch(url,pattern,RegexOptions.IgnoreCase)){
                        domain = Regex.Match(url, pattern, RegexOptions.IgnoreCase).Groups[1].Value;
                        
                    }
                    if(returnUrl.IndexOf(domain)>0){
                        //说明是
                        isRedirectReturnUrl = true;
                        break;
                    }
                }
            }
            Response.Clear();
            if(isRedirectReturnUrl){
                Response.Redirect(returnUrl);
                Response.End();
            }
            Response.Redirect("/");
            Response.End();
        }

        #region == 登出 ==
        public void Logout()
        {
            FormsAuthentication.SignOut();

            //删除单点登录Cookie
            Response.Cookies.Add(new HttpCookie("ht",string.Empty) { Domain = FormsAuthentication.CookieDomain, Expires = DateTime.Now.AddMonths(-1) });
            Response.Cookies.Add(new HttpCookie("hr", string.Empty) { Domain = FormsAuthentication.CookieDomain, Expires = DateTime.Now.AddMonths(-1) });
            
            //判断是否是合法的返回地址
            string returnUrl = CECRequest.GetQueryString("returnUrl");
            ToRedirectHome(returnUrl);
        }
        #endregion

        #region == 注册 ==
        public ActionResult Register()
        {
            //判断是否在hite.com.cn域下
            //如果不是，则跳转到Passport下注册
            if (Request.Url.Host.IndexOf(FormsAuthentication.CookieDomain) < 0)
            {
                Response.Redirect(string.Format("{0}/accounts/register?ReturnUrl={1}", System.Configuration.ConfigurationManager.AppSettings["PassportUrl"], HttpUtility.UrlEncode(Request.Url.ToString())));
                Response.End();
            }

            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection fc)
        {
            bool error = false;

            UserInfo model = new UserInfo();
            model.Company = fc["txtCompany"];
            model.Email = fc["txtEmail"];
            model.Industry = fc["ddlIndustry"];
            model.Phone = fc["txtPhone"];
            model.RealName = fc["txtRealName"];
            model.UserName = fc["txtUserName"];
            model.UserPwd = fc["txtUserPwdConfirm"];
            model.SiteId = HiteContext.Current.Site.Id;
            //加密
            model.UserPwd = Controleng.Common.Utils.MD5(model.UserPwd);

            //1,检查用户名是否存在
            if (UserService.IsExistsUserName(model.UserName))
            {
                ModelState.AddModelError("UserName", "该用户名已被使用，请使用其它用户名注册");
                error = true;
                return View();
            }
            if (model.UserName.Length < 4 || model.UserName.Length > 20)
            {
                ModelState.AddModelError("UserName", "用户名长度在4-20个字符内");
                error = true;
                return View();
            }
            if (!Regex.IsMatch(model.UserName, @"^[A-Za-z0-9_\\-]+$", RegexOptions.IgnoreCase))
            {
                ModelState.AddModelError("UserName", "用户名只能由英文、数字及“_”、“-”组成");
                error = true;
                return View();
            }
            //2,检查Email是否存在
            if (UserService.IsExistsEmail(model.Email))
            {
                ModelState.AddModelError("Email", "该Email已被使用，请使用其它Email");
                error = true;
                return View();
            }
            if (!error)
            {
                int id = UserService.Update(model).Id;
                if (id > 0)
                {
                    Login(model.UserName);
                }
                else
                {
                    ViewBag.Msg = 0;
                }
            }

            return View();
        }
        #endregion        

        #region == 申请论坛用户 ==
        [Authorize]
        public ActionResult ForumApply() {
            UserInfo userInfo = UserService.Get(User.Identity.Name);
            ViewBag.ForumGroupList = ForumService.GroupList();

            ViewBag.ApplyList = ForumApplyUserService.ListByUserId(userInfo.Id);

            return View(userInfo);
        }
        [HttpPost]
        public ActionResult ForumApply(FormCollection fc) {
            UserInfo userInfo = UserService.Get(User.Identity.Name);
            ViewBag.ForumGroupList = ForumService.GroupList();
            if (fc["forumGroups[]"] != null)
            {
                string[] forumGroupIds = fc["forumGroups[]"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<ForumApplyUserInfo> modelList = new List<ForumApplyUserInfo>();
                ForumApplyUserInfo model = null;
                foreach (string id in forumGroupIds)
                {
                    int forumGroupId = Controleng.Common.TypeConverter.StrToInt(id, 0);
                    if (forumGroupId > 0)
                    {
                        model = new ForumApplyUserInfo()
                        {
                            UserId = userInfo.Id,
                            UserName = userInfo.UserName,
                            ForumGroupId = forumGroupId,
                            ContactPerson = fc["forumGroup_" + forumGroupId],
                            Status = ForumApplyStatus.Applying,
                            CreateDateTime = DateTime.Now
                        };
                        modelList.Add(model);
                    }
                }
                ForumApplyUserService.Applying(modelList,userInfo.Id);

                ViewBag.Msg = "感谢您提交申请，我们会尽快审核，审核通过后您就可以您申请的事业部论坛浏览、发帖。 ";
            }
            ViewBag.ApplyList = ForumApplyUserService.ListByUserId(userInfo.Id);

            return View(userInfo);
        }
        #endregion
    }
}
