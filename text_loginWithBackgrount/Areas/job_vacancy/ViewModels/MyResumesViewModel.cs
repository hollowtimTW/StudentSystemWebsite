using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class MyResumesViewModel
    {
        public int StudentID { get; set; }
        public int ResumeID { get; set; }
        public string? ResumeTitle { get; set; }
        public string? HopeJobTitle { get; set; }
        public string? WorkType { get; set; }
        public DateTime? LastUpdate { get; set; }

    }
}
