﻿using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using text_loginWithBackgrount.Areas.quiz.Models;
using text_loginWithBackgrount.Areas.quiz.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student,teacher")]
    [Route("[area]/[controller]/[action]")]
    [Area("quiz")]
    public class QuizController : Controller
    {
        private readonly studentContext _context;

        private readonly IMongoCollection<QuestionOrder> _questionOrderCollection;
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<StudentAnswer> _studentAnswerCollection;


        public QuizController(studentContext context, IOptions<MongoDbSettings> mongoDbSettings)
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

        public IActionResult StdIndex()
        {
            string studentId = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;

            var quizList = _context.TQuizQuizzes
                .Select(p => new QuizView
                {
                    Id = p.FQuizId,
                    Name = p.FQname,
                    Note = p.FNote,
                    Code = p.FQcode,
                    LimitTime = p.FLimitTime,
                    TeacherId = p.FTeacher.姓名,
                    IsPublic = p.FPublic,
                    IsClosed = p.FClosed,
                    CreateTime = p.FCreateTime,
                    Count = _context.TQuizRecords.Count(r => r.FQuizId == p.FQuizId),
                    HasRecord = _context.TQuizRecords.Any(r => r.FQuizId == p.FQuizId && r.FStudentId.ToString() == studentId && r.FState==1) ? 1 : 0
                })
                .OrderByDescending(p => p.IsClosed)
                .ThenByDescending(p => p.Count)
                .ToList();

            return View(quizList);
        }



        [HttpGet("{quizCode}")]
        public IActionResult Quiz(string quizCode)
        {
            string studentId = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;

            var quiz = _context.TQuizQuizzes
                .Where(p => p.FQcode == quizCode)
                .FirstOrDefault();

            if (quiz == null)
            {
                return BadRequest("無測驗");
            }


            var questionOrder = _questionOrderCollection.Find(q => q.QuizID == quiz.FQuizId).FirstOrDefault();

            if (questionOrder == null || questionOrder.QuestionOrderList == null)
            {
                return BadRequest("無考題");
            }

            // 考題
            List<Question> questions = _questionCollection.Find(q => questionOrder.QuestionOrderList.Contains(q.Id)).ToList();



            var record = _context.TQuizRecords
                .Where(p=>p.FQuizId==quiz.FQuizId && p.FStudentId.ToString()==studentId)
                .FirstOrDefault();

            if (record == null)
            {
                return BadRequest("無紀錄");
            }

            if(record.FState == 1)
            {
                return RedirectToAction("StdIndex");
            }


            var studentAnswer = _studentAnswerCollection
                .Find(sa => sa.RecordId == record.FRecordId)
                .FirstOrDefault();

            if (record.FStartTime == null || record.FEndTime == null)
            {
                DateTime currentTime = DateTime.Now;
                DateTime endTime = currentTime.AddMinutes(quiz.FLimitTime??0);
                string currentTimeFormatted = currentTime.ToString("yyyy/MM/dd HH:mm:ss");
                string endTimeFormatted = endTime.ToString("yyyy/MM/dd HH:mm:ss");
                record.FStartTime = currentTimeFormatted;
                record.FEndTime = endTimeFormatted;
                _context.TQuizRecords.Update(record);
                _context.SaveChanges();
            }


            StdQuiz data = new StdQuiz
            {
                Quiz = quiz,
                Record = record,
                Questions = questions,
                StudentAnswer = studentAnswer
            };

            return View(data);
        }


        [HttpGet("{quizCode}")]
        public IActionResult Result(string quizCode)
        {
            return View();
        }
    }
}
