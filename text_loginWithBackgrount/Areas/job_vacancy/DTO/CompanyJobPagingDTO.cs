using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.job_vacancy.DTO
{
    public class CompanyJobPagingDTO
    {
        public int TotalPages { get; set; }
        public List<T工作職缺資料>? CompanyJobsResult { get; set; }

    }
}
