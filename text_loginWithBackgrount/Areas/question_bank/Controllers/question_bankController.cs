using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.question_bank.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("question_bank")]
    public class question_bankController : Controller
    {
        private readonly studentContext _context;
        public question_bankController(studentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ExamSh()
        {
            var subjects = _context.T課程科目s.Select(x => x.科目名稱).ToList();
            var classes = _context.T課程班級s.Select(x => x.班級名稱).ToList();
        
            ViewBag.subjects = subjects;
            ViewBag.classes = classes;

            return View();
        }

        public IActionResult Score()
        {
            return View();
        }

		public IActionResult Qbank()
		{
			return View();
		}
	}
}
