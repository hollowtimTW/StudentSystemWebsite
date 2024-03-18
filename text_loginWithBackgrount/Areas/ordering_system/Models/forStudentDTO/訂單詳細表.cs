namespace text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO
{
    public class 訂單詳細表
    {
        public string? 店家名稱 { get; set; }
        public string? 餐點名稱 { get; set; }

        public int? 數量 { get; set; }
        public int 小計 { get; set; }
        public string 信箱 { get; set; }
        public string 電話 { get; set; }
        public string 狀態 { get; set; }
    }
}
