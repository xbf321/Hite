using System;
using System.Web.Mvc;
using Hite.Web.Admin.Order.Models;
using Hite.Model;
using Hite.Services;

namespace Hite.Web.Admin.Order.Controllers
{
    public class AdminController : Controller
    {
        /// <summary>
        /// cookie过期时间
        /// </summary>
        private static readonly int COOKIEEXPIRETIME = 60;
        #region == 登录 ==
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc) {
            bool error = false;
            string userName = fc["txtUserName"];
            string userPwd = fc["txtUserPwd"];
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd)){
                error = true;
                ModelState.AddModelError("UserNameOrUserPwdEmpty","用户名或密码不能为空！");
            }            
            if(!error && ModelState.IsValid){

                //判断用户是否存在
                //判断密码是否正确
                if(!OrderAdminService.IsExistsUserName(userName)){
                    ModelState.AddModelError("UserNameNotExists", "用户名不存在，请重试！");
                }else if(!OrderAdminService.ValidateForLogin(userName, userPwd)){
                    ModelState.AddModelError("UserPwdError", "密码错误，请重试！");
                }else{
                    //正确
                    var userInfo = OrderAdminService.Get(userName);
                    string _cookieValue = string.Format("{0}#{1}",userInfo.UserName,userInfo.RoleType);
                    //MD5加密
                    _cookieValue = Goodspeed.Library.Security.DESCryptography.Encrypt(_cookieValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]);
                    //Write cookie
                    Controleng.Common.Utils.WriteCookie(OrdersAdminContext.LOGINCOOKIEKEY, _cookieValue, COOKIEEXPIRETIME);
                    Response.Redirect("/");
                }
            }
            return View();
        }
        #endregion

        #region == Logout ==
        public void Logout()
        {
            //清空Cookie
            Controleng.Common.Utils.WriteCookie(OrdersAdminContext.LOGINCOOKIEKEY,string.Empty,-1);
            Response.Redirect("/login");
        }
        #endregion

        #region == 管理员设置 ==
        [OrdersAdminAuthorize]
        public ActionResult List() {
            int pageIndex = Controleng.Common.CECRequest.GetQueryInt("page", 0);
            int pageSize = 10;

            var list = OrderAdminService.List(new SearchSetting()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return View(list);
        }
        #endregion

        #region == 删除管理员 ==
        [OrdersAdminAuthorize]
        public void DeleteUser() { 
            //删除用户
            int id = Controleng.Common.CECRequest.GetQueryInt("id",0);
            OrderAdminService.Delete(id);
            Response.Redirect(Request.UrlReferrer.ToString());
            Response.End();

        }
        #endregion

        #region == 恢复管理员 ==
        [OrdersAdminAuthorize]
        public void RestoreUser() { 
            //恢复用户
            int id = Controleng.Common.CECRequest.GetQueryInt("id", 0);
            OrderAdminService.Restore(id);
            Response.Redirect(Request.UrlReferrer.ToString());
            Response.End();
        }
        #endregion

        #region == 设置密码 ==
        [OrdersAdminAuthorize]
        public ActionResult SetPwd() {
            return View();
        }
        [HttpPost]
        public ActionResult SetPwd(FormCollection fc) {
            string oldPwd = fc["txtOldPwd"];
            string newPwd = fc["txtNewPwd"];
            string newConfirmPwd = fc["txtConfirmNewPwd"];
            if(string.IsNullOrEmpty(oldPwd)){
                ModelState.AddModelError("OldPwdEmpty","原密码不能为空");
                return View();
            }
            if(string.IsNullOrEmpty(newPwd)){
                ModelState.AddModelError("NewPwdEmpty","新密码不能为空");
                return View();
            }
            if(newPwd != newConfirmPwd){
                ModelState.AddModelError("NewPwdNotMatch","两次输入的新密码不匹配");
                return View();
            }
            var userInfo = OrderAdminService.Get(OrdersAdminContext.Current.UserName);
            var oldEncrptyPwd = Controleng.Common.Utils.MD5(oldPwd);
            if (oldEncrptyPwd != userInfo.UserPwd) {
                ModelState.AddModelError("OldPwdNotMatch", "旧密码有误，请重试");
                return View();
            }
            OrderAdminService.SetPwd(userInfo.Id,newConfirmPwd);

            ModelState.AddModelError("Success", "修改成功");
            return View();
        }
        #endregion

        #region == 添加管理员 ==
        [OrdersAdminAuthorize]
        public ActionResult Add() {

            return View();
        }
        [HttpPost]
        public ActionResult Add(OrderAdminInfo model,FormCollection fc) {
            bool error = false;
            string roleType = fc["ddlRoles"];
            if (OrderAdminService.IsExistsUserName(model.UserName))
            {
                error = true;
                ModelState.AddModelError("UserNameExists", "用户名已存在，请选择其他用户名！");
            }
            if (string.IsNullOrEmpty(roleType))
            {
                error = true;
                ModelState.AddModelError("RoleType", "请选择管理权限！");
            }      

            if(!error && ModelState.IsValid){                
                model.RoleType = (OrderAdminRoleType)Enum.Parse(typeof(OrderAdminRoleType),roleType);
                OrderAdminService.Add(model);
                ModelState.AddModelError("Success","添加成功！");
            }
            return View();
        }
        #endregion
    }
}
