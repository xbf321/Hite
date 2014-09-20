/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-09-19 16:26:10
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-09-19 16:26:10
 * Description: 
 * ********************************************************************/
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;


using Hite.Services;
using Hite.Model;
using Controleng.Common;
using Hite.Web.Forum.Models;
using Hite.Mvc.Filters;
using System.Web;
using System.IO;


namespace Hite.Web.Forum.Controllers
{
    public class ErrorMsg
    {
        public const string CATALOGNOTEXISTS = "对不起，该类目不存在";
        public const string THREADNOTEXISTS = "你所要查阅的帖子不存在，论坛已经没了它的消息";
        public const string APPLYNOTPASS = "对不起，您没有权限查看";
        public const string THREADNOTFORYOU = "对不起，你所要查阅的帖子不属于您";
        public const string NOTNORMALOPERATE = "对不起，您不是正常操作";
        public const string DELETETHREADSUCCESS = "帖子删除成功";
        public const string DELETEREPLYSUCCESS = "回复删除成功";
        public const string POSTTHREADSUCCESS = ""; //暂时没用
        public const string POSTREPLYSUCCESS = "回帖成功";
    }
    [SilenceHandleError]
    public class HomeController : Controller
    {
        log4net.ILog Logger = log4net.LogManager.GetLogger("HomeController");

        #region == 首页 ==
        public ActionResult Index()
        {
            ViewBag.GroupList = ForumService.GroupList(true).Where(p => p.IsDeleted == false);
            return View();
        }
        #endregion

        #region == 显示板块 ==
        [ForumAuth]
        public ActionResult ShowCatalog(int? id)
        {
            int forumId = id.HasValue ? Convert.ToInt32(id.Value) : 0;
            var forumInfo = ForumService.Get(forumId);
            if (forumInfo.Id == 0)
            {
                return new TipView() { Msg = ErrorMsg.CATALOGNOTEXISTS };
            }
            var userInfo = UserService.Get(User.Identity.Name);
            //检查用户是否查看的权限
            //获取通过审核的用户，只有审核通过的用户才能查看论坛
            if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
            {
                return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
            }

            int pageIndex = CECRequest.GetQueryInt("page", 1);
            var topicList = ForumTopicService.TopicList(new ForumSearchSetting() { ForumId = forumInfo.Id, PageIndex = pageIndex });

            ViewBag.TopicList = topicList;
            //是否是版主
            ViewBag.IsModerator = ForumService.IsModerator(userInfo.Id, forumId);
            //版主列表
            var moderatorList = ForumService.GetModerators(forumId);
            if (moderatorList.Count > 0)
            {
                ViewBag.ModeratorList = moderatorList.Select(p => p.UserName).Aggregate((a, b) => a + "," + b);
            }
            else
            {
                ViewBag.ModeratorList = "*空缺*";
            }

            return View(forumInfo);
        }
        #endregion

        #region == 显示贴 ==
        [ForumAuth]
        public ActionResult ShowThread(int? id)
        {
            int topicId = id.HasValue ? Convert.ToInt32(id.Value) : 0;
            var topicInfo = ForumTopicService.Get(topicId);
            int pageIndex = CECRequest.GetQueryInt("page", 1);
            ///是否显示主题，除了第一页显示主题页外，其它页不显示主题
            bool showTopic = false;

            if (topicInfo.Id == 0 || topicInfo.IsDeleted)
            {
                return new TipView() { Msg = ErrorMsg.THREADNOTEXISTS };
            }
            ///板块信息，用于导航
            var forumInfo = ForumService.Get(topicInfo.ForumId);
            //检查用户是否查看的权限
            //获取通过审核的用户，只有审核通过的用户才能查看论坛
            var userInfo = UserService.Get(User.Identity.Name);
            if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
            {
                return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
            }

            //更新查看数
            if (pageIndex <= 1)
            {
                //只有在第一页的时候，才更新浏览数
                ForumTopicService.UpdateTopicViewCount(topicInfo.Id);
                showTopic = true;
            }
            ViewBag.ShowTopic = showTopic;

            ViewBag.ForumInfo = forumInfo;
            ViewBag.LoginUserInfo = userInfo;

            ///回帖列表
            var replyList = ForumTopicService.ReplyList(new ForumSearchSetting() { ForumId = topicInfo.ForumId, TopicId = topicInfo.Id, PageIndex = pageIndex });
            ViewBag.ReplyList = replyList;

            //是否是版主
            ViewBag.IsModerator = ForumService.IsModerator(userInfo.Id, forumInfo.Id);

            return View(topicInfo);
        }
        #endregion

