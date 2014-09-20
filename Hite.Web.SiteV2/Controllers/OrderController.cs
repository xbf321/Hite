/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-17 16:32:10
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-17 16:32:10
 * Description: 订单信息，只有海得成套站点有，其他的站点没有
 * ********************************************************************/  
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Hite.Model;
using Hite.Mvc;
using Hite.Services;
using Hite.Common;
using Hite.Mvc.Filters;

namespace Hite.Web.Controllers.Site
{
    [SilenceHandleError]
    public class OrderController : HiteController
    {
        public ActionResult Index()
        {
            /*
             继承/Views/Shared/_ChannelLayout.cshtml必须要加的参数
             */
            SiteInfo currentSiteInfo = HiteContext.Current.Site;

            if(currentSiteInfo.IndexFileName != IndexFileNameOfSite.Distribution){
                Response.Redirect("/");
            }

            int currentCatId = 339; //订单类别ID

            //对_ChannelLayout.cshtml的变量赋值
            CategoryInfo catInfo = CategoryService.Get(currentCatId);
            ViewBag.RootCategoryInfo = GetRootCategoryInfo(currentSiteInfo, catInfo);
            ViewBag.CurrentCategoryInfo = catInfo;

            ViewBag.Title = catInfo.Name;

            return View();
        }
        [HttpPost]
        public ActionResult GetListForAjax(string userName,string userPwd) {
            OrderUserInfo orderUserInfo = OrderUserService.Get(userName,userPwd);
            if(orderUserInfo.Id == 0){
                return Json(new { login = false, orders = new List<OrderInfo>() });
            }

            var orders = OrderService.List(new OrderSearchSetting()
            {
                PageIndex = 0,
                PageSize = 1000,
                ShowDeleted = false,
                OrderUserId = orderUserInfo.Id
            }).Select((m,index) => new { 
                OrderNumber = m.OrderNumber,
                ProductName = m.ProductName,
                Amount = m.Amount,
                DeliveryDate = m.DeliveryDate.ToString("yyyy-MM-dd"),
                Status = EnumHelper.GetEnumDescription(m.Status),
                Remark = m.Remark,
                Index = index
            });

            return Json(new { login = true,orders = orders});
        }

    }
}
