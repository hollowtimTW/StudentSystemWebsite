using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級科目評分;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程評分主表Controller : Controller
    {
        private readonly studentContext _context;

        public T課程評分主表Controller(studentContext context)
        {
            _context = context;
        }

        [HttpGet("/course_management/T課程評分主表/{ClassId}/{ClassCourseId}")]
        public async Task<IActionResult> Index(int? ClassId, int? ClassCourseId)
        {
            if (ClassId == null || ClassCourseId == null)
            {
                return NotFound();
            }

            var studentsInClass = await _context.T會員學生s
            .Where(s => _context.T課程學生班級s.Any(tc => tc.學生id == s.學生id && tc.班級id == ClassId))
            .ToListAsync();
            ViewData["StudentsInClass"] = studentsInClass;
            ViewData["ClassCourseId"] = ClassCourseId;
            ViewData["ClassId"] = ClassId;
            return View();
        }

        [HttpGet("/course_management/T課程評分主表/CheckClassSubjectSatus/{id}")]
        public async Task<bool> CheckClassSubjectSatus(int? id)
        {
            var t課程班級科目= await _context.T課程班級科目s.FindAsync(id);

            if (t課程班級科目.狀態==7)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

        [HttpPost("/course_management/T課程評分主表/CheckRateFormSatus/{ClassCourseId}/{StudentId}")]
        public async Task<IActionResult> CheckRateFormSatus(int ClassCourseId, int StudentId)
        {

            var data = _context.T課程評分主表s.FirstOrDefault(tc => tc.學生id == StudentId && tc.班級科目id == ClassCourseId);

            if (data.狀態==2)
            {
                var evaluationTableID = data.評分主表id;
                return Ok(new { success = true, evaluationTableID });


            }
            else
            {
                return Ok(new { success = false });
            }

        }


        [HttpPost("/course_management/T課程評分主表/Create/{ClassCourseId}/{ClassId}")]
        public async Task<IActionResult> Create(int ClassCourseId, int ClassId)
        {
            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(ClassCourseId);

            t課程班級科目.狀態 = 7;

            var studentIds =
                    await _context.T課程學生班級s
                   .Where(tc => tc.班級id == ClassId)
                   .Select(tc => tc.學生id)
                   .ToListAsync();

            foreach (var StudentId in studentIds)
            {
                // 意見主表記得要更改資料表結構
                var t課程評分主表 = new T課程評分主表()
                {
                    班級科目id = ClassCourseId,
                    學生id = StudentId,
                    提交時間 = DateTime.Now,
                    狀態 = 1,
                    改進意見 = "",
                };
                _context.Add(t課程評分主表);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { classid = ClassId, ClassCourseId = ClassCourseId });


        }


        [HttpGet("/course_management/T課程評分主表/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.T課程評分主表s == null)
            {
                return NotFound();
            }

            var t課程評分表 = await _context.T課程評分主表s
            .Include(m => m.T課程評分s) 
            .FirstOrDefaultAsync(m => m.評分主表id == id);

            if (t課程評分表 == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", t課程評分表);
        }


        // GET: course_management/T課程評分主表/Edit/5
        [HttpGet("/course_management/T課程評分主表/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T課程評分主表s == null)
            {
                return NotFound();
            }

            var  t課程評分表ViewModel = await _context.T課程評分主表s
            .Where(m => m.評分主表id == id)
            .Select(m => new RateFormViewModel
            {
                評分主表id = m.評分主表id,
                班級科目id = m.班級科目id,
                學生id = m.學生id,
                提交時間 = m.提交時間,
                狀態 = m.狀態,
                改進意見 = m.改進意見,
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

        
        [HttpPost("/course_management/T課程評分主表/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("評分主表id, 班級科目id, 學生id, 狀態, 改進意見, 評分s")] RateFormViewModel ViewModel)
        {
            if (id != ViewModel.評分主表id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var transaction = _context.Database.BeginTransaction()) { 

                        var t課程評分主表 = await _context.T課程評分主表s.FindAsync(id);
                    if (t課程評分主表 == null)
                    {
                        return NotFound();
                    }

                    t課程評分主表.班級科目id = ViewModel.班級科目id;
                    t課程評分主表.學生id = ViewModel.學生id;
                    t課程評分主表.提交時間 = DateTime.Now;
                    t課程評分主表.狀態 = ViewModel.狀態;
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


            return RedirectToAction(nameof(Index), new { classid = t課程班級科目.班級id, ClassCourseId = ViewModel.班級科目id });

        }

        [HttpPost("/course_management/T課程評分主表/CalculateAverageRatings/{id}")]
        public async Task<IActionResult> CalculateAverageRatings(int? id)
        {

            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(id);

            if (t課程班級科目.狀態 == 7)
            {
                var ratingsForClass = _context.T課程評分主表s
                .Where(item => item.班級科目id == id)
                .SelectMany(item => item.T課程評分s)
                .ToList();

                var peopleDone = _context.T課程評分主表s
               .Where(item => item.班級科目id == id && item.狀態==2)
               .Count();


                var totalRatingsByCategory = ratingsForClass.GroupBy(item=> item.評分分類)
                    .ToDictionary(group => group.Key, group => group.Average(item => item.評分));

                return Ok(new { success = true , totalRatingsByCategory, peopleDone });

            }
            else
            {
                return Ok(new { success = false });

            }
        }

        private bool T課程評分主表Exists(int id)
        {
          return (_context.T課程評分主表s?.Any(e => e.評分主表id == id)).GetValueOrDefault();
        }










        // GET: course_management/T課程評分主表/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程評分主表s == null)
            {
                return NotFound();
            }

            var t課程評分主表 = await _context.T課程評分主表s
                .Include(t => t.學生)
                .Include(t => t.班級科目)
                .FirstOrDefaultAsync(m => m.評分主表id == id);
            if (t課程評分主表 == null)
            {
                return NotFound();
            }

            return View(t課程評分主表);
        }

        // POST: course_management/T課程評分主表/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程評分主表s == null)
            {
                return Problem("Entity set 'studentContext.T課程評分主表s'  is null.");
            }
            var t課程評分主表 = await _context.T課程評分主表s.FindAsync(id);
            if (t課程評分主表 != null)
            {
                _context.T課程評分主表s.Remove(t課程評分主表);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       
    }
}
