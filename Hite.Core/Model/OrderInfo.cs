using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class OrderInfo
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings= false,ErrorMessage="请选择客户公司")]
        public int OrderUserId { get; set; }
        /// <summary>
        /// 冗余字段，应该在OroderUser表中，但是为了显示，在放一遍
        /// 如果OrderUsers表中的公司更改，同时更改OrderCompanyName字段
        /// </summary>
        [DbField(Size = 100)]
        public string OrderCompanyName { get; set; }
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入订单号")]
        [RegularExpression(@"^\d+$",ErrorMessage="必须为数字")]
        [DbField(Size = 50)]
        public string OrderNumber { get; set; }
        public string ProductName { get; set; }
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入数量")]
        [RegularExpression(@"^\d+$", ErrorMessage = "必须为数字")]
        public int Amount { get; set; }
        [Required(ErrorMessage="请选择交货日期")]
        public DateTime DeliveryDate { get; set; }
        [Required(ErrorMessage="请选择状态")]
        public OrderStatus Status { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }
        public OrderInfo() {
            OrderCompanyName = OrderNumber = ProductName = Remark = string.Empty;
            CreateDateTime = DeliveryDate = DateTime.Now;
            Amount = 0;
        }

        /*
         USE [www_hite_com_cn]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderUserId] [int] NOT NULL,
	[OrderCompanyName] [nvarchar](100) NOT NULL,
	[OrderNumber] [nvarchar](50) NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[Amount] [int] NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[Remark] [nvarchar](max) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Orders', @level2type=N'COLUMN',@level2name=N'Amount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Orders', @level2type=N'COLUMN',@level2name=N'DeliveryDate'
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_OrderUserId]  DEFAULT ((0)) FOR [OrderUserId]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_OrderCompanyName]  DEFAULT ('') FOR [OrderCompanyName]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_OrderNumber]  DEFAULT ((0)) FOR [OrderNumber]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_ProductName]  DEFAULT ('') FOR [ProductName]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Amount]  DEFAULT ((0)) FOR [Amount]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_DeliveryDate]  DEFAULT (getdate()) FOR [DeliveryDate]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Remark]  DEFAULT ('') FOR [Remark]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_CreateDateTime]  DEFAULT (getdate()) FOR [CreateDateTime]
GO



         */
    }
}
