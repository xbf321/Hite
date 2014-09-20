using System;
using System.Collections.Generic;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ForumGroupInfo
    {
        public int Id { get; set; }
        [DbField(Size = 200)]
        public string Name { get; set; }
        public int Sort { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<ForumInfo> Forums { get; set; }
        public ForumGroupInfo() {
            Name = string.Empty;
            CreateDateTime = DateTime.Now;
        }
        /*
         CREATE TABLE [dbo].[ForumGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Sort] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ForumGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumGroups] ADD  CONSTRAINT [DF_ForumGroups_Name]  DEFAULT ('') FOR [Name]
GO

ALTER TABLE [dbo].[ForumGroups] ADD  CONSTRAINT [DF_ForumGroups_Sort]  DEFAULT ((0)) FOR [Sort]
GO

ALTER TABLE [dbo].[ForumGroups] ADD  CONSTRAINT [DF_ForumGroups_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[ForumGroups] ADD  CONSTRAINT [DF_ForumGroups_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
CREATE TABLE [dbo].[ForumMyReplies](
	[UserId] [int] NOT NULL,
	[TopicId] [int] NOT NULL,
	[ReplyId] [int] NOT NULL,
	[CreateDateTime] [smalldatetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumMyReplies] ADD  CONSTRAINT [DF_ForumMyReplies_UserId]  DEFAULT ((0)) FOR [UserId]
GO

ALTER TABLE [dbo].[ForumMyReplies] ADD  CONSTRAINT [DF_ForumMyReplies_TopicId]  DEFAULT ((0)) FOR [TopicId]
GO

ALTER TABLE [dbo].[ForumMyReplies] ADD  CONSTRAINT [DF_ForumMyReplies_ReplyId]  DEFAULT ((0)) FOR [ReplyId]
GO

ALTER TABLE [dbo].[ForumMyReplies] ADD  CONSTRAINT [DF_ForumMyReplies_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO
         CREATE TABLE [dbo].[ForumMyTopics](
	[UserId] [int] NOT NULL,
	[TopicId] [int] NOT NULL,
	[CreateDateTime] [smalldatetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumMyTopics] ADD  CONSTRAINT [DF_ForumMyTopics_UserId]  DEFAULT ((0)) FOR [UserId]
GO

ALTER TABLE [dbo].[ForumMyTopics] ADD  CONSTRAINT [DF_ForumMyTopics_TopicId]  DEFAULT ((0)) FOR [TopicId]
GO

ALTER TABLE [dbo].[ForumMyTopics] ADD  CONSTRAINT [DF_ForumMyTopics_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO


CREATE TABLE [dbo].[ForumReplies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ForumId] [int] NOT NULL,
	[TopicId] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Poster] [nvarchar](100) NOT NULL,
	[PosterId] [int] NOT NULL,
	[PostDateTime] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Floor] [int] NOT NULL,
 CONSTRAINT [PK_ForumReplies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_ForumId]  DEFAULT ((0)) FOR [ForumId]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_TopicId]  DEFAULT ((0)) FOR [TopicId]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_Content]  DEFAULT ('') FOR [Content]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_Poster]  DEFAULT ('') FOR [Poster]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_PosterId]  DEFAULT ((0)) FOR [PosterId]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_PostDateTime]  DEFAULT (getdate()) FOR [PostDateTime]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[ForumReplies] ADD  CONSTRAINT [DF_ForumReplies_Floor]  DEFAULT ((1)) FOR [Floor]
GO

         CREATE TABLE [dbo].[Forums](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Info] [nvarchar](max) NOT NULL,
	[Topics] [int] NOT NULL,
	[Replies] [int] NOT NULL,
	[LastTopicId] [int] NOT NULL,
	[LastTopic] [nvarchar](max) NOT NULL,
	[LastTopicDateTime] [datetime] NOT NULL,
	[LastReplyId] [int] NOT NULL,
	[LastReply] [nvarchar](max) NOT NULL,
	[LastReplyDateTime] [datetime] NOT NULL,
	[LastUserId] [int] NOT NULL,
	[LastUserName] [nvarchar](max) NOT NULL,
	[Sort] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Forums] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_GroupId]  DEFAULT ((0)) FOR [GroupId]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_Name]  DEFAULT ('') FOR [Name]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_Info]  DEFAULT ('') FOR [Info]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_Topics]  DEFAULT ((0)) FOR [Topics]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_Replies]  DEFAULT ((0)) FOR [Replies]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastTopicId]  DEFAULT ((0)) FOR [LastTopicId]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastTopic]  DEFAULT ('') FOR [LastTopic]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastTopicDateTime]  DEFAULT (((1900)-(1))-(1)) FOR [LastTopicDateTime]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastReplyId]  DEFAULT ((0)) FOR [LastReplyId]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastReply]  DEFAULT ('') FOR [LastReply]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastReplyDateTime]  DEFAULT (((1900)-(1))-(1)) FOR [LastReplyDateTime]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastUserId]  DEFAULT ((0)) FOR [LastUserId]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_LastUserName]  DEFAULT ('') FOR [LastUserName]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_Sort]  DEFAULT ((0)) FOR [Sort]
GO

ALTER TABLE [dbo].[Forums] ADD  CONSTRAINT [DF_Forums_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
CREATE TABLE [dbo].[ForumTopics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ForumId] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Poster] [nvarchar](100) NOT NULL,
	[PosterId] [int] NOT NULL,
	[LastPoster] [nvarchar](100) NOT NULL,
	[LastPosterId] [int] NOT NULL,
	[Views] [int] NOT NULL,
	[Replies] [int] NOT NULL,
	[PostDateTime] [datetime] NOT NULL,
	[LastPostDateTime] [datetime] NOT NULL,
	[Sticky] [int] NOT NULL,
	[Digest] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ForumTopics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'置顶' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ForumTopics', @level2type=N'COLUMN',@level2name=N'Sticky'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加精' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ForumTopics', @level2type=N'COLUMN',@level2name=N'Digest'
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_ForumId]  DEFAULT ((0)) FOR [ForumId]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Title]  DEFAULT ('') FOR [Title]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Content]  DEFAULT ('') FOR [Content]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Poster]  DEFAULT ('') FOR [Poster]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_PosterId]  DEFAULT ((0)) FOR [PosterId]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_LastPoster]  DEFAULT ('') FOR [LastPoster]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_LastPosterId]  DEFAULT ((0)) FOR [LastPosterId]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Views]  DEFAULT ((0)) FOR [Views]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Replies]  DEFAULT ((0)) FOR [Replies]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_PostDateTime]  DEFAULT (getdate()) FOR [PostDateTime]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_LastPostDateTime]  DEFAULT (((1900)-(1))-(1)) FOR [LastPostDateTime]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Sticky]  DEFAULT ((0)) FOR [Sticky]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_Highlight]  DEFAULT ((0)) FOR [Digest]
GO

ALTER TABLE [dbo].[ForumTopics] ADD  CONSTRAINT [DF_ForumTopics_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


         */
    }
}
