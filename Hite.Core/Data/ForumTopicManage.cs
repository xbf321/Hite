using System;
using System.Data;
using System.Data.SqlClient;

using Hite.Model;
using Hite.Common.Reflection;
using Goodspeed.Library.Data;
using Hite.Common;
using System.Text;
using System.Collections.Generic;

namespace Hite.Data
{
    internal static class ForumTopicManage
    {
        #region == Topic ==
        public static IPageOfList<ForumTopicInfo> TopicList(ForumSearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "ForumTopics";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.AppendFormat("  ForumId = @ForumId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "LastPostDateTime DESC";

            SqlParameter[] parms = { 
                                    new SqlParameter("@ForumId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.ForumId;

            IList<ForumTopicInfo> list = new List<ForumTopicInfo>();
            ForumTopicInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = GetTopicInfo(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = TopicListCount(settings);
            return new PageOfList<ForumTopicInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int TopicListCount(ForumSearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM ForumTopics WITH(NOLOCK) WHERE 1 = 1");

            sbCondition.AppendFormat("  AND ForumId = @ForumId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }

            SqlParameter[] parms = { 
                                    new SqlParameter("@ForumId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.ForumId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
        public static ForumTopicInfo GetTopicInfo(int id)
        {
            string strSQL = "SELECT * FROM ForumTopics WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return GetTopicInfo(dr);
        }
        private static ForumTopicInfo GetTopicInfo(DataRow dr)
        {
            ForumTopicInfo model = new ForumTopicInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static int PostTopic(ForumTopicInfo model) {
            string strSQL = "INSERT INTO dbo.ForumTopics(ForumId,Title,Content,Poster,PosterId,Sticky,[Digest],IsDeleted,PostDateTime,LastPostDateTime) VALUES(@ForumId,@Title,@Content,@Poster,@PosterId,@Sticky,@Digest,@IsDeleted,GETDATE(),GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void UpdateTopic(ForumTopicInfo model) {
            string strSQL = "UPDATE ForumTopics SET ForumId = @ForumId,Title = @Title ,Content = @Content,Sticky = @Sticky,[Digest] = @Digest,IsDeleted = @IsDeleted WHERE Id = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static void UpdateTopicViewsCount(int id) {
            string strSQL = "UPDATE ForumTopics SET [Views] = [Views] + 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);
        }
        public static void UpdateTopicRepliesCount(int id) {
            UpdateTopicRepliesCount(id,true);
        }
        private static void UpdateTopicRepliesCount(int id,bool plus) {
            string strSQL = string.Format("UPDATE ForumTopics SET Replies = Replies {0} 1 WHERE Id = @Id",plus? "+" : "-");
            SqlParameter parm = new SqlParameter("Id", id);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static void UpdateTopicLastPoster(int id,int userId,string userName) {
            string strSQL = "UPDATE ForumTopics SET LastPoster = @LastPoster,LastPosterId = @LastPosterId,LastPostDateTime = GETDATE() WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("LastPoster",SqlDbType.VarChar),
                                    new SqlParameter("LastPosterId",SqlDbType.Int)
                                   };
            parms[0].Value = id;
            parms[1].Value = userName;
            parms[2].Value = userId;
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        public static void DeleteTopic(int topicId) {
            string strSQL = "UPDATE ForumTopics SET IsDeleted = 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", topicId);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static void RestoreTopic(int topicId) {
            string strSQL = "UPDATE ForumTopics SET IsDeleted = 0 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", topicId);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        #endregion

        #region == Reply ==
        public static IPageOfList<ForumReplyInfo> ReplyList(ForumSearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "ForumReplies";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.AppendFormat("  TopicId = @TopicId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "PostDateTime ASC";

            SqlParameter[] parms = { 
                                    new SqlParameter("@TopicId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.TopicId;

            IList<ForumReplyInfo> list = new List<ForumReplyInfo>();
            ForumReplyInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = GetReplyInfo(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = ReplyListCount(settings);
            return new PageOfList<ForumReplyInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int ReplyListCount(ForumSearchSetting settings) {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM ForumReplies WITH(NOLOCK) WHERE 1 = 1");

            sbCondition.AppendFormat("  AND TopicId = @TopicId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }

            SqlParameter[] parms = { 
                                    new SqlParameter("@TopicId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.TopicId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
        public static void UpdateReplyDeleted(int replyId)
        {
            string strSQL = "UPDATE ForumReplies SET IsDeleted = ABS(IsDeleted - 1) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", replyId);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static ForumReplyInfo GetReplyInfo(int replyId) {
            string strSQL = "SELECT * FROM ForumReplies WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", replyId);
            return GetReplyInfo(SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        private static ForumReplyInfo GetReplyInfo(DataRow dr)
        {
            ForumReplyInfo model = new ForumReplyInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static int PostReply(ForumReplyInfo model) {
            string strSQL = "INSERT INTO ForumReplies(ForumId,TopicId,Content,Poster,PosterId,PostDateTime,IsDeleted,Floor) VALUES(@ForumId,@TopicId,@Content,@Poster,@PosterId,GETDATE(),0,@Floor);SELECT @@IDENTITY; ";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void UpdateReply(ForumReplyInfo model)
        {
            string strSQL = "UPDATE ForumReplies SET Content = @Content WHERE Id = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        #endregion

        #region == MyTopic ==
        public static void AddMyTopic(int userId,int topicId) {
            if (userId == 0 || topicId == 0) { return; }
            string strSQL = "INSERT INTO ForumMyTopics(UserId,TopicId,CreateDateTime) VALUES(@UserId,@TopicId,GETDATE())";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("TopicId",SqlDbType.Int)
                                   };
            parms[0].Value = userId;
            parms[1].Value = topicId;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        #endregion

        #region == MyReply ==
        public static void AddMyReply(int userId,int topicId,int replyId) {
            if (userId == 0 || topicId == 0 || replyId == 0) { return; }
            string strSQL = "INSERT INTO ForumMyReplies(UserId,TopicId,ReplyId,CreateDateTime) VALUES(@Userid,@TopicId,@ReplyId,GETDATE())";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("TopicId",SqlDbType.Int),
                                    new SqlParameter("ReplyId",SqlDbType.Int)
                                   };
            parms[0].Value = userId;
            parms[1].Value = topicId;
            parms[2].Value = replyId;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        #endregion
    }
}
