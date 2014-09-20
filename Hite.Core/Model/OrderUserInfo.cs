using System;
using Hite.Common.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Hite.Model
{
    public class OrderUserInfo
    {
        public int Id { get; set; }
        [DbField(Size = 50)]
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入用户名")]
        public string UserName { get; set; }
        [DbField(Size = 50)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入密码")]
        public string UserPwd { get; set; }
        [DbField(Size = 100)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入公司名")]
        public string CompanyName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public OrderUserInfo() {
            UserName = UserPwd = CompanyName = string.Empty;
            CreateDateTime = DateTime.Now;
        }

        /*
         CREATE TABLE [dbo].[OrderUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserPwd] [nvarchar](50) NOT NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_OrderUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OrderUsers] ADD  CONSTRAINT [DF_OrderUsers_UserName]  DEFAULT ('') FOR [UserName]
GO

ALTER TABLE [dbo].[OrderUsers] ADD  CONSTRAINT [DF_OrderUsers_UserPwd]  DEFAULT ('') FOR [UserPwd]
GO

ALTER TABLE [dbo].[OrderUsers] ADD  CONSTRAINT [DF_OrderUsers_CompanyName]  DEFAULT ('') FOR [CompanyName]
GO

ALTER TABLE [dbo].[OrderUsers] ADD  CONSTRAINT [DF_OrderUsers_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO



         */
    }
}
