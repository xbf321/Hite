using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hite.Common.Reflection
{
    /// <summary>
    /// SqlParameter的操作类
    /// </summary>
    public class ParameterHelper
    {
        #region = JoinSqlParameter =
        /// <summary>
        /// 合并SqlParameter数组
        /// </summary>
        /// <param name="oldparms">原SqlParameter数组</param>
        /// <param name="newparms">新SqlParameter数组</param>
        /// <returns>合并后的SqlParameter数组</returns>
        public static List<SqlParameter> JoinSqlParameter(List<SqlParameter> oldparms, List<SqlParameter> newparms)
        {
            if (newparms != null && newparms.Count != 0)
            {
                foreach (SqlParameter pa in newparms)
                {
                    if (oldparms.Contains(pa) == false)
                    {
                        oldparms.Add(pa);
                    }
                }
            }
            return oldparms;
        }

        #endregion

        #region = ConvertToListSqlParameter =
        /// <summary>
        /// SqlParameter[]转换成 List SqlParameter
        /// </summary>
        /// <param name="sqlParms">SqlParameter[]参数</param>
        /// <returns>List SqlParameter</returns>
        public static List<SqlParameter> ConvertToListSqlParameter(SqlParameter[] sqlParms)
        {
            List<SqlParameter> parms = new List<SqlParameter>();

            if (sqlParms != null && sqlParms.Length > 0)
            {
                foreach (SqlParameter p in sqlParms)
                {
                    parms.Add(p);
                }
            }

            return parms;
        }

        #endregion

        #region = GetClassSqlParameters =
        /// <summary>
        /// 通过Model类获取属性的SqlParameter数组
        /// </summary>
        /// <param name="classObject">Model类</param>
        /// <returns>SqlParameter数组</returns>
        public static SqlParameter[] GetClassSqlParameters(object classObject)
        {
            return GetClassSqlParameters(classObject, false);
        }

        /// <summary>
        /// 通过Model类获取属性的SqlParameter数组
        /// </summary>
        /// <param name="classObject">Model类</param>
        /// <param name="isOnlyDbField">是否只反射仅包含DbField标签的属性</param>
        /// <returns>SqlParameter数组</returns>
        public static SqlParameter[] GetClassSqlParameters(object classObject,bool isOnlyDbField)
        {
            var sqlParms = new List<SqlParameter>();
            var oType = classObject.GetType();
            foreach (var mi in oType.GetMembers())
            {
                if (mi.MemberType != MemberTypes.Property) continue;
                var columnAttributes = mi.GetCustomAttributes(typeof(DbFieldAttribute), false);
                if (columnAttributes != null && columnAttributes.Length > 0)
                {
                    foreach (DbFieldAttribute attr in columnAttributes)
                    {
                        var p = (PropertyInfo)mi;
                        object value = p.GetValue(classObject, null);
                        value = value == null ? string.Empty : value;

                        var parm =
                            new SqlParameter(
                                string.Concat("@", string.IsNullOrEmpty(attr.Name) ? p.Name : attr.Name),value);
                        if (attr.Size > 0)
                            parm.Size = attr.Size;
                        if (sqlParms.Contains(parm) == false)
                        {
                            sqlParms.Add(parm);
                        }
                    }
                    continue;
                }
                if(isOnlyDbField) continue;
                var prop = (PropertyInfo)mi;
                var parm_ = new SqlParameter(string.Concat("@", prop.Name), prop.GetValue(classObject, null));
                if (sqlParms.Contains(parm_) == false)
                {
                    sqlParms.Add(parm_);
                }
            }
            return sqlParms.ToArray();
        }

        #endregion

        #region = GetClassOleDbParameters =

        /// <summary>
        /// 通过Model类获取属性的OleDbParameter数组
        /// </summary>
        /// <param name="classObject">Model类</param>
        /// <returns>OleDbParameter数组</returns>
        public static OleDbParameter[] GetClassOleDbParameters(object classObject)
        {
            Hashtable ht = new Hashtable();
            Type oType = classObject.GetType();
            foreach (MemberInfo mi in oType.GetMembers())
            {
                if (mi.MemberType != MemberTypes.Property) continue;
                object[] columnAttributes = mi.GetCustomAttributes(typeof(DbFieldAttribute), false);
                if (columnAttributes != null && columnAttributes.Length > 0)
                {
                    foreach (DbFieldAttribute attr in columnAttributes)
                    {
                        var p = (PropertyInfo)mi;

                        if (ht.ContainsKey(attr.Name) == false)
                        {
                            ht.Add(attr.Name, p.GetValue(classObject, null));
                        }
                    }
                }
                else
                {
                    PropertyInfo p = (PropertyInfo)mi;
                    if (ht.ContainsKey(p.Name) == false)
                    {
                        ht.Add(p.Name, p.GetValue(classObject, null));
                    }
                }
            }
            List<OleDbParameter> sqlParms = new List<OleDbParameter>();
            foreach (object column in ht.Keys)
            {
                OleDbParameter parm = new OleDbParameter("@" + (string)column, ht[column]);
                if (sqlParms.Contains(parm) == false)
                {
                    sqlParms.Add(parm);
                }
            }
            return sqlParms.ToArray();
        }

        #endregion
    }
}
