using System;

using Hite.Common.Reflection;

namespace Hite.Model
{
    /// <summary>
    /// 资料下载
    /// </summary>
    public class AttachmentInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int CategoryId { get; set; }
        [DbField(Size=100)]
        public string ParentCategoryIds { get; set; }
        [DbField(Size = 100)]
        public string Title { get; set; }
        public FileExtension Extension { get; set; }
        public int Size { get; set; }
        public string Introduction { get; set; }
        [DbField(Size = 200)]
        public string Url { get; set; }
        public int Sort { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public AttachmentInfo() {
            ParentCategoryIds = string.Empty;
            Title = string.Empty;
            Introduction = string.Empty;
            Url = string.Empty;
            CreateDateTime = PublishDate = DateTime.Now;
        }
    }
}
