using System;
using Hite.Common.Reflection;

namespace Hite.Model
{
    public class JobInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int CategoryId { get; set; }
        [DbField(Size = 100)]
        public string ParentCategoryIds { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        [DbField(Size = 100)]
        public string Title { get; set; }
        /// <summary>
        /// 招聘部门
        /// </summary>
        [DbField(Size = 100)]
        public string Department { get; set; }
        /// <summary>
        /// 招聘地区
        /// </summary>
        [DbField(Size = 50)]
        public string Area { get; set; }
        /// <summary>
        /// 招聘人数
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 职位描述
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 应聘邮箱
        /// </summary>
        [DbField(Size = 50)]
        public string Email { get; set; }
        public int Sort { get; set; }
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 结束日期，如果没填就为NULL
        /// </summary>
        public DateTime EndDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public JobInfo() {
            ParentCategoryIds = string.Empty;
            Title = string.Empty;
            Department = string.Empty;
            Area = string.Empty;
            Introduction = string.Empty;
            Email = string.Empty;
            CreateDateTime = DateTime.Now;
            EndDateTime = DateTime.MaxValue;
        }
    }
}
