using Class_system_Backstage_pj.Models;
using text_loginWithBackgrount.Areas.class_discuss.ViewModels;

namespace text_loginWithBackgrount.Areas.class_discuss.DTO
{
    public class CardPageDTO
    {
        public int TotalPages { get; set; }
        public List<ArtCardViewModel>? Result { get; set; }
        public List<T討論子版>? TypeResult { get; set; }

    }
}
