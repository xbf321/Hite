
namespace Hite.Model
{
    public class RoleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SiteId { get; set; }

        /*
         CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SiteId] [int] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_Name]  DEFAULT ('') FOR [Name]
GO

ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_SiteId]  DEFAULT ((0)) FOR [SiteId]
GO
         */
    }
}
