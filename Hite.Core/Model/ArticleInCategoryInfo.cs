using Hite.Common.Reflection;

namespace Hite.Model
{
    public class ArticleInCategoryInfo
    {
        public int SiteId { get; set; }
        public int CategoryId { get; set; }
        [DbField(Size=100)]
        public string CategoryName { get; set; }
        public int ArticleId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
