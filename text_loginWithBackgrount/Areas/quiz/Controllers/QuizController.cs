using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using text_loginWithBackgrount.Areas.quiz.ViewModel;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student,teacher")]
    [Area("quiz")]
    public class QuizController : Controller
    {
        private readonly studentContext _context;

 
        public QuizController(studentContext context)
        {
            _context = context;
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
                    HasRecord = _context.TQuizRecords.Any(r => r.FQuizId == p.FQuizId && r.FStudentId.ToString() == studentId) ? 1 : 0
                })
                .OrderByDescending(p => p.IsClosed)
                .ThenByDescending(p => p.Count  )
                .ToList();

            return View(quizList);
        }


        public IActionResult Quiz()
        {
            return View();
        }
    }
}
