using Class_system_Backstage_pj.Models;
using text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO;

namespace text_loginWithBackgrount.Areas.ordering_system.Models
{
    public class VMstore
    {
        public int 店家id { get; set; }
        public string 店家名稱 { get; set; }
        public string? 地址 { get; set; }
        public string? 餐廳照片 { get; set; }
        public List<T訂餐餐點資訊表>? 餐點列表 { get; set; }
        public List<T訂餐營業時間表>? 營業時間表 { get; set; }
        public List<CminStore>? 相關店家 { get; set; }

    }
}
