
namespace Hite.Web.Forum.Models
{
    /// <summary>
    /// 提示实体
    /// </summary>
    public class TipModel
    {
        public string Msg { get; set; }
        public string Url { get; set; }
        public bool Success { get; set; }
        public TipModel()
        {
            Success = false;
            Url = "/";
        }
    }
}