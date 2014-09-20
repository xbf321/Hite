using System.ComponentModel;

namespace Hite.Model
{
    public enum TemplateType
    {
        
        /// <summary>
        /// 无
        /// </summary>
        [Description("==请选择==")]
        None = 0,
        /// <summary>
        /// 显示一条类别详细信息（图片和描述），不显示此类别下的文章
        /// </summary>
        [Description("显示一条类别详细信息，不显示此类别下的文章")]
        ShowSingleCategoryInfo = 2,
        /// <summary>
        /// 类别一列一列的显示
        /// </summary>
        [Description("类别列表（一列）")]
        CategoryListWithOneColumn = 3,
        /// <summary>
        /// 类别两列显示
        /// </summary>
        [Description("类别列表（二列）")]
        CategoryListWithTwoColumn = 4,
        [Description("文章列表")]
        ArticleList = 6,
        /// <summary>
        /// 文章列表（头一条带有图片）
        /// </summary>
        [Description("文章列表（头条带有图片）")]
        ArticleListWithTopOneImage = 1,
        /// <summary>
        /// 
        /// </summary>
        [Description("文章列表（带有类别图片）")]
        ArticleListWithCategory = 5,
        [Description("文章列表（每一条都带有图片）")]
        ArticleListWithImage = 9,
        /// <summary>
        /// 职位列表
        /// </summary>
        [Description("职位列表")]
        JobList = 7,
        /// <summary>
        /// 资料下载列表
        /// </summary>
        [Description("资料下载列表")]
        AttachmentList = 8
    }
}
