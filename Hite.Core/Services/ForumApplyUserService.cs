using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class ForumApplyUserService
    {
        /// <summary>
        /// 申请
        /// </summary>
        /// <param name="modelList"></param>
        public static void Applying(List<ForumApplyUserInfo> modelList,int userId) {
            //在这有一下情况
            //1,从没有申请过(直接插入)
            //2,在申请中，不可再申请，也不能更改接洽人
            //3,已申请通过，不可再申请，也不能更改接洽人
            //4,申请未通过，可以在申请，可以更改接洽人，申请的时候把申请状态，从申请未通过，更改为申请中
            var hasData = ListByUserId(userId);
            foreach(var model in modelList){
                //判断是否在ForumApplyUsers表中有数据
                bool isHasData = false;
                var data = hasData.SingleOrDefault(p => p.ForumGroupId == model.ForumGroupId);
                if(data != null && data.Id>0){
                    //有数据，更新，判断状态
                    isHasData = true;
                    switch(data.Status){
                        case ForumApplyStatus.Applying:
                        case ForumApplyStatus.Passed:
                            //申请中或申请通过，不可再申请，也不能更改接洽人
                            break;
                        case ForumApplyStatus.NoPass:
                            //没通过，可以更改接洽人，申请的时候把申请状态，从申请未通过，更改为申请中
                            model.Id = data.Id;
                            model.Status = ForumApplyStatus.Applying;
                            ForumApplyUserManage.Update(model);
                            break;
                    }
                }
                if(!isHasData){
                    //没有数据，插入
                    ForumApplyUserManage.Add(model);
                }
            }
        }
        /// <summary>
        /// 根据用户ID获得通过的论坛版块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<int> GetPassedGroupIdsByUserId(int userId) {
            return ForumApplyUserManage.GetPassedGroupIdsByUserId(userId);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public static void UpdateStatus(int id, ForumApplyStatus status) {
            ForumApplyUserManage.UpdateStatus(id,status);
        }
        /// <summary>
        /// 根据用户ID获得此用户所有的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<ForumApplyUserInfo> ListByUserId(int userId) {
            return ForumApplyUserManage.ListByUserId(userId);
        }
        public static IPageOfList<ForumApplyUserInfo> List(int pageIndex, int pageSize) {
            return ForumApplyUserManage.List(pageIndex,pageSize);
        }
    }
}
