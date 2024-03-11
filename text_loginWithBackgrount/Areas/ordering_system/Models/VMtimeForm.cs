using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.ordering_system.Models
{
    public class VMtimeForm : IValidatableObject
    {
        [Required(ErrorMessage = "名稱不可空白")]
        [Display(Name = "titletime")]
        public string titletime { get; set; }
        public string weeklist { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string ID { get; set; }
        public string? timeShow { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
