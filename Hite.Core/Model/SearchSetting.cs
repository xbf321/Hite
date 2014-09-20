
namespace Hite.Model
{
    public class SearchSetting
    {
        /// <summary>
        /// 必填
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 可选
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 必填，默认10
        /// </summary>
        public int PageSize { get; set; }

        #region == 主要针对文章 ==
        /// <summary>
        /// 发布时间
        /// 格式2010-02-13
        /// </summary>
        public string PublishDate { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        #endregion

        /// <summary>
        /// 是否显示删除的招聘信息
        /// 默认False
        /// </summary>
        public bool ShowDeleted { get; set; }

        public SearchSetting()
        {
            PageIndex = 1;
            PageSize = 10;
            ShowDeleted = false;
            PublishDate = string.Empty;
            Title = string.Empty;
        } 
    }
}
