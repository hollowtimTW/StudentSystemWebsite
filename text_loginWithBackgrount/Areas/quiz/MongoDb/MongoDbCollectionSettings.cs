namespace text_loginWithBackgrount.Areas.quiz.Models
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string QuestionOrderCollection { get; set; } = null!;
        public string QuestionCollection { get; set; } = null!;
        public string StudentAnswerCollection { get; set; } = null!;
    }
}
