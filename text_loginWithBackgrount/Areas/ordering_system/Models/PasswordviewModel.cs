using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.ordering_system.Models
{
    public class PasswordviewModel : IValidatableObject
    {
        [Required(ErrorMessage = "密碼欄位必填")]
        [Display(Name = "密碼")]
        [StringLength(maximumLength: 10, MinimumLength = 8, ErrorMessage = "至少需要八個英文與數字")]
        [RegularExpression(@"^(?=.*[A-Z])[A-Za-z0-9]+$", ErrorMessage = "必須包含一個大寫英文，且只允許字母和數字。")]

        public string? newPassword { get; set; }

        [Required(ErrorMessage = "密碼欄位必填")]
        [Display(Name = "二次輸入密碼")]
        [StringLength(maximumLength: 10, MinimumLength = 8, ErrorMessage = "至少需要八個英文與數字")]
        public string? newPassword_again { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (newPassword != newPassword_again)
            {
                yield return new ValidationResult("密碼輸入有錯，重新輸入", new string[] { "newPassword_again" });
            }
        }
    }
}
