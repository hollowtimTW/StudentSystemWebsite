using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.job_vacancy.ViewModels
{
    public class ResumeCreateViewModel
    {
        [Display(Name = "學員ID")]
        public int StudentID { get; set; }

        [Display(Name = "履歷ID")]
        public int ResumeID { get; set; }

        [Display(Name = "履歷名稱")]
        [Required(ErrorMessage = "履歷名稱未填寫")]
        [StringLength(50, ErrorMessage = "履歷名稱過長")]
        public string? ResumeTitle { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "姓名未填寫")]
        [StringLength(50, ErrorMessage = "姓名過長")]
        public string? Name { get; set; }

        [Display(Name = "照片")]
        public byte[]? Photo { get; set; }

        [Display(Name = "性別")]
        public string? Gender { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        public DateTime? Birth { get; set; }

        [Display(Name = "Email")]
        [StringLength(50, ErrorMessage = "Email過長")]
        [EmailAddress(ErrorMessage = "Email格式錯誤")]
        public string? Email { get; set; }

        [Display(Name = "手機")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "手機號碼格式不正確")]
        [StringLength(10, ErrorMessage = "手機號碼過長")]
        public string? Phone { get; set; }

        [Display(Name = "學校")]
        [Required(ErrorMessage = "畢業學校名稱未填寫")]
        [StringLength(50, ErrorMessage = "學校名稱過長")]
        public string? School { get; set; }

        [Display(Name = "科系")]
        [Required(ErrorMessage = "科系名稱未填寫")]
        [StringLength(50, ErrorMessage = "科系名稱過長")]
        public string? Department { get; set; }

        [Display(Name = "學位")]
        [Required(ErrorMessage = "學位未選取")]
        public string? Academic { get; set; }

        [Display(Name = "畢肄")]
        [Required(ErrorMessage = "畢肄狀況未選取")]
        public string? Graduated { get; set; }


        [Display(Name = "狀態")]
        public string? ResumeStatus { get; set; }

        [Display(Name = "希望職稱")]
        [Required(ErrorMessage = "希望職稱未填寫")]
        [StringLength(10, ErrorMessage = "希望職稱過長")]
        public string? HopeJobTitle { get; set; }

        [Display(Name = "希望薪水待遇")]
        [Required(ErrorMessage = "希望薪水待遇未填寫")]
        [StringLength(10, ErrorMessage = "希望薪水待遇過長")]
        public string? HopeSalary { get; set; }

        [Display(Name = "希望工作地點")]
        [Required(ErrorMessage = "希望工作地點未填寫")]
        [StringLength(50, ErrorMessage = "希望工作地點過長")]
        public string? HopeLocation { get; set; }

        [Display(Name = "專長技能")]
        public string? Skill { get; set; }

        [Display(Name = "語文能力")]
        public string? Language { get; set; }

        [Display(Name = "工作經驗")]
        public string? WorkExperience { get; set; }

        [Display(Name = "工作性質")]
        [Required(ErrorMessage = "希望工作性質未選取")]
        public string? WorkType { get; set; }

        [Display(Name = "工作時段")]
        [Required(ErrorMessage = "希望工作時段未選取")]
        public string? WorkTime { get; set; }

        [Display(Name = "配合輪班")]
        public string? WorkShift { get; set; }

        [Display(Name = "自傳")]
        public string? Autobiography { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateDateTime { get; set; }

        [Display(Name = "最後更新時間")]
        public DateTime? LastUpdate { get; set; }

    }
}
