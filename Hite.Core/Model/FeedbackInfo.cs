using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    /// <summary>
    /// 反馈
    /// </summary>
    public class FeedbackInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int UserId { get; set; }
        [DbField(Size = 50)]
        public string UserName { get; set; }
        [DbField(Size = 50)]
        public string RealName { get; set; }
        [DbField(Size = 100)]
        public string Company { get; set; }
        [DbField(Size = 50)]
        public string Phone { get; set; }
        [DbField(Size = 50)]
        public string Email { get; set; }
        [DbField(Size = 200)]
        public string Requirement { get; set; }
        public string Intention { get; set; }
        public DateTime CreateDateTime { get; set; }
        [DbField(Size = 20)]
        public string IP { get; set; }
        public FeedbackInfo() {
            UserName = RealName = Company = Phone = Email = Requirement = Intention = IP = string.Empty;
            CreateDateTime = DateTime.Now;
        }
    }
}
