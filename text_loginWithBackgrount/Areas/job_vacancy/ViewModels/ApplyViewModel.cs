using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class ApplyViewModel
    {
        public int StudentID { get; set; }
        public int JobID { get; set; }
        public string? JobTitle { get; set; }
        public List<int>? ResumeIDs { get; set; }
        public List<string>? ResumeTitles { get; set; }

        public string? ApplyLetter { get; set; }
    }
}
