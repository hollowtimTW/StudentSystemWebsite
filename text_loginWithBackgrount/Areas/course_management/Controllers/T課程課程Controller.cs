using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using text_loginWithBackgrount.Areas.course_management.ViewModel.time;
using System.Globalization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程課程Controller : Controller
    {
        private readonly studentContext _context;

        public T課程課程Controller(studentContext context)
        {
            _context = context;
        }

        // GET: course_management/T課程課程
        public async Task<IActionResult> classCourseTimeIndex(int? id)
        {
            try
            {
                var TimeContext = _context.T課程班級科目s
                .Include(t => t.T課程課程s)
                .Where(t => t.班級id == id)
                .SelectMany(subject => subject.T課程課程s);

                var courseContext = _context.T課程班級科目s
                    .Include(t => t.老師)
                    .Include(t => t.科目)
                    .Where(t => t.班級id == id)
                    .ToList();

                var classContext = _context.T課程班級s
                    .Where(t => t.班級id == id)
                    .FirstOrDefault();

                ViewBag.Class = classContext;
                ViewBag.ClassCourse = courseContext;

                return View(TimeContext);
            }catch(Exception ex)
            {
                Console.WriteLine("error :" + ex.Message);
                return View("Errors");
            }
        }

        // GET: course_management/T課程課程/Create
        [HttpGet, ActionName("Create")]
        public IActionResult Create(int? id,string? date)
        {
            ViewBag.CourseData = id;
            ViewBag.TimeData = date;
            return PartialView("_CreatePartial");
        }

        // POST: T課程課程/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("班級科目id,開始時間,結束時間,課堂摘要,值日生id,狀態,日期")] CourseTimeDto model)
        {
            try
            {
                DateTime date = DateTime.ParseExact(model.日期, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                DateTime combinedStartTime = date.Add(model.開始時間.TimeOfDay);
                DateTime combinedEndTime = date.Add(model.結束時間.TimeOfDay);

                if (combinedStartTime == null || combinedEndTime == null)
                {
                    ModelState.AddModelError(string.Empty, "開始時間和結束時間必填。");
                    return BadRequest(ModelState);
                }

                bool isOverlap = _context.T課程課程s.Any(c =>
                     c.班級科目id == model.班級科目id &&
                     ((c.開始時間 < combinedEndTime && combinedStartTime < c.結束時間) ||
                     (combinedStartTime <= c.開始時間 && c.開始時間 < combinedEndTime)));


                if (isOverlap)
                {
                    ModelState.AddModelError(string.Empty, "新的課程和舊有的資料有重疊請重新。");
                    return BadRequest(ModelState);
                }


                var 班級 = _context.T課程班級s
                .Include(t => t.T課程班級科目s)
                .FirstOrDefault(t => t.T課程班級科目s.Any(s => s.班級科目id == model.班級科目id));

                if (combinedStartTime < 班級.入學日期 || combinedEndTime > 班級.結訓日期)
                {
                    ModelState.AddModelError(string.Empty, "新的課程時間不在開班時間內。");
                    return BadRequest(ModelState);
                }
                var t課程課程 = new T課程課程
                {
                    班級科目id = model.班級科目id,
                    開始時間 = combinedStartTime,
                    結束時間 = combinedEndTime,
                    課堂摘要 = model.課堂摘要,
                    值日生id = model.值日生id,
                    狀態 = model.狀態,
                };

                _context.Add(t課程課程);
                await _context.SaveChangesAsync();


                var id = _context.T課程班級科目s
                    .Where(t => t.班級科目id == model.班級科目id)
                    .Select(t => t.班級id)
                    .FirstOrDefault();

                return RedirectToAction(nameof(classCourseTimeIndex), new { id });
            }catch(Exception ex)
            {
                Console.WriteLine("error :" + ex.Message);
                return View("Errors");
            }

        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程課程s == null)
            {
                return NotFound();
            }

            var t課程課程 = await _context.T課程課程s           
                .FirstOrDefaultAsync(m => m.課程id == id);
            if (t課程課程 == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", t課程課程);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程課程s == null)
            {
                return Problem("Entity set 'studentContext.T課程課程s'  is null.");
            }
            var t課程課程 = await _context.T課程課程s.FindAsync(id);
            if (t課程課程 != null)
            {
                _context.T課程課程s.Remove(t課程課程);
                await _context.SaveChangesAsync();
            }

            //這裡是classid
            id = _context.T課程班級科目s
             .Where(t => t.班級科目id == t課程課程.班級科目id)
             .Select(t => t.班級id)
             .FirstOrDefault();

            return RedirectToAction(nameof(classCourseTimeIndex), new { id });
        }


        // GET: course_management/T課程課程/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T課程課程s == null)
            {
                return NotFound();
            }

            var t課程課程 = await _context.T課程課程s.FindAsync(id);
            if (t課程課程 == null)
            {
                return NotFound();
            }
            ViewData["值日生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱", t課程課程.值日生id);
            ViewData["班級科目id"] = new SelectList(_context.T課程班級科目s, "班級科目id", "班級科目id", t課程課程.班級科目id);
            return View(t課程課程);
        }

        // POST: course_management/T課程課程/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("課程id,班級科目id,開始時間,結束時間,課堂摘要,值日生id,狀態")] T課程課程 t課程課程)
        {
            if (id != t課程課程.課程id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t課程課程);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程課程Exists(t課程課程.課程id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["值日生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱", t課程課程.值日生id);
            ViewData["班級科目id"] = new SelectList(_context.T課程班級科目s, "班級科目id", "班級科目id", t課程課程.班級科目id);
            return View(t課程課程);
        }


        private bool T課程課程Exists(int id)
        {
          return (_context.T課程課程s?.Any(e => e.課程id == id)).GetValueOrDefault();
        }
    }
}
