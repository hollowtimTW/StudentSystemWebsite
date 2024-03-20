using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class WorkExpViewModel
    {
        public int WorkExpID { get; set; }
        public int StudentID { get; set; }
        public List<int>? AvailableYears { get; set; }

        public bool IsInResume { get; set; }

        [Display(Name = "公司名稱")]
        [Required(ErrorMessage = "請填寫任職公司名稱")]
        [StringLength(50, ErrorMessage = "任職公司名稱過長")]
        public string? CompanyName { get; set; }

        [Display(Name = "職務名稱")]
        [Required(ErrorMessage = "請填寫職務名稱")]
        [StringLength(50, ErrorMessage = "職務名稱過長")]
        public string? JobTitle { get; set; }

        [Display(Name = "開始年月")]
        [Required(ErrorMessage = "請選取工作經歷起始點")]
        public string? Start { get; set; }

        [Display(Name = "結束年月")]
        [Required(ErrorMessage = "請填寫工作經歷結束點")]
        public string? End { get; set; }

        [Display(Name = "薪水待遇")]
        [Required(ErrorMessage = "請填寫薪資待遇狀況")]
        [StringLength(50, ErrorMessage = "薪資待遇內容過長")]
        public string? Salary { get; set; }

        [Display(Name = "工作內容")]
        [Required(ErrorMessage = "請填寫任職工作內容")]
        [StringLength(500, ErrorMessage = "任職工作內容過長")]
        public string? JobContent { get; set; }

    }
}
