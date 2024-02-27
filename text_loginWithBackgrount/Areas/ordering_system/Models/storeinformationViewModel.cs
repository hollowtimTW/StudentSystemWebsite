using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.ordering_system.Models
{
    public class storeinformationViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "店家名稱不可空白")]
        [Display(Name = "店家名稱")]
        public string? storeName { get; set; }

        [Required(ErrorMessage = "店家電話不可空白")]
        [Phone(ErrorMessage = "電話格式錯誤")]
        [Display(Name = "聯絡電話")]
        public string? storePhone { get; set; }

        [Required(ErrorMessage = "聯絡資訊不可空白")]
        [EmailAddress(ErrorMessage = "信箱格式錯誤")]
        [Display(Name = "電子郵件")]
        public string? storeEmail { get; set; }

        [Required(ErrorMessage = "地址不可空白")]
        [Display(Name = "地址")]
        public string? storeAdress { get; set; }

        [Display(Name = "店家介紹")]
        [MaxLength(length:100, ErrorMessage = "店家介紹字數不可超過100字")]
        public string? storeinformation { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //目前不用使用自訂驗證
            yield break;
        }
    }
}
