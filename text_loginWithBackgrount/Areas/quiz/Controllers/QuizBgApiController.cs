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
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Route("Api/QuizBg/[action]")]
    public class QuizBgApiController : Controller
    {

        private readonly studentContext _context;

        private readonly IMongoCollection<QuestionOrder> _questionOrderCollection;
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<StudentAnswer> _studentAnswerCollection;

        public QuizBgApiController(studentContext context, IOptions<MongoDbSettings> mongoDbSettings)
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


        // 取得擁有測驗(老師)
        [HttpGet("{teacherId}")]
        public IActionResult GetQuiz(int teacherId)
        {
            var quizzes = _context.TQuizQuizzes
                .Where(p => p.FTeacherId == teacherId)
                .Select(p => new Quiz
                {
                    id = p.FQuizId,
                    name = p.FQname,
                    note = p.FNote,
                    code = p.FQcode,
                    limitTime = p.FLimitTime,
                    teacherId = p.FTeacherId,
                    isPublic = p.FPublic,
                    isClosed = p.FClosed,
                    createTime = p.FCreateTime
                });

            return Json(quizzes);
        }


        // 新增測驗(老師)
        [HttpPost]
        public IActionResult CreateQuiz([FromBody] Quiz quiz)
        {
            TQuizQuiz newQuiz;
            if (ModelState.IsValid)
            {
                newQuiz = new TQuizQuiz
                {
                    FQuizId = quiz.id,
                    FQname = quiz.name,
                    FNote = quiz.note,
                    FQcode = quiz.code,
                    FLimitTime = quiz.limitTime,
                    FTeacherId = quiz.teacherId,
                    FPublic = quiz.isPublic,
                    FClosed = quiz.isClosed,
                    FCreateTime = quiz.createTime
                };
                _context.TQuizQuizzes.Add(newQuiz);
                _context.SaveChanges();


                QuestionOrder newQuestionOrder = new QuestionOrder
                {
                    QuizID = newQuiz.FQuizId,
                };

                try
                {

                     _questionOrderCollection.InsertOneAsync(newQuestionOrder);

                    return Ok(newQuiz.FQuizId);
                }
                catch (Exception ex)
                {
                    return BadRequest("新增失敗");
                }
                


            }
            else
            {
                return BadRequest("新增失敗");
            }


        }


        // 修改測驗資訊(老師)
        [HttpPost]
        public IActionResult UpdateQuiz([FromBody] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                var data = _context.TQuizQuizzes.FirstOrDefault(q => q.FQuizId == quiz.id);
                data.FQname = quiz.name;
                data.FNote = quiz.note;
                data.FLimitTime = quiz.limitTime;
                data.FPublic = quiz.isPublic;
                data.FClosed = quiz.isClosed;

                _context.SaveChanges();
                return Ok("修改成功");
            }
            else
            {
                return BadRequest("新增失敗");
            }
        }



        // 刪除測驗(老師)
        [HttpPost]
        public IActionResult DeleteQuiz([FromBody] Quiz quiz)
        {

            var data = _context.TQuizQuizzes.FirstOrDefault(q => q.FQuizId == quiz.id);

            if (data == null)
            {
                return BadRequest("刪除失敗");
            }

            _context.TQuizQuizzes.Remove(data);
            _context.SaveChanges();
            return Ok("修改成功");

        }


        // 開啟或關閉測驗(老師)
        [HttpPost]
        public IActionResult OpenOrClose([FromForm] int quizId)
        {
            var quiz = _context.TQuizQuizzes.Where(x => x.FQuizId == quizId).FirstOrDefault();

            if (quiz == null)
            {
                return BadRequest("修改失敗");
            }

            quiz.FClosed = quiz.FClosed == 0 ? 1 : 0;
            _context.SaveChanges();

            return Ok("修改成功");
        }


        // 取得某測驗資訊(老師)
        [HttpGet("{quizId}")]
        public IActionResult GetInfo(int quizId)
        {
            var quiz = _context.TQuizQuizzes
                .Where(p => p.FQuizId == quizId)
                .Select(p => new Quiz
                {
                    id = p.FQuizId,
                    name = p.FQname,
                    note = p.FNote,
                    code = p.FQcode,
                    limitTime = p.FLimitTime,
                    teacherId = p.FTeacherId,
                    isPublic = p.FPublic,
                    isClosed = p.FClosed,
                    createTime = p.FCreateTime
                })
                .FirstOrDefault();

            return Json(quiz);
        }



        // 取得某測驗考題(老師)
        [HttpGet("{quizId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetQuestions(int quizId)
        {
            var questionOrder = await _questionOrderCollection.Find(q => q.QuizID == quizId).FirstOrDefaultAsync();

            if (questionOrder == null)
            {
                return Ok("無考題資料");
            }

            var questionIds = questionOrder.QuestionOrderList;

            try
            {
                var filter = Builders<Question>.Filter.In(q => q.Id, questionIds);

                var questions = await _questionCollection.Find(filter).ToListAsync();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest($"查詢問題時出現錯誤：{ex.Message}");
            }



        }






        // 儲存測驗考題
        [HttpPost]
        public async Task<ActionResult<IEnumerable<string>>> SaveQuesions([FromBody] QuizInfo data)
        {
            int quizId = data.quizId;
            List<Question> questionList = data.questions;

            List<string> ids = new List<string>();

            foreach (var question in questionList)
            {
                if (!string.IsNullOrEmpty(question.Id))
                {
                    var filter = Builders<Question>.Filter.Eq(q => q.Id, question.Id);
                    var update = Builders<Question>.Update
                        .Set(q => q.Type, question.Type)
                        .Set(q => q.QuestionText, question.QuestionText)
                        .Set(q => q.Image, question.Image)
                        .Set(q => q.Options, question.Options)
                        .Set(q => q.Answer, question.Answer);

                    await _questionCollection.UpdateOneAsync(filter, update);

                    ids.Add(question.Id);
                }
                else
                {

                    await _questionCollection.InsertOneAsync(question);
                    ids.Add(question.Id);
                }
            }

            try
            {
                var filter = Builders<QuestionOrder>.Filter.Eq(q => q.QuizID, quizId);
                var questionOrder = await _questionOrderCollection.Find(filter).FirstOrDefaultAsync();
                questionOrder.QuestionOrderList = new List<string>();
                questionOrder.QuestionOrderList.AddRange(ids);

                await _questionOrderCollection.ReplaceOneAsync(filter, questionOrder);

                return Ok("更新成功");
            }
            catch (Exception ex)
            {
                return BadRequest("更新失敗");
            }
        }
    }
}
