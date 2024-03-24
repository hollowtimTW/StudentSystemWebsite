using Class_system_Backstage_pj.Models;
using text_loginWithBackgrount.Areas.class_discuss.ViewModels;

namespace text_loginWithBackgrount.Areas.class_discuss.DTO
{
    public class MessageDTO
    {
        public int TotalMessages { get; set; }
        public int TotalPages { get; set; }
        public List<MessageViewModel>? Result { get; set; }
    }
}
