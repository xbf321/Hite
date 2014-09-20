using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class OrderAdminService
    {
        public static int Add(OrderAdminInfo model) {
            model.UserPwd = Controleng.Common.Utils.MD5(model.UserPwd);
            return OrderAdminManage.Add(model);
        }
        public static bool IsExistsUserName(string userName){
            return OrderAdminManage.IsExistsUserName(userName);
        }
        public static IPageOfList<OrderAdminInfo> List(SearchSetting settings) {
            return OrderAdminManage.List(settings);
        }
        public static bool ValidateForLogin(string userName, string userPwd) {
            userPwd = Controleng.Common.Utils.MD5(userPwd);
            return OrderAdminManage.ValidateForLogin(userName,userPwd);
        }
        public static OrderAdminInfo Get(string userName) {
            return OrderAdminManage.Get(userName);
        }
        public static void SetPwd(int userId, string newPwd) {
            newPwd = Controleng.Common.Utils.MD5(newPwd);
            OrderAdminManage.SetPwd(userId,newPwd);
        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id) {
            OrderAdminManage.Delete(id);
        }
        /// <summary>
        /// 还原管理员
        /// </summary>
        /// <param name="id"></param>
        public static void Restore(int id) {
            OrderAdminManage.Restore(id);
        }
    }
}
