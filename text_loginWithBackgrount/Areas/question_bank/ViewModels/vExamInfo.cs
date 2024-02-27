using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.question_bank.ViewModels
{
    public class vExamInfo
    {
        [Display(Name = "ID")]
        public int fId { get; set; }
        [Display(Name = "考試名稱")]
        public string? fExamName { get; set; }

        [Display(Name = "科目")]
        public string? fCourse { get; set; }

        [Display(Name = "班級")]
        public string? fCLass { get; set; }

        [Display(Name = "開始時間")]
        public string? fSTime { get; set; }

        [Display(Name = "結束時間")]
        public string? fETime { get; set; }

        [Display(Name = "發布者")]
        public string? fPublish { get; set; }

        [Display(Name = "備註")]
        public string? fDescribe { get; set; }


    }
}
