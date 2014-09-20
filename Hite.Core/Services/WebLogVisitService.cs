using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class WebLogVisitService
    {
        public static void Add(WebLogVisitInfo model) {
            WebLogVisitManage.Add(model);
        }
    }
}
