using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using text_loginWithBackgrount.Areas.quiz.Models;
using text_loginWithBackgrount.Areas.quiz.ViewModel;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Route("Api/Quiz/[action]")]
    public class QuizApiController : Controller
    {

        private readonly studentContext _context;

        private readonly IMongoCollection<QuestionOrder> _questionOrderCollection;
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<StudentAnswer> _studentAnswerCollection;

        public QuizApiController(studentContext context, IOptions<MongoDbSettings> mongoDbSettings)
        {
            _context = context;

            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _questionOrderCollection = mongoDatabase.GetCollection<QuestionOrder>(
                mongoDbSettings.Value.QuestionOrderCollection);

            _questionCollection = mongoDatabase.GetCollection<Question>(
                mongoDbSettings.Value.QuestionCollection);

            _studentAnswerCollection = mongoDatabase.GetCollection<StudentAnswer>(
                mongoDbSettings.Value.StudentAnswerCollection);
        }



        // 代碼搜尋
        [HttpGet("{quizCode}")]
        public IActionResult PrivateQuiz(string quizCode)
        {
            var quiz = _context.TQuizQuizzes
                .Where(p => p.FQcode == quizCode)
                .Select(p => new
                {
                    id = p.FQuizId,
                    name = p.FQname,
                    note = p.FNote,
                    code = p.FQcode,
                    limitTime = p.FLimitTime,
                    teacherId = p.FTeacher.姓名,
                    isPublic = p.FPublic,
                    isClosed = p.FClosed,
                    createTime = p.FCreateTime,
                    count = _context.TQuizRecords.Count(r => r.FQuizId == p.FQuizId)
                })
                .FirstOrDefault();

            if (quiz == null)
            {
                return BadRequest("無此代碼");
            }

            return Json(quiz);
        }


        // 查看有無未完成測驗
        [HttpGet("{studentId}")]
        public IActionResult UnFinishRecord(int studentId)
        {

            var record = _context.TQuizRecords
                .Where(p => p.FStudentId == studentId && p.FState == 0)
                .Select(p =>new
                {
                    code = p.FQuiz.FQcode,
                    recordId = p.FRecordId
                })
                .FirstOrDefault();

            return Json(record);
        }


        // 查看測驗狀態
        [HttpGet("{recordId}")]
        public IActionResult CheckState(int recordId)
        {
            var record = _context.TQuizRecords
                .Where(p => p.FRecordId == recordId)
                .Select(p => p.FState)
                .FirstOrDefault();

            return Json(record);
        }



        // 設定紀錄
        [HttpPost]
        public IActionResult CheckRecord([FromBody] TQuizRecord r)
        {
            var record = _context.TQuizRecords
                .Where(p=>p.FQuizId==r.FQuizId && p.FStudentId==r.FStudentId)
                .FirstOrDefault();

            if(record == null)
            {
                record = new TQuizRecord
                {
                    FQuizId = r.FQuizId,
                    FStudentId = r.FStudentId,
                    FState = 0
                };

                _context.TQuizRecords.Add(record);
                _context.SaveChanges();
            }

            var studentAnswer = _studentAnswerCollection
                .Find(sa => sa.RecordId == record.FRecordId)
                .FirstOrDefault();

            if(studentAnswer == null)
            {
                studentAnswer = new StudentAnswer
                {
                    RecordId = record.FRecordId,
                    Answers = new List<List<int>>()
                };
                _studentAnswerCollection.InsertOne(studentAnswer);
            }

            return Json(record);
        }



        // 計分
        [HttpPost]
        public IActionResult SetScore([FromBody] StudentAnswer r)
        {
            var record = _context.TQuizRecords.FirstOrDefault(p => p.FRecordId == r.RecordId);

            if (record == null)
            {
                return NotFound("Record not found.");
            }

            var quiz = _context.TQuizQuizzes.FirstOrDefault(p => p.FQuizId == record.FQuizId);

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            var questionOrder = _questionOrderCollection.Find(q => q.QuizID == quiz.FQuizId).FirstOrDefault();

            if (questionOrder == null)
            {
                return NotFound("Question order not found for the given QuizID.");
            }

            var questions = _questionCollection.Find(q => questionOrder.QuestionOrderList.Contains(q.Id)).ToList();

            if (questions.Count == 0)
            {
                return NotFound("No questions found for the given QuizID.");
            }

            int totalQuestions = questions.Count;
            int correctAnswers = 0;

            for (int i = 0; i < totalQuestions; i++)
            {
                var correctAnswer = questions[i].Answer;

                if (r.Answers.Count > i && Enumerable.SequenceEqual(r.Answers[i], correctAnswer))
                {
                    correctAnswers++;
                }
            }

			var studentAnswer = _studentAnswerCollection
				.Find(sa => sa.RecordId == record.FRecordId)
				.FirstOrDefault();

            studentAnswer.Answers = r.Answers;
			_studentAnswerCollection.ReplaceOne(sa => sa.Id == studentAnswer.Id, studentAnswer);

			decimal accuracy = (decimal)correctAnswers / totalQuestions;

            accuracy = Math.Round(accuracy, 2);

            record.FState = 1;
            record.FRate = accuracy;

            _context.TQuizRecords.Update(record);
            _context.SaveChanges();

           return Ok(accuracy);
   
        }



        // 放棄作答
        [HttpGet("{recordId}")]
        public IActionResult GiveUpScore(int recordId)
        {
            var studentAnswer = _studentAnswerCollection
                .Find(sa => sa.RecordId == recordId)
                .FirstOrDefault();

            if (studentAnswer == null)
            {
                return BadRequest("無答題記錄");
            }

            var record = _context.TQuizRecords
                .Where(p => p.FRecordId == recordId)
                .FirstOrDefault();

            if (studentAnswer == null)
            {
                return BadRequest("無記錄");
            }


            var questionOrder = _questionOrderCollection
                .Find(q => q.QuizID == record.FQuizId)
                .FirstOrDefault();

            if (questionOrder == null)
            {
                return NotFound("Question order not found for the given QuizID.");
            }

            var questions = _questionCollection
                .Find(q => questionOrder.QuestionOrderList.Contains(q.Id))
                .ToList();

            if (questions.Count == 0)
            {
                return NotFound("No questions found for the given QuizID.");
            }


            var totalQuestions = questions.Count;
            var correctAnswers = 0;

            for (int i = 0; i < totalQuestions; i++)
            {
                var correctAnswer = questions[i].Answer;

                if (studentAnswer.Answers.Count > i && Enumerable.SequenceEqual(studentAnswer.Answers[i], correctAnswer))
                {
                    correctAnswers++;
                }
            }

            // 计算准确率
            decimal accuracy = (decimal)correctAnswers / totalQuestions;
            accuracy = Math.Round(accuracy, 2);

            record.FState = 1;
            record.FRate = accuracy;

            _context.TQuizRecords.Update(record);
            _context.SaveChanges();


            return Ok(accuracy);
        }




        // 儲存答題記錄
        [HttpPost]
        public IActionResult SaveAnswers([FromBody] StudentAnswer r)
        {
            var studentAnswer = _studentAnswerCollection
                .Find(sa => sa.RecordId == r.RecordId)
                .FirstOrDefault();

            studentAnswer.Answers = r.Answers;
            _studentAnswerCollection.ReplaceOne(sa => sa.Id == studentAnswer.Id, studentAnswer);
            return Ok("儲存成功");

        }





        [HttpGet]
        public IActionResult GetRecords()
        {
            string studentId = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;

            var recordsList = _context.TQuizRecords
                .Where(p => p.FStudentId.ToString() == studentId && p.FState == 1)
                .Select(p => new
                {
                    name = p.FQuiz.FQname,
                    code = p.FQuiz.FQcode,
                    rate = p.FRate,
                    date = p.FStartTime,
                })
                .ToList();

            return Json(recordsList);
        }



    }
}
