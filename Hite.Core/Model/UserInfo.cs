using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class UserInfo
    {
        public int Id { get; set; }
        [DbField(Size = 20)]
        public string RealName { get; set; }
        public int SiteId { get; set; }
        [DbField(Size=50)]
        public string UserName { get; set; }
        [DbField(Size = 50)]
        public string UserPwd { get; set; }
        [DbField(Size=50)]
        public string Email { get; set; }
        
        [DbField(Size=100)]
        public string Company { get; set; }
        [DbField(Size=50)]
        public string Phone { get; set; }
        [DbField(Size=50)]
        public string Industry { get; set; }
        [DbField(Size = 1000)]
        public string Avatar { get; set; }
        public DateTime LastLoginDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public bool OnlineState { get; set; }
        public DateTime CreateDateTime { get; set; }
        public UserInfo() {
            
            UserPwd = Email = RealName = Company = Phone = Industry = Avatar = UserName = string.Empty;
            RealName = string.Empty;
            CreateDateTime = LastLoginDateTime = ModifyDateTime = DateTime.Now;
        }
    }
}
