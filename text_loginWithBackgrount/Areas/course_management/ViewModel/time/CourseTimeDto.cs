using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.course_management.ViewModel.time
{
    public class CourseTimeDto
    {

        public int 班級科目id { get; set; }

        public DateTime 開始時間 { get; set; }

        public DateTime 結束時間 { get; set; }

        public string 課堂摘要 { get; set; }

        public int? 值日生id { get; set; }

        public int 狀態 { get; set; }

        public string  日期 { get; set; }

    }
}
