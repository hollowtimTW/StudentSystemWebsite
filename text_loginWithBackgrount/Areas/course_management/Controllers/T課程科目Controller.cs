using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程科目;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程科目Controller : Controller
    {
        private readonly studentContext _context;

        public T課程科目Controller(studentContext context)
        {
            _context = context;
        }

        //course_management/T課程科目
        public  IActionResult Index()
        {
            return View();
        }
        
        public async Task<JsonResult> IndexJson()
        {

            var viewModel = await _context.T課程科目s
           .Where(t => t.狀態 == 1)
           .Select(t => new courseIndexViewModel
           {
               科目id = t.科目id,
               科目名稱 = t.科目名稱,
               科目類別id = t.科目類別id,
               科目類別名稱 = t.科目類別.科目類別名稱,
               狀態 = t.狀態
           }).ToListAsync();

            return Json(viewModel);


        }

        //course_management/T課程科目/Details
        [HttpGet, ActionName("Details")]
        public async Task<IActionResult> DetailsPartialView(int? id)
        {
            if (id == null || _context.T課程科目s == null)
            {
                return NotFound();
            }

            var t課程科目 = await _context.T課程科目s
                .Include(t => t.科目類別)
                .FirstOrDefaultAsync(m => m.科目id == id);
            if (t課程科目 == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial",t課程科目);
        }

        //course_management/T課程科目/Create
        [HttpGet, ActionName("Create")]
        public IActionResult CreatePartialView()
        {
            var categories = _context.T課程科目分類s.ToList();
            SelectList categorySelectList = new SelectList(_context.T課程科目分類s
             .Where(t => t.狀態 == 1)
             .Select(c=>new
            {
                Categoryid=c.科目類別id,
                CategoryName = c.科目類別名稱
            }), "Categoryid", "CategoryName");

            ViewBag.Category = categorySelectList;

            return PartialView("_CreatePartial");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("科目名稱,科目類別id,狀態")] T課程科目 t課程科目)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t課程科目);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
               
                
            }
            return View(t課程科目);

        }


        //course_management/T課程科目/Edit/5
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditPartialView(int? id)
        {
            if (id == null || _context.T課程科目s == null)
            {
                return NotFound();
            }

            var t課程科目 = await _context.T課程科目s.FindAsync(id);
            if (t課程科目 == null)
            {
                return NotFound();
            }

            var categories = _context.T課程科目分類s.ToList();
            SelectList categorySelectList = new SelectList(_context.T課程科目分類s
                 .Where(t => t.狀態 == 1)
                .Select(c => new
            {
                Categoryid = c.科目類別id,
                CategoryName = c.科目類別名稱
            }), "Categoryid", "CategoryName");

            ViewBag.Category = categorySelectList;

            return PartialView("_EditPartial", t課程科目);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("科目id,科目名稱,科目類別id,狀態")] T課程科目 t課程科目)
        {
            if (id != t課程科目.科目id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t課程科目);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程科目Exists(t課程科目.科目id))
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

            //如果沒成功
            var categories = _context.T課程科目分類s.ToList();
            SelectList categorySelectList = new SelectList(_context.T課程科目分類s.Select(c => new
            {
                Categoryid = c.科目類別id,
                CategoryName = c.科目類別名稱
            }), "Categoryid", "CategoryName");

            ViewBag.Category = categorySelectList; 
            return View(t課程科目);
        }

        // GET: course_management/T課程科目/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程科目s == null)
            {
                return NotFound();
            }

            var t課程科目 = await _context.T課程科目s
                .Include(t => t.科目類別)
                .FirstOrDefaultAsync(m => m.科目id == id);
            if (t課程科目 == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", t課程科目);

        }

        // POST: course_management/T課程科目/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程科目s == null)
            {
                return Problem("Entity set 'studentContext.T課程科目s'  is null.");
            }
            var t課程科目 = await _context.T課程科目s.FindAsync(id);
            if (t課程科目 != null)
            {
                _context.T課程科目s.Remove(t課程科目);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, ActionName("CourseTeacher")]
        public  IActionResult IndexCourseteacherView()
        {
            
            return PartialView("_IndexCourseteacherPartial");

        }

        [HttpPost, ActionName("CourseTeacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseTeacher([Bind("老師id,科目id,狀態")] T課程老師科目 t課程老師科目)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t課程老師科目);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<JsonResult> allTeacherJson()
        {
             var teachers = await _context.T會員老師s.ToListAsync();


            return Json(teachers);

        }

        public async Task<IActionResult> allcourseTeacherJson(int? id)
        {
            if (id == null || _context.T課程科目s == null)
            {
                return NotFound();
            }
            var t課程科目 = await _context.T課程科目s
               .FirstOrDefaultAsync(m => m.科目id == id);

            if (t課程科目 == null)
            {
                return NotFound();
            }
            var Courseteachers = await _context.T課程老師科目s
            .Where(tc => tc.狀態 == 1 && tc.科目id == id)
            .Select(tc => tc.老師)
            .ToListAsync();



            return Json(Courseteachers);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourseTeacherConfirmed(int teacherid, int courseid)
        {
            if (_context.T課程老師科目s == null)
            {
                return Problem("Entity set 'studentContext.T課程科目s'  is null.");
            }

            var t課程老師科目 = await _context.T課程老師科目s
              .FirstOrDefaultAsync(m => m.科目id == courseid & m.老師id== teacherid);

            if (t課程老師科目 != null)
            {
                _context.T課程老師科目s.Remove(t課程老師科目);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T課程科目Exists(int id)
        {
          return (_context.T課程科目s?.Any(e => e.科目id == id)).GetValueOrDefault();
        }
    }
}
