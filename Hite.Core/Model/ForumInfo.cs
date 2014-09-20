/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-09-19 15:01:27
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-09-19 15:01:27
 * Description: 论坛板块信息
 * ********************************************************************/  
using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ForumInfo
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        [DbField(Size = 200)]
        public string Name { get; set; }
        public string Info { get; set; }
        public int Topics { get; set; }
        public int Replies { get; set; }
        public int LastTopicId { get; set; }
        [DbField(Size = 1000)]
        public string LastTopic { get; set; }
        public DateTime LastTopicDateTime { get; set; }
        public int LastReplyId { get; set; }
        [DbField(Size = 1000)]
        public string LastReply { get; set; }
        public DateTime LastReplyDateTime { get; set; }
        public int LastPosterId { get; set; }
        [DbField(Size = 100)]
        public string LastPoster { get; set; }
        public int Sort { get; set; }
        public bool IsDeleted { get; set; }

        public ForumInfo() {
            Name = Info = LastTopic = LastReply = LastPoster = string.Empty;
            LastReplyDateTime = LastTopicDateTime = DateTime.Parse("1900-1-1");
        }
    }
}
