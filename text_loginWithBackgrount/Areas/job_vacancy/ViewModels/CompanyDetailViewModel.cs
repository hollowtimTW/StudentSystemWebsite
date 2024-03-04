using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.job_vacancy.ViewModels
{
    public class CompanyDetailViewModel
    {
        public int companyID { get; set; }

        [Display(Name = "統編")]
        public string? taxID { get; set; }

        [Display(Name = "公司名稱")]
        public string? CompanyName { get; set; }

        [Display(Name = "聯絡人")]
        public string? ContactPerson { get; set; }

        [Display(Name = "聯絡人電話")]
        public string? ContactPhone { get; set; }

        [Display(Name = "聯絡人Email")]
        public string? ContactEmail { get; set; }

        [Display(Name = "負責人")]
        public string? Principal { get; set; }

        [Display(Name = "公司電話")]
        public string? CompanyPhone { get; set; }

        [Display(Name = "公司地址")]
        public string? CompanyAddress { get; set; }

        [Display(Name = "公司簡介")]
        public string? CompanyProfile { get; set; }

    }
}
