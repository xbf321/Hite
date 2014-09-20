using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;
using System.Web.Mvc;

namespace Hite.Services
{
    public static class OrderUserService
    {
        public static OrderUserInfo Update(OrderUserInfo model) {
            if (model.Id == 0)
            {
                //Add
                int id = OrderUserManage.Add(model);
                model.Id = id;
            }
            else {
                OrderUserManage.Update(model);
            }
            return model;
        }
        public static OrderUserInfo Get(int id)
        {
            return OrderUserManage.Get(id);   
        }
        public static bool ExistsUserName(int id, string userName) {
            return OrderUserManage.ExistsUserName(id,userName);
        }
        public static bool ExistsCompanyName(int id, string companyName) {
            return OrderUserManage.ExistsCompanyName(id,companyName);
        }
        public static IPageOfList<OrderUserInfo> List(int pageIndex, int pageSize) {
            return OrderUserManage.List(pageIndex,pageSize);
        }
        public static OrderUserInfo Get(string userName, string userPwd) {
            return OrderUserManage.Get(userName,userPwd);
        }
        public static IEnumerable<SelectListItem> GetCompanyNameSelectListItem() {
            var list = OrderUserManage.CompanyList();
            var listItem = new List<SelectListItem>();
            foreach(var item in list){
                listItem.Add(new SelectListItem() { 
                    Text = item.Item2,
                    Value = item.Item1.ToString()
                });
            }
            return listItem;
        }
    }
}
