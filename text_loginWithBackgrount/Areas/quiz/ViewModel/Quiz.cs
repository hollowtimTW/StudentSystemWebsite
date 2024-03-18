using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.quiz.ViewModel
{
    public class Quiz
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? note { get; set; }
        public string? code { get; set; }
        public int? limitTime { get; set; }
        public int? teacherId { get; set; }
        public int? isPublic { get; set; }
        public int? isClosed { get; set; }
        public DateTime? createTime { get; set; }

    }
}
