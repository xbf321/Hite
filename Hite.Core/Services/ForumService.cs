using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;
using System.Web;

namespace Hite.Services
{
    public static class ForumService
    {
        #region == ForumGroupInfo ==
        public static ForumGroupInfo GetGroupInfo(int id) {
            return ForumManage.GetGroup(id);
        }
        public static ForumGroupInfo UpdateGroupInfo(ForumGroupInfo model) {
            if (model.Id == 0)
            {
                //添加
                model.Id = ForumManage.AddGroup(model);
            }
            else {
                ForumManage.UpdateGroup(model);
            }
            return model;
        }
        public static IList<ForumGroupInfo> GroupList() {
            return GroupList(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLoadForum">是否加载板块，默认false</param>
        /// <returns></returns>
        public static IList<ForumGroupInfo> GroupList(bool isLoadForum) {
            var groupList = ForumManage.GroupList();
            if(isLoadForum){
                foreach(var item in groupList){
                    item.Forums = ForumManage.List(item.Id);
                }
            }
            return groupList;
        }
        #endregion

        #region == ForumInfo ==
        public static ForumInfo Update(ForumInfo model) {
            if (model.Id == 0)
            {
                model.Id = ForumManage.Add(model);
            }
            else {
                ForumManage.Update(model);
            }
            return model;
        }
        public static ForumInfo Get(int id) {
            return ForumManage.Get(id);
        }
        #endregion

        public static List<ForumModeratorInfo> GetModerators(int forumId = 0) {
            return ForumManage.GetModerators(forumId);
        }
        /// <summary>
        /// 添加版主
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddModerator(ForumModeratorInfo model) {
            return ForumManage.AddModerator(model);
        }
        /// <summary>
        /// 删除版主
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forumId"></param>
        public static void DeleteModerator(int userId, int forumId) {
            ForumManage.DeleteModerator(userId,forumId);
        }
        /// <summary>
        /// 是否是版主
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forumId"></param>
        /// <returns></returns>
        public static bool IsModerator(int userId, int forumId) {
            return ForumManage.IsModerator(userId,forumId);
        }

        #region == RenderForumDropdownList ==
        public static HtmlString RenderForumDropdownList(string name, object value)
        {
            return RenderForumDropdownList(name, value, false);
        }
        /// <summary>
        /// 输出板块列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="showDeleted">显示删除，默认不显示</param>
        /// <returns></returns>
        private static HtmlString RenderForumDropdownList(string name, object value, bool showDeleted)
        {
            StringBuilder sbText = new StringBuilder();
            sbText.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            sbText.Append("<option value=\"\">==请选择==</option>");
            var forumGroupList = ForumService.GroupList(true);
            foreach (var group in forumGroupList)
            {
                if (group.IsDeleted == showDeleted)
                {
                    sbText.AppendFormat("<optgroup label=\"{0}\">", group.Name);
                    foreach (var forum in group.Forums)
                    {
                        if (forum.IsDeleted == showDeleted)
                        {
                            string selected = "";
                            if (value != null && forum.Id.Equals(value)) selected = "selected=\"selected\"";
                            sbText.AppendFormat("<option value=\"{0}\" {2}>{1}</option>", forum.Id, forum.Name, selected);
                        }
                    }
                    sbText.Append("</optgroup>");
                }
            }
            sbText.Append("</select>");

            return new HtmlString(sbText.ToString());
        }
        #endregion
    }
}
