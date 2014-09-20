using System;
using Hite.Common.Reflection;
using System.Collections.Generic;

namespace Hite.Model
{
    public class CategoryInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int ParentId { get; set; }
        [DbField(Size=500)]
        public string ParentIdList { get; set; } 
        /// <summary>
        /// 类别名称
        /// </summary>       
        [DbField(Size=100)]
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [DbField(Size=100)]
        public string Alias { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 图片链接
        /// </summary>
        [DbField(Size = 100)]
        public string ImageUrl { get; set; }
        /// <summary>
        /// 页面的Banner广告图片链接
        /// 目前只针对父节点，子节点字段没有作用
        /// </summary>
        [DbField(Size=100)]
        public string BannerAdImageUrl { get; set; }
        /// <summary>
        /// 快速链接
        /// </summary>
        [DbField(Size = 100)]
        public string LinkUrl { get; set; }
        /// <summary>
        /// 类别描述
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 模板类型[来自Hite.Model.TemplateType]
        /// </summary>
        public int TemplateType { get; set; }
        
        /// <summary>
        /// 是否显示第一个节点的内容
        /// 相当于跳到第一个节点，但是Url不变
        /// </summary>
        public bool IsShowFirstChildNode { get; set; }
        /// <summary>
        /// 是否启用，主要针对有的类别前台不显示，但是要读取类别中的内容，
        /// 比如首页中的焦点图，这样就不用新建焦点图的表了，直接用文章的就行了
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        #region == Extension Protype
        public IList<ArticleInfo> ArticleList { get; set; }
        #endregion

        ///// <summary>
        ///// 是否是根节点
        ///// </summary>
        //public bool IsLeaf { get; set; }
        ///// <summary>
        ///// Meta关键词
        ///// </summary>
        //[DbField(Size = 1000)]
        //public string MetaKeywords { get; set; }
        ///// <summary>
        ///// Meta描述
        ///// </summary>
        //[DbField(Size = 1000)]
        //public string MetaDescription { get; set; }
        //public int Layer { get; set; }
        //public int SubCount { get; set; }

        public CategoryInfo() {
            Sort = 0;
            CreateDateTime = DateTime.Now;
            ParentIdList = string.Empty;
            Introduction = string.Empty;
            //MetaDescription = string.Empty;
            //MetaKeywords = string.Empty;
            Alias = string.Empty;
            Name = string.Empty;
            ImageUrl = string.Empty;
            LinkUrl = string.Empty;
        }
    }
}
