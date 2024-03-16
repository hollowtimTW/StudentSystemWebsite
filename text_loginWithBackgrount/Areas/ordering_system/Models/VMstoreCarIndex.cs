using Class_system_Backstage_pj.Areas.ordering_system.Models;

namespace text_loginWithBackgrount.Areas.ordering_system.Models
{
    public class VMstoreCarIndex
    {
        public int 店家id { get; set; }
        public string 電話 { get; set; }
        public string 餐廳照片 { get; set; }
        public string 店家名稱 { get; set; }
        public string 餐廳介紹 { get; set; }
        public List<string> 風味列表 { get; set; }
        public string 平均評論 { get; set; }
    }
}
