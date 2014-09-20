using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class AdminService
    {
        public static IPageOfList<AdminInfo> List(SearchSetting settings) {
            return AdminManage.List(settings);
        }
        public static bool IsExistsUser(string userName) {
            return AdminManage.IsExistsUser(userName);
        }
        public static AdminInfo Update(AdminInfo model) {
            model.UserPwd = Controleng.Common.Utils.MD5(model.UserPwd);
            if (model.Id == 0)
            {
                int id = AdminManage.Add(model);
                model.Id = id;
            }
            else {
                AdminManage.Update(model);
            }
            return model;
        }
        public static AdminInfo Get(string userName) {
            return Get(userName,false);
        }
        public static AdminInfo Get(string userName, bool isLoadRoles) {
            return AdminManage.Get(userName,isLoadRoles);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isLoadRoles">是否加载角色</param>
        /// <returns></returns>
        public static AdminInfo Get(int id,bool isLoadRoles) {
            return AdminManage.Get(id,isLoadRoles);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdminInfo Get(int id) {
            return Get(id,false);
        }
        /// <summary>
        /// 重设密码
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="pwd"></param>
        public static void SetPwd(int adminId, string pwd) {
            string userPwd = Controleng.Common.Utils.MD5(pwd);
            AdminManage.SetPwd(adminId,userPwd);
        }
    }
}
