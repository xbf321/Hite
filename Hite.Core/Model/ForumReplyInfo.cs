/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-09-19 15:01:39
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-09-19 15:01:39
 * Description: 论坛回复信息
 * ********************************************************************/  
using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ForumReplyInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public int Floor { get; set; }
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        public string Content { get; set; }
        [DbField(Size = 100)]
        public string Poster { get; set; }
        public int PosterId { get; set; }
        public DateTime PostDateTime { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string PostDateTimeString { get; set; }
        public bool IsDeleted { get; set; }

        public ForumReplyInfo() {
             Content = Poster = string.Empty;
            PostDateTime = DateTime.Now;
        }
    }
}
