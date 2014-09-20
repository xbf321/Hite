using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using System.Data.SqlClient;
using System.Data;
using Hite.Common.Reflection;
using Goodspeed.Library.Data;
using Hite.Common;

namespace Hite.Data
{
    internal static class OrderManage
    {
        public static int Add(OrderInfo model) {
            string strSQL = "INSERT INTO Orders(OrderUserId,OrderNumber,ProductName,Amount,DeliveryDate,[Status],Remark,IsDeleted,CreateDateTime,OrderCompanyName) VALUES(@OrderUserId,@OrderNumber,@ProductName,@Amount,@DeliveryDate,@Status,@Remark,@IsDeleted,GETDATE(),@OrderCompanyName);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void Update(OrderInfo model) {
            string strSQL = "UPDATE Orders SET OrderUserId = @OrderUserId,OrderCompanyName = @OrderCompanyName,OrderNumber = @OrderNumber,ProductName = @ProductName,Amount = @Amount,DeliveryDate = @DeliveryDate,[Status] = @Status,Remark = @Remark,IsDeleted = @IsDeleted WHERE ID = @ID;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static OrderInfo Get(int id)
        {
            string strSQL = "SELECT * FROM Orders WITH(NOLOCK) WHERE Id = @ID";
            SqlParameter parm = new SqlParameter("ID", id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        public static OrderInfo Get(DataRow dr)
        {
            OrderInfo model = new OrderInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static bool IsExistsOrderNumber(int id,string orderNumber) {
            string strSQL = "SELECT COUNT(*) FROM Orders WITH(NOLOCK) WHERE OrderNumber = @OrderNumber";
            if (id > 0)
            {
                strSQL += string.Format("  AND ID <> {0}", id);
            }
            SqlParameter parm = new SqlParameter("OrderNumber", orderNumber);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parm)) > 0;
        }
        public static IPageOfList<OrderInfo> List(OrderSearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "Orders";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder(" 1 = 1");
            
            if (!string.IsNullOrEmpty(settings.CompanyName))
            {
                sbCondition.Append("    AND OrderCompanyName LIKE '%'+@CompanyName+'%'");
            }
            if (!string.IsNullOrEmpty(settings.OrderNumber))
            {
                sbCondition.Append("    AND OrderNumber = @OrderNumber");
            }
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "  CreateDateTime DESC";

            SqlParameter[] parms = { 
                                    new SqlParameter("@CompanyName",SqlDbType.NVarChar),
                                    new SqlParameter("@OrderNumber",SqlDbType.NVarChar)
                                   };
            parms[0].Value = settings.CompanyName;
            parms[1].Value = settings.OrderNumber;

            IList<OrderInfo> list = new List<OrderInfo>();
            OrderInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = Count(settings);
            return new PageOfList<OrderInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count(OrderSearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM Orders WITH(NOLOCK) WHERE 1 = 1");

            if (!string.IsNullOrEmpty(settings.CompanyName))
            {
                sbCondition.Append("    AND OrderCompanyName LIKE '%'+@CompanyName+'%'");
            }
            if (!string.IsNullOrEmpty(settings.OrderNumber))
            {
                sbCondition.Append("    AND OrderNumber = @OrderNumber");
            }
            if (settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }

            SqlParameter[] parms = { 
                                    new SqlParameter("@CompanyName",SqlDbType.NVarChar),
                                    new SqlParameter("@OrderNumber",SqlDbType.NVarChar)
                                   };
            parms[0].Value = settings.CompanyName;
            parms[1].Value = settings.OrderNumber;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
    }
}
