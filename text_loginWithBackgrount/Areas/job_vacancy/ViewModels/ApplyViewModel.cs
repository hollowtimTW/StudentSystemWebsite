using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class ApplyViewModel
    {
        public string? JobTitle { get; set; }
        public List<int>? ResumeIDs { get; set; }
        public List<string>? ResumeTitles { get; set; }
    }
}
