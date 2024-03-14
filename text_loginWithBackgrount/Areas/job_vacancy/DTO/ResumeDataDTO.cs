using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.job_vacancy.DTO
{
    public class ResumeDataDTO
    {
        public T工作履歷資料? Resume { get; set; }
        public T會員學生? Student { get; set; }
        public List<T工作工作經驗>? WorkExperience { get; set; }
    }
}
