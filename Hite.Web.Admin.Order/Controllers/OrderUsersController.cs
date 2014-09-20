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
    public class OrderUsersController : Controller
    {
        //
        // GET: /OrderUser/

        public ActionResult Edit()
        {
            int id = Controleng.Common.CECRequest.GetQueryInt("id",0);
            var model = OrderUserService.Get(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(OrderUserInfo model) {
            bool error = false;
            if (OrderUserService.ExistsUserName(model.Id, model.UserName))
            {
                ModelState.AddModelError("UserNameExists", "用户名存在");
                error = true;
            }
            if (OrderUserService.ExistsCompanyName(model.Id, model.CompanyName))
            {
                ModelState.AddModelError("CompanyNameExists", "公司名已存在");
            }
            if (!error && ModelState.IsValid)
            {
                OrderUserService.Update(model);
                ModelState.AddModelError("Success", "保存成功！");
            }
            
            return View();
        }

        public ActionResult List() {
            int pageIndex = Controleng.Common.CECRequest.GetQueryInt("page",0);
            int pageSize = 10;
            var list = OrderUserService.List(pageIndex,pageSize);
            return View(list);
        }

    }
}
