using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Areas.course_management.Controllers
{
    
    [Area("course_management")]
    public class courseController : Controller
    {
        private readonly studentContext _context;

        public courseController(studentContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 訪客看到的頁面，從現在到未來六個月之內的班級，取三個班。
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {

                var now = DateTime.Now;
                var sixMonthsLater = now.AddMonths(6);

                var classContext = _context.T課程班級s
                    .Include(t => t.班級導師)
                    .Where(t => t.入學日期 > now && t.入學日期 <= sixMonthsLater)
                    .Take(3);

                //如果null前端會顯示不同的資訊
                var classlist = await classContext.ToListAsync();
                return View(classlist);

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }
    }
}
