using Class_system_Backstage_pj.Models;
using text_loginWithBackgrount.Areas.quiz.Models;

namespace text_loginWithBackgrount.Areas.quiz.ViewModel
{
    public class StdQuiz
    {
        public TQuizQuiz Quiz { get; set; }
        public TQuizRecord Record { get; set; }
        public List<Question> Questions { get; set; }
        public StudentAnswer StudentAnswer { get; set; }

    }
}
