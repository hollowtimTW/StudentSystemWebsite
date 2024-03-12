using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class WorkExpViewModel
    {
        public int WorkExpID { get; set; }
        public int StudentID { get; set; }
        public List<int>? AvailableYears { get; set; }


        [Display(Name = "公司名稱")]
        public string? CompanyName { get; set; }

        [Display(Name = "職務名稱")]
        public string? JobTitle { get; set; }

        [Display(Name = "開始年月")]
        public string? Start { get; set; }

        [Display(Name = "結束年月")]
        public string? End { get; set; }

        [Display(Name = "薪水待遇")]
        public string? Salary { get; set; }

        [Display(Name = "工作內容")]
        public string? JobContent { get; set; }

    }
}
