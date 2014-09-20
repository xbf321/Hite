
namespace Hite.Model
{
    public class AdminInRoles
    {
        public int RoleId { get; set; }
        public int AdminId { get; set; }

        /*
         CREATE TABLE [dbo].[AdminInRoles](
	[AdminId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_AdminInRoles_1] PRIMARY KEY CLUSTERED 
(
	[AdminId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

         */
    }
}
