using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.job_vacancy.ViewModels
{
    public class ResumeListViewModel
    {
        [Display(Name = "姓名")]
        public string? Name { get; set; }

        [Display(Name = "班級")]
        public string? ClassName { get; set; }

        [Display(Name = "履歷名稱")]
        public string? ResumeTitle { get; set; }

        [Display(Name = "狀態")]
        public string? ResumeStatus { get; set; }

        [Display(Name = "希望職稱")]
        public string? HopeJobTitle { get; set; }

        [Display(Name = "希望薪水待遇")]
        public string? HopeSalary { get; set; }

        [Display(Name = "希望工作地點")]
        public string? HopeLocation { get; set; }

        [Display(Name = "最後更新時間")]
        public DateTime? LastUpdate { get; set; }
    }
}
