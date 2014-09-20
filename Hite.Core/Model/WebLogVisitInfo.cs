using System;

namespace Hite.Model
{
    public class WebLogVisitInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Url { get; set; }
        public string Referrer { get; set; }
        public string Querys { get; set; }
        public DateTime VisitTime { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public string OS { get; set; }
        public string Brower { get; set; }
        public string UserName { get; set; }

        public WebLogVisitInfo() {
            Url = Referrer = Querys = IP = UserAgent= OS = UserName = Brower = string.Empty;
            VisitTime = DateTime.Now;
        }
    }
}
