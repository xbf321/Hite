using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ArticleInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int CategoryId { get; set; }
        /// <summary>
        /// 记住类别的路径
        /// </summary>
        [DbField(Size = 100)]
        public string ParentCategoryIds { get; set; }
        [DbField(Size=200)]
        public string Title { get; set; }
        public string Remark { get; set; }
        public string Content { get; set; }
        [DbField(Size = 200)]
        public string ImageUrl { get; set; }
        [DbField(Size = 200)]
        public string Url { get; set; }
        [DbField(Size = 200)]
        public string LinkUrl { get; set; }
        [DbField(Size = 200)]
        public string Tags { get; set; }
        public int Sort { get; set; }
        public bool IsTop { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        //public string GUID { get; set; }
        public string Timespan { get; set; }
        public string CatsJSON { get; set; }

        public ArticleInfo() {
            IsTop = false;
            IsDeleted = false;
            Sort = 0;
            PublishDateTime = DateTime.Now;
            CreateDateTime = DateTime.Now;
            CatsJSON = string.Empty;
            ParentCategoryIds = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
            ImageUrl = string.Empty;
            LinkUrl = string.Empty;
            Tags = string.Empty;
            Timespan = DateTime.Now.ToString("HHmmssfff");
            Url = string.Empty;
            //GUID = Guid.NewGuid().ToString();
        }
    }
}
