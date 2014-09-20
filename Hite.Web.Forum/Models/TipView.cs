using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Hite.Web.Forum.Models
{
    public class TipView : ViewResultBase
    {
        public string Msg { get; set; }
        public string Url { get; set; }
        public bool Success { get; set; }
        private new string ViewName { get { return "Tip"; } }

        protected override ViewEngineResult FindView(ControllerContext context)
        {
            ViewEngineResult result = ViewEngineCollection.FindView(context,ViewName, string.Empty);
            if (result.View != null)
            {
                return result;
            }
            return null;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            ViewEngineResult result = null;

            if (View == null)
            {
                result = FindView(context);
                View = result.View;
            }

            TextWriter writer = context.HttpContext.Response.Output;
            if(string.IsNullOrEmpty(Url)){
                Url = "/";
            }
            ViewContext viewContext = new ViewContext(context, View, new ViewDataDictionary(new TipModel() { Msg = Msg,Url = Url,Success = Success }), TempData, writer);
            View.Render(viewContext, writer);

            if (result != null)
            {
                result.ViewEngine.ReleaseView(context, View);
            }
        }
    }
}