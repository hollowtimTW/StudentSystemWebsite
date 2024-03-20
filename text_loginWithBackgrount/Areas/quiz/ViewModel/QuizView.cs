

namespace text_loginWithBackgrount.Areas.quiz.ViewModel
{
    public class QuizView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
        public int? LimitTime { get; set; }
        public string TeacherId { get; set; }
        public int? IsPublic { get; set; }
        public int? IsClosed { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Count { get; set; }
        public int HasRecord { get; set; }
    }
}
