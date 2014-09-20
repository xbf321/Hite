using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Hite.Common.Reflection;
using Hite.Model;

namespace Hite.Data
{
    internal static class SiteManage
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(SiteInfo model)
        {
            string strSQL = "INSERT INTO Sites(Name,Url,WebTitle,WebKeywords,WebDesc,WebLanguage,CreateDateTime,Introduction,ContactUs,Logo,IndexFileName,ThirdCode) VALUES(@Name,@Url,@WebTitle,@WebKeywords,@WebDesc,@WebLanguage,GETDATE(),@Introduction,@ContactUs,@Logo,@IndexFileName,@ThirdCode);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(SiteInfo model)
        {
            string strSQL = "UPDATE Sites SET Name = @Name,Url = @Url,WebTitle = @WebTitle,WebKeywords = @Webkeywords,WebDesc = @WebDesc,WebLanguage = @WebLanguage,Introduction = @Introduction,ContactUs =@ContactUs ,Logo = @Logo,IndexFileName = @IndexFileName, ThirdCode = @ThirdCode WHERE ID = @ID";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static IList<SiteInfo> List() {
            IList<SiteInfo> list = new List<SiteInfo>();
            SiteInfo model = null;
            string strSQL = "SELECT * FROM Sites WITH(NOLOCK)";
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        public static SiteInfo Get(int id) {
            string strSQL = string.Format("SELECT * FROM Sites WITH(NOLOCK) WHERE Id = {0}", id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL));
        }
        public static SiteInfo Get(DataRow dr)
        {
            SiteInfo model = new SiteInfo();
            ReflectionHelper.Fill(dr,model);
            if (model.Id > 0)
            {
                model.Language = (WebLanguage)Enum.Parse(typeof(WebLanguage), dr.Field<byte>("WebLanguage").ToString());
            }
            return model;
        }
    }
}
