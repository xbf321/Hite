using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class OrderService
    {
        public static OrderInfo Update(OrderInfo model) {
            if (model.Id == 0)
            {
                //Add
                int id = OrderManage.Add(model);
                model.Id = id;
            }
            else { 
                //Update
                OrderManage.Update(model);
            }
            return model;
        }
        public static OrderInfo Get(int id) {
            return OrderManage.Get(id);
        }
        public static bool IsExistsOrderNumber(int id,string orderNumber) {
            return OrderManage.IsExistsOrderNumber(id,orderNumber);
        }
        public static IPageOfList<OrderInfo> List(OrderSearchSetting settings) {
            return OrderManage.List(settings);
        }
    }
}
