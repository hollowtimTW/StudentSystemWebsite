using text_loginWithBackgrount.Areas.quiz.Models;

namespace text_loginWithBackgrount.Areas.quiz.ViewModel
{
    public class QuizInfo
    {
        public int quizId { get; set; }
        public List<Question> questions { get; set; }

    }
}
