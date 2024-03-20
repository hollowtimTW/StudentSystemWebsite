namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class MyFavoritesJobsViewModel
    {
        public int FavoriteID { get; set; }
        public int JobID { get; set; }
        public string? JobTitle { get; set; }
        public DateTime AddTime { get; set; }

        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public string? Salary { get; set; }
        public string? JobType { get; set; }
        public string? JobLocation { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsFavorite { get; set; }
    }
}
