using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.ordering_system.Models
{
    public class menuDeatailDTO : IValidatableObject
    {
        [Required(ErrorMessage = "餐點名稱不可空白")]
        public string 餐點名稱 { get; set; }
        [Required(ErrorMessage = "餐點售價不可空白")]
        public int? 餐點售價 { get; set; }
        [Display(Name = "餐點描述")]
        [MaxLength(length: 50, ErrorMessage = "餐點描述字數不可超過50字")]
        public string? 餐點描述 { get; set; }
        public IFormFile? file { get; set; }
        public int menuID { get; set; }
        public int storeID { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //目前不用使用自訂驗證
            yield break;
        }
    }
}
