using Class_system_Backstage_pj.Models;

namespace Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級科目評分
{
    public class RateFormViewModel
    {
        public int 評分主表id { get; set; }
        public int 班級科目id { get; set; }
        public int 學生id { get; set; }
        public DateTime 提交時間 { get; set; }
        public int 狀態 { get; set; }
        public string 改進意見 { get; set; }
        public ICollection<T課程評分> T課程評分s { get; set; } = new List<T課程評分>();
        public Dictionary<int, int> 評分s { get; set; }

    }
}
