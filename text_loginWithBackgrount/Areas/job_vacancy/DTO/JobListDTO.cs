using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.DTO
{
    public class JobListDTO
    {
        public int? JobID { get; set; }

        public string? JobTitle { get; set; }

        public int? CompanyID { get; set; }

        public string? CompanyName { get; set; }

        public string? JobLocation { get; set; }

        public string? JobType { get; set; }

        public string? Salary { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string? RequiredPeople { get; set; }

    }
}
