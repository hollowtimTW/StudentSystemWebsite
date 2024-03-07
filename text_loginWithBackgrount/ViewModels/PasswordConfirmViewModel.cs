using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.ViewModels
{
    public class PasswordConfirmViewModel
    {
        public string? email { get; set; }

        public string? token { get; set; }

        [Required(ErrorMessage = "密碼欄位未填寫")]
        public string? password { get; set; }

        [Compare("password", ErrorMessage = "密碼與確認密碼不相符")]
        public string? passwordConfirm { get; set; }
    }
}
