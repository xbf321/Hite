
namespace Hite.Model
{
    public class OrderSearchSetting : SearchSetting
    {
        public int OrderUserId { get; set; }
        /// <summary>
        /// 公司名
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNumber { get; set; }
    }
}
