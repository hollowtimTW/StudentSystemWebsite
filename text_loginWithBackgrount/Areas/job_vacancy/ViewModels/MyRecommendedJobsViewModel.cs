namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class MyRecommendedJobsViewModel
    {
        public int RecommendedID { get; set; }
        public int JobID { get; set; }
        public string? JobTitle { get; set; }
        public string? Score { get; set; }

        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? Salary { get; set; }
        public string? JobType { get; set; }
        public string? JobLocation { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsFavorite { get; set; }
    }
}
