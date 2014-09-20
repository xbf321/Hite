using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hite.Model;
using Hite.Services;
using Hite.Web.Admin.Order.Models;

namespace Hite.Web.Admin.Order.Controllers
{
    [OrdersAdminAuthorize]
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult Edit()
        {
            int id = Controleng.Common.CECRequest.GetQueryInt("id",0);
            var model = OrderService.Get(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(OrderInfo model,FormCollection fc) {
            bool error = false;
            
            if(OrderService.IsExistsOrderNumber(model.Id,model.OrderNumber)){
                error = true;
                ModelState.AddModelError("ExistsOrderNumber","此订单号已存在，请选择其他订单号！");
            }
            if(!error && ModelState.IsValid){
                model.OrderCompanyName = OrderUserService.Get(model.OrderUserId).CompanyName;
                OrderService.Update(model);
                ModelState.AddModelError("Success","保存成功！");
            }
            return View(model);
        }
        public ActionResult List() {
            int pageIndex = Controleng.Common.CECRequest.GetQueryInt("page",0);
            int pageSize = 10;

            var list = OrderService.List(new OrderSearchSetting() { 
                PageIndex = pageIndex,
                PageSize = pageSize,
                OrderNumber = Controleng.Common.CECRequest.GetQueryString("o"),
                CompanyName = Controleng.Common.CECRequest.GetQueryString("c"),
                ShowDeleted = true
            });

            return View(list);
        }

    }
}
