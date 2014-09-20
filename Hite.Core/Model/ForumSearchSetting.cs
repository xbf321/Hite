using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hite.Model
{
    public class ForumSearchSetting
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 必填，默认10
        /// </summary>
        public int PageSize { get; set; }

        public bool ShowDeleted { get; set; }

        public ForumSearchSetting() {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
