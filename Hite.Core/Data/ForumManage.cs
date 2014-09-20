/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-09-19 15:01:56
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-09-19 15:01:56
 * Description: 论坛管理
 * ********************************************************************/  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Hite.Model;
using Hite.Common.Reflection;
using Goodspeed.Library.Data;

namespace Hite.Data
{
    internal static class ForumManage
    {
        #region == GroupInfo ==
        public static int AddGroup(ForumGroupInfo model) {
            string strSQL = "INSERT INTO dbo.ForumGroups(Name,Sort,IsDeleted,CreateDateTime) VALUES(@Name,@Sort,@IsDeleted,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void UpdateGroup(ForumGroupInfo model) {
            string strSQL = "UPDATE ForumGroups SET Name = @Name,Sort = @Sort,IsDeleted = @IsDeleted WHERE Id = @ID";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static ForumGroupInfo GetGroup(int id) {
            string strSQL = "SELECT * FROM ForumGroups WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            return GetGroup(dr);
        }
        private static ForumGroupInfo GetGroup(DataRow dr)
        {
            ForumGroupInfo model = new ForumGroupInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static List<ForumGroupInfo> GroupList()
        {
            string strSQL = "SELECT * FROM ForumGroups WITH(NOLOCK) ORDER BY Sort";
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            List<ForumGroupInfo> list = new List<ForumGroupInfo>();
            ForumGroupInfo model = null;
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow item in dt.Rows){
                    model = GetGroup(item);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == ForumInfo ==
        public static ForumInfo Get(int id)
        {
            string strSQL = "SELECT * FROM Forums WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        private static ForumInfo Get(DataRow dr)
        {
            ForumInfo model = new ForumInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static int Add(ForumInfo model) {
            string strSQL = "INSERT INTO Forums(GroupId,Name,Info,Sort,IsDeleted) VALUES(@GroupId,@Name,@Info,@Sort,@IsDeleted);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void Update(ForumInfo model) {
            string strSQL = "UPDATE Forums SET GroupId = @GroupId,Name = @Name,Info = @Info,Sort = @Sort,IsDeleted = @IsDeleted WHERE Id = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static List<ForumInfo> List(int groupId) {
            string strSQL = "SELECT * FROM Forums WITH(NOLOCK) WHERE GroupId = @GroupId ORDER BY Sort";
            SqlParameter parm = new SqlParameter("GroupId",groupId);
            List<ForumInfo> list = new List<ForumInfo>();
            ForumInfo model = null;
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        /// <summary>
        /// 更新主题数（默认+）
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateTopics(int id) {
            UpdateTopics(id,true);
        }
        /// <summary>
        /// 更新主题数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plus">true:+,false:-</param>
        public static void UpdateTopics(int id,bool plus) {
            string strSQL = string.Format("UPDATE Forums SET Topics = Topics {0} 1 WHERE Id = @Id",plus ? "+" : "-");
            SqlParameter parm = new SqlParameter("Id", id);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        /// <summary>
        /// 更新回复数（默认+）
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateReplies(int id) {
            UpdateReplies(id,true);
        }
        /// <summary>
        /// 更新回复数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plus">true:+,false:-</param>
        public static void UpdateReplies(int id, bool plus)
        {
            string strSQL = string.Format("UPDATE Forums SET Replies = Replies {0} 1 WHERE Id = @Id", plus ? "+" : "-");
            SqlParameter parm = new SqlParameter("Id", id);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static void UpdateLastTopic(int id,int topicId,string topicTitle,DateTime postDateTime) {
            string strSQL = "UPDATE Forums SET LastTopic = @LastTopic,LastTopicId =@LastTopicId,LastTopicDateTime = @LastTopicDateTime WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("LastTopic",SqlDbType.VarChar),
                                    new SqlParameter("LastTopicId",SqlDbType.Int),
                                    new SqlParameter("LastTopicDateTime",SqlDbType.DateTime)
                                   };
            parms[0].Value = id;
            parms[1].Value = topicTitle;
            parms[2].Value = topicId;
            parms[3].Value = postDateTime;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static void UpdateLastReply(int id, int replyId, string replyContent, DateTime postDateTime)
        {
            string strSQL = "UPDATE Forums SET LastReply = @LastReply,LastReplyId =@LastReplyId,LastReplyDateTime = @LastReplyDateTime WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("LastReply",SqlDbType.VarChar),
                                    new SqlParameter("LastReplyId",SqlDbType.Int),
                                    new SqlParameter("LastReplyDateTime",SqlDbType.DateTime)
                                   };
            parms[0].Value = id;
            parms[1].Value = replyContent;
            parms[2].Value = replyId;
            parms[3].Value = postDateTime;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static void UpdateLastPoster(int id,int userId,string userName) {
            string strSQL = "UPDATE Forums SET LastPosterId = @LastPosterId,LastPoster =@LastPoster WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("LastPoster",SqlDbType.VarChar),
                                    new SqlParameter("LastPosterId",SqlDbType.Int)
                                   };
            parms[0].Value = id;
            parms[1].Value = userName;
            parms[2].Value = userId;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        #endregion

        #region == Moderator ==
        public static List<ForumModeratorInfo> GetModerators(int forumId = 0) {
            string strSQL = string.Format("SELECT FM.*,F.Name AS ForumName FROM ForumModerators AS FM WITH(NOLOCK) LEFT JOIN Forums AS F WITH(NOLOCK) ON FM.ForumId = F.Id WHERE ForumId = {0}",forumId > 0 ? forumId.ToString() : "ForumId");
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            List<ForumModeratorInfo> list = new List<ForumModeratorInfo>();
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(new ForumModeratorInfo() { 
                        CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                        UserName = dr.Field<string>("UserName"),
                        UserId = dr.Field<int>("UserId"),
                        ForumId = dr.Field<int>("ForumId"),
                        ForumName = dr.Field<string>("ForumName")
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 添加版主
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddModerator(ForumModeratorInfo model) {
            string strSQL = @"INSERT INTO ForumModerators(UserId,UserName,ForumId,CreateDateTime)
                                SELECT @UserId,@UserName,@ForumId,GETDATE() WHERE NOT EXISTS(
	                                SELECT * FROM ForumModerators AS FMB WITH(NOLOCK) 
	                                WHERE @UserId= FMB.UserID AND @ForumId = FMB.ForumId
                                );SELECT @@ROWCOUNT;";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("ForumId",SqlDbType.Int),
                                   };
            parms[0].Value = model.UserId;
            parms[1].Value = model.UserName;
            parms[2].Value = model.ForumId;

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms)) > 0;
        }
        /// <summary>
        /// 删除版主
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forumId"></param>
        public static void DeleteModerator(int userId,int forumId) {
            string strSQL = "DELETE ForumModerators WHERE UserId = @UserId AND ForumId = @ForumId";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("ForumId",SqlDbType.Int)
                                   };
            parms[0].Value = userId;
            parms[1].Value = forumId;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        /// <summary>
        /// 是否是版主
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forumId"></param>
        /// <returns></returns>
        public static bool IsModerator(int userId,int forumId) {
            string strSQL = "SELECT COUNT(*) FROM ForumModerators WITH(NOLOCK) WHERE UserId = @UserId AND ForumId = @ForumId";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("ForumId",SqlDbType.Int)
                                   };
            parms[0].Value = userId;
            parms[1].Value = forumId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms))>0;
        }
        #endregion

    }
}