        #region == 发贴 ==
        [ForumAuth]
        public ActionResult PulishThread(int? forumId, int? topicId)
        {
            int _forumId = forumId.HasValue ? Convert.ToInt32(forumId.Value) : 0;
            int _topicId = topicId.HasValue ? Convert.ToInt32(topicId.Value) : 0;

            //判断板块是否存在
            var forumInfo = ForumService.Get(_forumId);
            //检查用户是否查看的权限
            //获取通过审核的用户，只有审核通过的用户才能查看论坛
            var userInfo = UserService.Get(User.Identity.Name);
            if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
            {
                return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
            }

            if (forumInfo.Id == 0)
            {
                //跳转到错误页
                return new TipView() { Msg = ErrorMsg.CATALOGNOTEXISTS };
            }

            //判断是否主题存在，不存在，默认添加
            var topicInfo = ForumTopicService.Get(_topicId);
            if (_topicId > 0)
            {
                if (topicInfo.Id == 0 || topicInfo.IsDeleted)
                {
                    //跳转到错误页
                    return new TipView() { Msg = ErrorMsg.THREADNOTEXISTS };
                }
                if (topicInfo.Id > 0 && topicInfo.ForumId != forumInfo.Id)
                {
                    return new TipView() { Msg = ErrorMsg.THREADNOTEXISTS };
                }
                if (topicInfo.Id > 0 && topicInfo.PosterId != userInfo.Id)
                {
                    return new TipView() { Msg = ErrorMsg.THREADNOTFORYOU };
                }
            }
            ViewBag.ForumInfo = forumInfo;
            return View(topicInfo);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ForumAuth]
        public ActionResult PulishThread(ForumTopicInfo oldModel)
        {
            #region == 发表主题 ==
            if (CECRequest.GetFormString("event_submit_do_publish") == "anything")
            {
                //发布或编辑
                var forumInfo = ForumService.Get(oldModel.ForumId);
                //检查用户是否查看的权限
                //获取通过审核的用户，只有审核通过的用户才能查看论坛
                var userInfo = UserService.Get(User.Identity.Name);
                if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
                {
                    return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
                }

                ViewBag.ForumInfo = forumInfo;

                //在这里多设一个
                //下面更新问oldModel就会自动变成新的实体
                int requestTopicId = oldModel.Id;

                oldModel.PosterId = userInfo.Id;
                oldModel.Poster = userInfo.UserName;
                if (string.IsNullOrEmpty(oldModel.Title))
                {
                    ModelState.AddModelError("Title", "帖子标题不能为空！");
                }
                if (string.IsNullOrEmpty(oldModel.Content))
                {
                    ModelState.AddModelError("Content", "帖子内容不能为空！");
                }
                if (ModelState.IsValid)
                {

                    oldModel = ForumTopicService.PostTopic(oldModel);
                    string url = String.Format("/thread/{0}.html", oldModel.Id);
                    return new TipView() { Msg = string.Format("{0}成功！", requestTopicId > 0 ? "编辑" : "发表"), Url = url, Success = true };
                }
            }
            #endregion

            #region == 删除主题 ==
            if (CECRequest.GetFormString("event_submit_do_delete") == "anything")
            {
                //删除主题
                int forumId = CECRequest.GetFormInt("catalogId", 0);
                int topicId = CECRequest.GetFormInt("threadId", 0);
                string returnUrl = string.Format("/catalog/{0}.html", forumId);

                var forumInfo = ForumService.Get(forumId);
                //检查用户是否查看的权限
                //获取通过审核的用户，只有审核通过的用户才能查看论坛
                var userInfo = UserService.Get(User.Identity.Name);
                if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
                {
                    return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
                }

                var topicInfo = ForumTopicService.Get(topicId);
                if (topicInfo.Id > 0 && topicInfo.ForumId == forumId && !topicInfo.IsDeleted)
                {
                    //执行删除操作
                    ForumTopicService.DeleteTopic(topicInfo);
                    return new TipView() { Msg = ErrorMsg.DELETETHREADSUCCESS, Success = true, Url = returnUrl };
                }
                return new TipView() { Msg = ErrorMsg.NOTNORMALOPERATE, Url = returnUrl };
            }
            #endregion

            return View(oldModel);
        }
        #endregion

