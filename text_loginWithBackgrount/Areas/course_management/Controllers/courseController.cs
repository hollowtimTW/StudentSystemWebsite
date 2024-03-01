using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Areas.course_management.Controllers
{
    /// <summary>
    /// 訪客看到的頁面
    /// </summary>
    [Area("course_management")]
    public class courseController : Controller
    {
        private readonly studentContext _context;

        public courseController(studentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> HomeIndexCard()
        {
            var now = DateTime.Now;
            var sixMonthsLater = now.AddMonths(6);

            var studentContext = _context.T課程班級s
                .Include(t => t.班級導師)
                .Where(t => t.入學日期 > now && t.入學日期 <= sixMonthsLater)
                .Take(3);

            return PartialView("_HomeIndexCardPartial", await studentContext.ToListAsync());
        }
    }
}
