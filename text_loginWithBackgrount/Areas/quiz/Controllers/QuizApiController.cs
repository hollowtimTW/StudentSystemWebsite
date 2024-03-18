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
        //[HttpGet("{quizCode}")]
        //public IActionResult GetQuiz(string quizCode)
        //{
        //    var quiz = _context.TQuizQuizzes
        //        .Where(p => p.FQcode == quizCode)
        //        .Select(p => new
        //        {
        //            id = p.FQuizId,
        //            name = p.FQname,
        //            note = p.FNote,
        //            code = p.FQcode,
        //            limitTime = p.FLimitTime,
        //            teacherId = p.FTeacher.姓名,
        //            isPublic = p.FPublic,
        //            isClosed = p.FClosed,
        //            createTime = p.FCreateTime
        //        })
        //        .FirstOrDefault();

        //    if (result == null)
        //    {
        //        return BadRequest("匹配錯誤");
        //    }

        //    return Json(result);
        //}





    }
}
