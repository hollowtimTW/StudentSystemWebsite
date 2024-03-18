using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace text_loginWithBackgrount.Areas.quiz.Models
{
 
    public class QuestionOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("QuizID")]
        public int QuizID { get; set; }

        [BsonElement("QuestionOrder")]
        public List<string> QuestionOrderList { get; set; }
    }

    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Type")]
        public int Type { get; set; }

        [BsonElement("Question")]
        public string? QuestionText { get; set; }

        [BsonElement("Image")]
        public string? Image { get; set; }

        [BsonElement("Options")]
        public List<string> Options { get; set; }

        [BsonElement("Answer")]
        public List<int> Answer { get; set; }
    }

    public class StudentAnswer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int RecordId { get; set; }
        public int[][] Answers { get; set; }

    }
}
