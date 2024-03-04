namespace text_loginWithBackgrount.Areas.job_vacancy.DTO
{
    public class SearchJobDTO
    {
        public int? CompanyID { get; set; }
        public string? Keyword { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Location { get; set; }
        public string? SortBy { get; set; }
        public string? SortType { get; set; }
    }
}
