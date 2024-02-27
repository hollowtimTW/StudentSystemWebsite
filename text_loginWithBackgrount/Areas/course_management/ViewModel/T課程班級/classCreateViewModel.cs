namespace Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級
{
    public class classCreateViewModel
    {
        public string? 班級名稱 { get; set; }
        public int 班級導師id { get; set; }
        public DateTime 入學日期 { get; set; }
        public DateTime 結訓日期 { get; set; }
        public List<ClassSubjectPair>? 班級科目 { get; set; }
        public int 狀態 { get; set; }
    }
}
