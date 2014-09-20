using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class UserService
    {
        public static UserInfo Update(UserInfo model) {
            if (model.Id == 0)
            {
                int id = UserManage.Add(model);
                model.Id = id;
            }
            else {
                UserManage.Update(model);
            }
            return model;
        }
        public static IPageOfList<UserInfo> List(int pageIndex, int pageSize) {
            return UserManage.List(pageIndex,pageSize);
        }
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        /// <param name="userId"></param>
        public static void UpdateLastLoginDateTime(int userId) {
            UserManage.UpdateLastLoginDateTime(userId);
        }
        /// <summary>
        /// 验证用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsExistsUserName(string userName) {
            return UserManage.IsExistsUserName(userName);
        }
        /// <summary>
        /// 验证Email是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsExistsEmail(string email) {
            return IsExistsEmail(email,0);
        }
        public static bool IsExistsEmail(string email,int id) {
            return UserManage.IsExistsEmail(email,id);
        }
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        public static bool ValidateUser(string userName, string userPwd) {
            return UserManage.ValidateUser(userName,userPwd);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="newPwd"></param>
        public static void ChangePwd(string userName, string newPwd) {
            UserManage.ChangePwd(userName,newPwd);
        }
        public static UserInfo Get(string userName) {
            return UserManage.Get(userName);
        }
        public static UserInfo Get(int userId) {
            return UserManage.Get(userId);
        }
    }
}
