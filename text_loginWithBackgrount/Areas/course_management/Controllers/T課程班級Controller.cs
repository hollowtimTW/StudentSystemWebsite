using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程班級Controller : Controller
    {
        private readonly studentContext _context;

        public T課程班級Controller(studentContext context)
        {
            _context = context;
        }

        // GET: course_management/T課程班級
        public async Task<IActionResult> Index()
        {
            var studentContext = await _context.T課程班級s
         .Include(t => t.班級導師)
         .Where(t => t.狀態 == 1)
         .Select(t => new ClassIndexViewModel
         {
             班級id = t.班級id,
             班級名稱 = t.班級名稱,
             入學日期 = t.入學日期,
             結訓日期 = t.結訓日期,
             班級導師姓名 = t.班級導師.姓名
         })
         .ToListAsync();

            return View(studentContext);
        }

        public async Task<IActionResult> Indexjson()
        {
                var T課程班級 = await _context.T課程班級s
                .Where(t => t.狀態 == 1)
                .Join(
                    _context.T會員老師s,
                    班級 => 班級.班級導師id,
                    導師 => 導師.老師id,
                    (班級, 導師) => new{
                        班級 = 班級,
                        導師 = 導師
                    })
                .Select(joinResult => new ClassIndexViewModel
                {
                    班級id=joinResult.班級.班級id,
                    班級名稱 = joinResult.班級.班級名稱,
                    入學日期 = joinResult.班級.入學日期,
                    結訓日期 = joinResult.班級.結訓日期,
                    班級導師姓名 = joinResult.導師.姓名,
                })
                .ToListAsync();

            return Json(T課程班級);


        }


        // GET: course_management/T課程班級/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.T課程班級s == null)
            {
                return NotFound();
            }

            var t課程班級 = await _context.T課程班級s
                .Include(t => t.班級導師)
                .FirstOrDefaultAsync(m => m.班級id == id);
            if (t課程班級 == null)
            {
                return NotFound();
            }

            return View(t課程班級);
        }

        public async Task<bool> checkClassCourse(int? id)
        {
            var number = _context.T課程班級科目s
            .Where(t => t.班級id==id)
            .Count();
            if (number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 

        // GET: course_management/T課程班級/Edit/5
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T課程班級s == null)
            {
                return NotFound();
            }

            var t課程班級 = await _context.T課程班級s.FindAsync(id);
            if (t課程班級 == null)
            {
                return NotFound();
            }
            ViewData["班級導師id"] = new SelectList(_context.T會員老師s, "老師id", "姓名", t課程班級.班級導師id);
            return PartialView("_EditPartial", t課程班級);
        }

        // POST: course_management/T課程班級/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("班級id,班級名稱,班級導師id,入學日期,結訓日期,狀態")] T課程班級 t課程班級)
        {
            if (id != t課程班級.班級id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t課程班級);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程班級Exists(t課程班級.班級id))
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
            ViewData["班級導師id"] = new SelectList(_context.T會員老師s, "老師id", "信箱", t課程班級.班級導師id);
            return View(t課程班級);
        }

        // GET: course_management/T課程班級/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程班級s == null)
            {
                return NotFound();
            }

            var t課程班級 = await _context.T課程班級s
                .Include(t => t.班級導師)
                .FirstOrDefaultAsync(m => m.班級id == id);
            if (t課程班級 == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", t課程班級);
        }

        // POST: course_management/T課程班級/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程班級s == null)
            {
                return Problem("Entity set 'studentContext.T課程班級s'  is null.");
            }
            var t課程班級 = await _context.T課程班級s.FindAsync(id);
            if (t課程班級 != null)
            {
                _context.T課程班級s.Remove(t課程班級);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T課程班級Exists(int id)
        {
            return (_context.T課程班級s?.Any(e => e.班級id == id)).GetValueOrDefault();
        }
    }
}
