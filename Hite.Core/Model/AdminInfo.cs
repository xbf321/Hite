using System;
using System.Collections.Generic;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<RoleInfo> Roles { get; set; }
        public AdminInfo() {
            UserName = UserPwd = string.Empty;
            CreateDateTime = DateTime.Now;
        }
        /*
CREATE TABLE [dbo].[Admins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserPwd] [nvarchar](50) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Admins] ADD  CONSTRAINT [DF_Admins_UserName]  DEFAULT ('') FOR [UserName]
GO

ALTER TABLE [dbo].[Admins] ADD  CONSTRAINT [DF_Admins_UserPwd]  DEFAULT ('') FOR [UserPwd]
GO

ALTER TABLE [dbo].[Admins] ADD  CONSTRAINT [DF_Admins_IsEnabled]  DEFAULT ((1)) FOR [IsEnabled]
GO

ALTER TABLE [dbo].[Admins] ADD  CONSTRAINT [DF_Admins_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[Admins] ADD  CONSTRAINT [DF_Admins_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO



         */
    }
}
