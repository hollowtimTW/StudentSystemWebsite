using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class ApplyViewModel
    {
        public int StudentID { get; set; }
        public int JobID { get; set; }
        public int ResumeID { get; set; }
        public string? JobTitle { get; set; }
        public List<int>? ResumeIDs { get; set; }
        public List<string>? ResumeTitles { get; set; }

        [Required(ErrorMessage = "請填寫信件內容")]
        public string? ApplyLetter { get; set; }
    }
}
