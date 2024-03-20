using Class_system_Backstage_pj.Models;
using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class MyApplyRecordsViewModel
    {
        public int ApplyRecordID { get; set; }
        public int JobID { get; set; }
        public string? JobTitle { get; set; }
        public string? LetterContent { get; set; }
        public DateTime ApplyTime { get; set; }

        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? Salary { get; set; }
        public string? JobType { get; set; }
        public string? JobLocation { get; set; }
        public DateTime? UpdateTime { get; set; }

        public bool IsFavorite { get; set; }
    }
}
