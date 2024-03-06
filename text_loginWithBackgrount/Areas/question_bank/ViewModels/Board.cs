using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace text_loginWithBackgrount.Areas.question_bank.ViewModels
{
    public class Board
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [Display(Name = "標題")]
        public string Title { get; set; } = null!;
        [Display(Name = "內容")]
        public string Content { get; set; } = null!;
        [Display(Name = "發布者")]
        public string Author { get; set; } = null!;
    }
}
