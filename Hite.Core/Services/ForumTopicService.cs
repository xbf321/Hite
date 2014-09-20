using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class ForumTopicService
    {
        /// <summary>
        /// 主题列表
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IPageOfList<ForumTopicInfo> TopicList(ForumSearchSetting settings) {
            var list =  ForumTopicManage.TopicList(settings);
            foreach(var item in list){
                LoadTopicExtensionInfo(item);
            }
            return list;
        }
        private static void LoadTopicExtensionInfo(ForumTopicInfo item) {
            item.PostDateTimeString = TransferDateTimeDiffToHtml(item.PostDateTime);//在这里做时间处理
            //
            LoadTopicFolder(item);
            if (item.LastPosterId == 0)
            {
                item.LastPostDateTimeString = "--";
            }
            else
            {
                item.LastPostDateTimeString = TransferDateTimeDiffCorrectToSecond(item.LastPostDateTime);   //同上
            }
        }
        /// <summary>
        /// 处理帖子前面那个文件夹图片
        /// </summary>
        /// <param name="topicInfo"></param>
        private static void LoadTopicFolder(ForumTopicInfo topicInfo) {
            LoadTopicFolder(15,topicInfo);
        }
        private static void LoadTopicFolder(int hotReplyNumber,ForumTopicInfo topicInfo) {
            if (DateTime.Parse(DateTime.Now.ToShortDateString()) == DateTime.Parse(topicInfo.LastPostDateTime.ToShortDateString()))
            {
                topicInfo.Folder = "new";
            }
            else {
                topicInfo.Folder = "old";
            }
            ///没有图片，暂时没用
            //if(topicInfo.Replies > hotReplyNumber){
            //    topicInfo.Folder += "hot"; 
            //}
        }
        /// <summary>
        /// 回帖列表
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IPageOfList<ForumReplyInfo> ReplyList(ForumSearchSetting settings) {
            var list = ForumTopicManage.ReplyList(settings);
            foreach(var item in list){
                item.PostDateTimeString = TransferDateTimeDiffCorrectToSecond(item.PostDateTime);
            }
            return list;
        }
        /// <summary>
        /// 转换时间为用户
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string TransferDateTimeDiffToHtml(DateTime time)
        {
            string _html = string.Empty;
            var nowDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            var yesterDayDate = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
            var postDate = DateTime.Parse(time.ToShortDateString()); 
            if(nowDate == postDate){
                _html = "<font color=\"red\">今天发表</font>";
            }
            else if (postDate == yesterDayDate)
            {
                _html = "<font color=\"blue\">昨天发表</font>";
            }
            else {
                _html = time.ToString("yyyy/MM/dd");
            }
            return _html;
        }
        public static string TransferDateTimeDiffCorrectToSecond(DateTime time)
        {
            string _html = string.Empty;
            int _seconds = Controleng.Common.Utils.StrDateDiffSeconds(time.ToString(), 0);
            if (_seconds < 60)
            {
                _html = string.Format("{0}秒前", _seconds);
            }
            else
            {
                int _m = Controleng.Common.Utils.StrDateDiffMinutes(time.ToString(), 0);
                if (_m < 60)
                {
                    _html = string.Format("{0}分钟前", _m);
                }
                else
                {
                    int _h = Controleng.Common.Utils.StrDateDiffHours(time.ToString(), 0);
                    if (_h < 24)
                    {
                        _html = string.Format("{0}小时前", _h);
                    }
                    else
                    {
                        int _day = _h / 24;
                        if (_day < 8)
                        {
                            switch (_day)
                            {
                                case 1:
                                    _html = string.Format("昨天 {0}", time.ToString("HH:mm")); ;
                                    break;
                                case 2:
                                    _html = string.Format("前天 {0}", time.ToString("HH:mm"));
                                    break;
                                default:
                                    _html = string.Format("{0}天前 {1}", _day, time.ToString("HH:mm"));
                                    break;
                            }
                        }
                        else
                        {
                            _html = time.ToString("yyyy/MM/dd");
                        }

                    }
                }
            }
            return _html;
        }
        /// <summary>
        /// 获得帖子信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ForumTopicInfo Get(int id) {
            var topicInfo = ForumTopicManage.GetTopicInfo(id);
            topicInfo.PostDateTimeString = TransferDateTimeDiffCorrectToSecond(topicInfo.PostDateTime);
            return topicInfo;
        }
        /// <summary>
        /// 发表或编辑帖子
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ForumTopicInfo PostTopic(ForumTopicInfo model) {
            if (model.Id == 0)
            {
                //添加
                model.Id = ForumTopicManage.PostTopic(model);

                //Forums表
                ForumManage.UpdateTopics(model.ForumId); //主题数+1
                ForumManage.UpdateLastTopic(model.ForumId, model.Id, model.Title, model.PostDateTime); //更新最后发表贴
                ForumManage.UpdateLastPoster(model.ForumId,model.PosterId,model.Poster);
                //ForumMyTopic表
                ForumTopicManage.AddMyTopic(model.PosterId,model.Id);
            }
            else { 
                //编辑
                ForumTopicManage.UpdateTopic(model);
            }
            return model;
        }
        public static ForumReplyInfo GetReplyInfo(int replyId) {
            return ForumTopicManage.GetReplyInfo(replyId);
        }
        /// <summary>
        /// 发表或编辑回复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ForumReplyInfo PostReply(ForumTopicInfo topicInfo,ForumReplyInfo replyInfo) {
            if (replyInfo.Id == 0)
            {               

                //处理楼层
                replyInfo.Floor = topicInfo.Replies + 1;
                //添加
                replyInfo.Id = ForumTopicManage.PostReply(replyInfo);
                //Forums表
                ForumManage.UpdateReplies(replyInfo.ForumId); //回复数 + 1
                ForumManage.UpdateLastReply(replyInfo.ForumId, replyInfo.Id, replyInfo.Content, replyInfo.PostDateTime);
                ForumManage.UpdateLastPoster(replyInfo.ForumId, replyInfo.PosterId, replyInfo.Poster);

                //ForumTopics表
                ForumTopicManage.UpdateTopicRepliesCount(replyInfo.TopicId);   //回复数 + 1
                ForumTopicManage.UpdateTopicLastPoster(replyInfo.TopicId, replyInfo.PosterId, replyInfo.Poster);
                
                //ForumMyReply表
                ForumTopicManage.AddMyReply(replyInfo.PosterId, replyInfo.TopicId, replyInfo.Id);
            }
            else {
                ForumTopicManage.UpdateReply(replyInfo);
            }
            return replyInfo;
        }
        /// <summary>
        /// 删除主题
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public static void DeleteTopic(ForumTopicInfo topicInfo) {
            if (topicInfo.IsDeleted) { return; }
            //
            ForumTopicManage.DeleteTopic(topicInfo.Id);
            //更新Forums表中回复数
            ForumManage.UpdateTopics(topicInfo.ForumId, false);
        }
        /// <summary>
        /// 还原主题
        /// </summary>
        /// <param name="topicInfo"></param>
        public static void RestoreTopic(ForumTopicInfo topicInfo) {
            if (!topicInfo.IsDeleted) { return; }
            //
            ForumTopicManage.RestoreTopic(topicInfo.Id);
            //更新Forums表中回复数
            ForumManage.UpdateTopics(topicInfo.ForumId, true);
        }
        /// <summary>
        /// 删除回复
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="replyId"></param>
        /// <returns></returns>
        public static int DeleteReply(ForumReplyInfo replyInfo) {
            ForumTopicManage.UpdateReplyDeleted(replyInfo.Id);
            //更新Forums表中回复数
            ForumManage.UpdateReplies(replyInfo.ForumId,false);
            return 1;
        }
        /// <summary>
        /// 更新查看数
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateTopicViewCount(int id) {
            ForumTopicManage.UpdateTopicViewsCount(id);
        }
    }
}
