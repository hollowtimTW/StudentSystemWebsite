using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.ViewModels
{
    public class MemberViewModelWithPwd : MemberViewModel
    {
        [StringLength(16, MinimumLength = 4, ErrorMessage = "密碼長度不正確")]
        [Required(ErrorMessage = "密碼欄位未填寫")]
        public string? 密碼 { get; set; }

        [StringLength(16, MinimumLength = 4, ErrorMessage = "密碼長度不正確")]
        [Compare("密碼", ErrorMessage = "密碼與確認密碼不相符")]
        public string? 密碼驗證 { get; set; }
    }
}
