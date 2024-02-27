using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程科目分類
{
    public class CategoryPictureViewModel
    {
        public int 科目類別id { get; set; }

        [Required(ErrorMessage = "科目類別名稱是必填的")]
        public string? 科目類別名稱 { get; set; }

        public string? 科目類別封面名稱 { get; set; }

        public IFormFile? 科目類別封面 { get; set; }

        public int 狀態 { get; set; }

    }
}
