using System;

namespace Hite.Model
{
    public class ForumApplyUserInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ForumGroupId { get; set; }
        public string ForumGroupName { get; set; }
        public string ContactPerson { get; set; }
        public ForumApplyStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }
        /*
         CREATE TABLE [dbo].[ForumApplyUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[ForumGroupId] [int] NOT NULL,
	[ContactPerson] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ForumApplyUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_UserId]  DEFAULT ((0)) FOR [UserId]
GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_UserName]  DEFAULT ('') FOR [UserName]
GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_ForumGroupId]  DEFAULT ((0)) FOR [ForumGroupId]
GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_ContactPerson]  DEFAULT ('') FOR [ContactPerson]
GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[ForumApplyUsers] ADD  CONSTRAINT [DF_ForumApplyUsers_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
         */
    }
}
