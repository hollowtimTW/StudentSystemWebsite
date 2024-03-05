using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.ViewModels
{
    public class MemberViewModel
    {
        [Required(ErrorMessage = "姓名欄位未填寫")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "姓名長度不正確")]
        public string? 姓名 { get; set; }

        [Required(ErrorMessage = "性別欄位為空")]
        [StringLength(1, ErrorMessage = "性別長度不正確")]
        public string? 性別 { get; set; }

        [Required(ErrorMessage = "身分證字號未填寫")]
        [RegularExpression(@"^[A-Za-z][0-9]{9}$", ErrorMessage = "身分證字號格式不正確")]
        public string? 身分證字號 { get; set; }

        [EmailAddress(ErrorMessage = "信箱格式不正確")]
        public string? 信箱 { get; set; }

        [StringLength(10, MinimumLength = 8, ErrorMessage = "手機長度為8~10碼")]
        public string? 手機 { get; set; }

        public string? 地址 { get; set; }

        [Required(ErrorMessage = "密碼欄位未填寫")]
        public string? 密碼 { get; set; }

        [Compare("密碼", ErrorMessage = "密碼與確認密碼不相符")]
        public string? 密碼驗證 { get; set; }

        public IFormFile? 圖片 { get; set; }

        public DateTime? 生日 { get; set; }

        public string? 學校 { get; set; }

        public string? 科系 { get; set; }

        public string? 學位 { get; set; }

        public string? 畢肄 { get; set; }

    }
}
