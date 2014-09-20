using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ForumTopicInfo
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        [DbField(Size = 1000)]
        public string Title { get; set; }
        public string Content { get; set; }
        [DbField(Size = 100)]
        public string Poster { get; set; }
        public int PosterId { get; set; }
        [DbField(Size = 100)]
        public string LastPoster { get; set; }
        public int LastPosterId { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public DateTime PostDateTime { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string PostDateTimeString { get; set; }
        public DateTime LastPostDateTime { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string LastPostDateTimeString { get; set; }
        /// <summary>
        /// 置顶
        /// </summary>
        public int Sticky { get; set; }
        /// <summary>
        /// 加精
        /// </summary>
        public int Digest { get; set; }
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 扩展字段，标示帖子的图片
        /// </summary>
        public string Folder { get; set; }


        public ForumTopicInfo() {
            Title = Content = Poster = LastPoster = string.Empty;
            PostDateTime = DateTime.Now;
            LastPostDateTime = DateTime.Parse("1900-1-1");

        }
    }
}
