using System;
using Hite.Common.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Hite.Model
{
    public class OrderAdminInfo
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入用户名")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email格式不正确")]
        [DbField(Size = 50)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入密码")]
        [DbField(Size = 50)]
        public string UserPwd { get; set; }

        public OrderAdminRoleType RoleType { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDateTime { get; set; }

        public OrderAdminInfo() {
            UserPwd = UserName = string.Empty;
            CreateDateTime = DateTime.Now;
        }
    }
}
