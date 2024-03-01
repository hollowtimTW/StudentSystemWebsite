using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級科目評分;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Areas.course_management.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("course_management")]
    public class courseforStudentController : Controller
    {
        private readonly studentContext _context;

        public courseforStudentController(studentContext context)
        {
            _context = context;
        }
     

        async public Task<IActionResult> Index(int? id)
        {
            
            var classId = _context.T課程學生班級s
            .Where(t => t.學生id == id)
            .Select(t => t.班級id)
            .FirstOrDefault();

           
            if (classId == null || classId == 0)
            {
                ViewBag.state = false;
                ViewBag.message = "學生尚未被添加到班級中";
                return View();
            }

            var classContext = _context.T課程班級s
            .Where(t => t.班級id == classId)
            .FirstOrDefault();

            var courseContext = await _context.T課程班級科目s
            .Include(t => t.老師)
            .Include(t => t.科目)
            .Where(t => t.班級id == classId)
            .ToListAsync();


            if (courseContext.Count == 0)
            {
                ViewBag.state = false;
                ViewBag.message = "班級科目尚未被添加到班級中";
                return View();
            }

            var TimeContextQuery = _context.T課程班級科目s
            .Include(t => t.T課程課程s)
            .Where(t => t.班級id == classId)
            .SelectMany(subject => subject.T課程課程s);

            var TimeContext = await TimeContextQuery.ToListAsync();

            if (!TimeContext.Any())
            {
                ViewBag.State = false;
                ViewBag.Message = "都未安排上課時間";
                return View();
               
            }

            ViewBag.State = true;
            ViewBag.Class = classContext;
            ViewBag.ClassCourse = courseContext;

            return View(TimeContext);


        }

        [HttpGet("/course_management/courseforStudent/Edit/{id}/{studentid}")]
        async public Task<IActionResult> Edit(int? id, int? studentid)
        {
            if (id == null || _context.T課程評分主表s == null)
            {
                return NotFound();
            }

            var resultClass = CheckClassCourseSatus(id);
            if (!resultClass)
            {
                return Ok("老師還尚未開啟填寫表單");
            }

            var resultForm = CheckRateFormSatus(id, studentid);

            if (!resultForm)
            {
                return Ok("已經填寫過了");
            }

            var t課程評分表ViewModel = await _context.T課程評分主表s
            .Where(m => m.班級科目id == id && m.學生id == studentid)
            .Select(m => new RateFormViewModel
            {
                評分主表id = m.評分主表id,
                班級科目id = m.班級科目id,
                學生id = m.學生id,
                T課程評分s = m.T課程評分s.ToList(),
                評分s = new Dictionary<int, int>()
            })
            .FirstOrDefaultAsync();

            if (t課程評分表ViewModel == null)
            {
                return NotFound();
            }


            return PartialView("_EditPartial", t課程評分表ViewModel);
        }

        [HttpPost("/course_management/StudentHome/Edit/{id}/{studentid}")]
        public async Task<IActionResult> Edit(int? id, int? studentid, RateFormViewModel ViewModel)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {

                        var t課程評分主表 = await _context.T課程評分主表s.FindAsync(ViewModel.評分主表id);
                        if (t課程評分主表 == null)
                        {
                            return NotFound();
                        }

                        t課程評分主表.班級科目id = ViewModel.班級科目id;
                        t課程評分主表.學生id = ViewModel.學生id;
                        t課程評分主表.提交時間 = DateTime.Now;
                        t課程評分主表.狀態 = 2;
                        t課程評分主表.改進意見 = ViewModel.改進意見;
                        _context.Update(t課程評分主表);

                        foreach (var kvp in ViewModel.評分s)
                        {
                            var t課程評分 = await _context.T課程評分s.FindAsync(kvp.Key);
                            if (t課程評分 != null)
                            {
                                t課程評分.評分 = kvp.Value;
                                _context.Update(t課程評分);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程評分主表Exists(ViewModel.評分主表id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(ViewModel.班級科目id);


            return RedirectToAction(nameof(Index), new { id = ViewModel.學生id });
        }

        public bool CheckClassCourseSatus(int? ClassCourseId)
        {

            var data = _context.T課程班級科目s.FirstOrDefault(tc => tc.班級科目id == ClassCourseId);

            if (data.狀態 == 7)
            {

                //已經開啟可以填寫
                return true;

            }
            else
            {
                return false;
            }
        }

        public bool CheckRateFormSatus(int? ClassCourseId, int? StudentId)
        {

            var data = _context.T課程評分主表s.FirstOrDefault(tc => tc.學生id == StudentId && tc.班級科目id == ClassCourseId);

            if (data.狀態 == 2)
            {

                //已經填寫過了
                return false;

            }
            else
            {
                return true;
            }
        }

        private bool T課程評分主表Exists(int id)
        {
            return (_context.T課程評分主表s?.Any(e => e.評分主表id == id)).GetValueOrDefault();
        }
    }
}