        #region == 回帖 ==
        [HttpPost]
        [ValidateInput(false)]
        [ForumAuth]
        public ActionResult ReplyThread(FormCollection fc)
        {
            int forumId = CECRequest.GetFormInt("catalogId", 0);
            int topicId = CECRequest.GetFormInt("threadId", 0);

            var forumInfo = ForumService.Get(forumId);
            //检查用户是否查看的权限
            //获取通过审核的用户，只有审核通过的用户才能查看论坛
            var userInfo = UserService.Get(User.Identity.Name);
            if (!CheckApplyUserAuth(userInfo.Id, forumInfo.GroupId))
            {
                return new TipView() { Msg = ErrorMsg.APPLYNOTPASS };
            }

            //判断主题是否存在
            var topicInfo = ForumTopicService.Get(topicId);

            string threadUrlFormat = "/thread/{0}.html{1}";
            if (topicInfo.Id == 0 || topicInfo.ForumId != forumId || topicInfo.IsDeleted)
            {
                return new TipView() { Msg = ErrorMsg.THREADNOTEXISTS };
            }

            #region == 发表回帖 ==
            //判断提交类型
            if (CECRequest.GetFormString("event_submit_do_publish") == "anything")
            {
                string replyContent = fc["txtReplyContent"];

                //判断回复内容是否为空
                if (string.IsNullOrEmpty(replyContent))
                {
                    return new TipView() { Msg = "请输入回复内容", Url = String.Format(threadUrlFormat, topicInfo.Id, string.Empty) };
                }


                //回复
                ForumReplyInfo replyInfo = new ForumReplyInfo();
                replyInfo.Content = replyContent;
                replyInfo.ForumId = topicInfo.ForumId;
                replyInfo.TopicId = topicInfo.Id;
                replyInfo.Poster = userInfo.UserName;
                replyInfo.PosterId = userInfo.Id;

                replyInfo = ForumTopicService.PostReply(topicInfo, replyInfo);

                return new TipView() { Msg = ErrorMsg.POSTREPLYSUCCESS, Url = String.Format(threadUrlFormat, topicInfo.Id, String.Format("#reply{0}", replyInfo.Id)), Success = true };
            }
            #endregion

            #region == 删除回帖 ==
            if (CECRequest.GetFormString("event_submit_do_delete") == "anything")
            {
                int replyId = CECRequest.GetFormInt("replyId", 0);

                var replyInfo = ForumTopicService.GetReplyInfo(replyId);
                if (replyInfo.Id == 0 || replyInfo.IsDeleted || topicInfo.Id != replyInfo.TopicId || replyInfo.ForumId != forumId)
                {
                    return new TipView() { Msg = ErrorMsg.NOTNORMALOPERATE, Url = String.Format(threadUrlFormat, topicInfo.Id, string.Empty) };
                }

                ForumTopicService.DeleteReply(replyInfo);
                return new TipView() { Msg = ErrorMsg.DELETEREPLYSUCCESS, Url = String.Format(threadUrlFormat, topicInfo.Id, string.Empty), Success = true };
            }
            #endregion

            return new TipView() { Msg = ErrorMsg.NOTNORMALOPERATE, Url = String.Format(threadUrlFormat, topicInfo.Id, string.Empty) };

        }
        #endregion

