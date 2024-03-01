using System.Numerics;

namespace text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO
{
    public class 學生訂單
    {
        public int? 訂單id { get; set; }
        public string 狀態 { get; set; }
        public string 日期 { get; set; }
        public string 支付方式 { get; set; }
        public string 評價內容 { get; set; }
        public string 評價星數 { get; set; }
        public string 總價 { get; set; }
        public List<訂單詳細表> 訂單詳細表 { get; set; }
    }
}
