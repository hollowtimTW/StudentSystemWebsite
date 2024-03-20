using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class JobDetailViewModel
    {
        public int JobID { get; set; }

        public bool IsFavorite { get; set; }

        [Display(Name = "職務名稱")]
        public string? JobTitle { get; set; }

        public int? CompanyID { get; set; }

        [Display(Name = "公司名稱")]
        public string? CompanyName { get; set; }

        [Display(Name = "更新時間")]
        public string? UpdateTime { get; set; }

        [Display(Name = "工作內容")]
        public string? JobContent { get; set; }

        [Display(Name = "薪水待遇")]
        public string? Salary { get; set; }

        [Display(Name = "工作性質")]
        public string? JobType { get; set; }

        [Display(Name = "工作地點")]
        public string? JobLocation { get; set; }

        [Display(Name = "工作時段")]
        public string? JobTime { get; set; }

        [Display(Name = "輪班需求")]
        public string? ShiftRequirement { get; set; }

        [Display(Name = "需求人數")]
        public string? RequiredPeople { get; set; }

        [Display(Name = "學歷要求")]
        public string? AcademicRequirement { get; set; }

        [Display(Name = "語文條件")]
        public string? LanguageCondition { get; set; }

        [Display(Name = "工作技能")]
        public string? WorkAbility { get; set; }

        [Display(Name = "其他條件")]
        public string? OtherCondition { get; set; }

        [Display(Name = "聯絡人")]
        public string? ContactPerson { get; set; }

        [Display(Name = "電洽")]
        public string? ContactPhone { get; set; }

        [Display(Name = "Email")]
        public string? ContactEmail { get; set; }
    }
}