        #region == 输出导航 ==
        [ChildActionOnly]
        public ActionResult RenderNav(ForumInfo forumInfo, ForumTopicInfo topicInfo)
        {
            StringBuilder sbText = new StringBuilder("<div class=\"nav\">");
            sbText.Append("<a href=\"/\" title=\"首页\" class=\"nav-home\">首页</a>");
            sbText.Append("<em>›</em>");
            sbText.Append("<a href=\"/\" title=\"论坛首页\">论坛首页</a>");
            if (forumInfo != null && forumInfo.Id > 0)
            {

                //获取ForumGroupInfo 
                var forumGroupInfo = ForumService.GetGroupInfo(forumInfo.GroupId);
                sbText.Append("<em>›</em>");
                sbText.AppendFormat("<a href=\"/#catalog_{1}\" title=\"{0}\">{0}</a>", forumGroupInfo.Name, forumGroupInfo.Id);

                sbText.Append("<em>›</em>");
                sbText.AppendFormat("<a href=\"/catalog/{0}.html\" title=\"{1}\">{1}</a>", forumInfo.Id, forumInfo.Name);
            }
            if (topicInfo != null && topicInfo.Id > 0)
            {
                sbText.Append("<em>›</em>");
                sbText.AppendFormat("<a href=\"/thread/{0}.html\" title=\"{1}\">{1}</a>", topicInfo.Id, topicInfo.Title);
            }
            sbText.Append("</div>");
            sbText.Append("<div class=\"clear\"></div>");
            return Content(sbText.ToString());
        }
        #endregion

        #region == 检查是否是通过申请的用户 ==
        private bool CheckApplyUserAuth(int userId, int forumGroupId)
        {
            var myAuthGroupIds = ForumApplyUserService.GetPassedGroupIdsByUserId(userId);
            return myAuthGroupIds.Contains(forumGroupId);
        }
        #endregion

        #region == 上传图片 ==
        [HttpPost]
        public void UploadPhoto()
        {
            var folder = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERFOLDER"];
            var serverDomain = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERDOMAIN"];
            HttpFileCollectionBase files = Request.Files;
            try
            {
                HttpPostedFileBase postedFile = files[0];
                //
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension);
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    string returnImage = string.Format("{0}/{1}/{2}/{3}/{4}", serverDomain, DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);
                    Response.Write(returnImage);
                }
            }
            catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
        #endregion

        #region == 上传附件 ==
        [HttpPost]
        public void UploadSWF() {
            var folder = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERFOLDER"];
            var serverDomain = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERDOMAIN"];
            HttpFileCollectionBase files = Request.Files;
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                HttpPostedFileBase postedFile = files["attachFile"];
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{2}_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension, originalFileName.Replace(originalExtension, string.Empty));
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    /*
                    //以流模式保存文件
                    // 把 Stream 转换成 byte[]
                    byte[] bytes = new byte[postedFile.InputStream.Length];
                    postedFile.InputStream.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始
                    postedFile.InputStream.Seek(0, SeekOrigin.Begin);
                    // 把 byte[] 写入文件
                    fs = new FileStream(String.Concat(imageServerFolder, newFileName), FileMode.Create);
                    bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();*/


                    string returnImage = string.Format("{0}/{1}/{2}/{3}/{4}", serverDomain, DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);
                    Response.StatusCode = 200;
                    Response.Write(returnImage);
                }
                Response.Write("");

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                Response.StatusCode = 500;
                Response.Write("An error occured");
                Response.End();
            }
            finally
            {
                if (fs != null) { fs.Close(); fs.Dispose(); }
                if (bw != null) { bw.Close(); bw.Dispose(); }
                Response.End();
            }
        }
        #endregion

        public ActionResult Test()
        {
            return new TipView() { Msg = "dddd", Url = "http://www.baidu.com" };
        }
    }
}
