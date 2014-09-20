/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-09-19 15:01:16
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-09-19 15:01:16
 * Description: 论坛版主
 * ********************************************************************/  
using System;

namespace Hite.Model
{
    public class ForumModeratorInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
