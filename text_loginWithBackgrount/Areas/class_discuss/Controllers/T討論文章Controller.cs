using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
    [Area("class_discuss")]
    public class T討論文章Controller : Controller
    {
        private readonly studentContext _context;

        public T討論文章Controller(studentContext context)
        {
            _context = context;
        }

        // GET: class_discuss/T討論文章
        public async Task<IActionResult> Index()
        {
            var studentContext = _context.T討論文章s.Include(t => t.子版).Include(t => t.學生).Include(t => t.看板);
            return View(await studentContext.ToListAsync());
        }

        // GET: class_discuss/T討論文章/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.T討論文章s == null)
            {
                return NotFound();
            }

            var t討論文章 = await _context.T討論文章s
                .Include(t => t.子版)
                .Include(t => t.學生)
                .Include(t => t.看板)
                .FirstOrDefaultAsync(m => m.文章id == id);
            if (t討論文章 == null)
            {
                return NotFound();
            }

            return View(t討論文章);
        }

        // GET: class_discuss/T討論文章/Create
        public IActionResult Create()
        {
            ViewData["子版id"] = new SelectList(_context.T討論子版s, "子版id", "名稱");
            ViewData["學生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱");
            ViewData["看板id"] = new SelectList(_context.T討論看板s, "看板id", "看板id");
            return View();
        }

        // POST: class_discuss/T討論文章/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("文章id,學生id,內容,時間,觀看數,標題,子版id,看板id")] T討論文章 t討論文章)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t討論文章);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["子版id"] = new SelectList(_context.T討論子版s, "子版id", "子版id", t討論文章.子版id);
            ViewData["學生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱", t討論文章.學生id);
            ViewData["看板id"] = new SelectList(_context.T討論看板s, "看板id", "看板id", t討論文章.看板id);
            return View(t討論文章);
        }

        // GET: class_discuss/T討論文章/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T討論文章s == null)
            {
                return NotFound();
            }

            var t討論文章 = await _context.T討論文章s.FindAsync(id);
            if (t討論文章 == null)
            {
                return NotFound();
            }
            ViewData["子版id"] = new SelectList(_context.T討論子版s, "子版id", "子版id", t討論文章.子版id);
            ViewData["學生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱", t討論文章.學生id);
            ViewData["看板id"] = new SelectList(_context.T討論看板s, "看板id", "看板id", t討論文章.看板id);
            return View(t討論文章);
        }

        // POST: class_discuss/T討論文章/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("文章id,學生id,內容,時間,觀看數,標題,子版id,看板id")] T討論文章 t討論文章)
        {
            if (id != t討論文章.文章id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t討論文章);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T討論文章Exists(t討論文章.文章id))
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
            ViewData["子版id"] = new SelectList(_context.T討論子版s, "子版id", "子版id", t討論文章.子版id);
            ViewData["學生id"] = new SelectList(_context.T會員學生s, "學生id", "信箱", t討論文章.學生id);
            ViewData["看板id"] = new SelectList(_context.T討論看板s, "看板id", "看板id", t討論文章.看板id);
            return View(t討論文章);
        }

        // GET: class_discuss/T討論文章/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T討論文章s == null)
            {
                return NotFound();
            }

            var t討論文章 = await _context.T討論文章s
                .Include(t => t.子版)
                .Include(t => t.學生)
                .Include(t => t.看板)
                .FirstOrDefaultAsync(m => m.文章id == id);
            if (t討論文章 == null)
            {
                return NotFound();
            }

            return View(t討論文章);
        }

        // POST: class_discuss/T討論文章/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T討論文章s == null)
            {
                return Problem("Entity set 'studentContext.T討論文章s'  is null.");
            }
            var t討論文章 = await _context.T討論文章s.FindAsync(id);
            if (t討論文章 != null)
            {
                _context.T討論文章s.Remove(t討論文章);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T討論文章Exists(int id)
        {
          return (_context.T討論文章s?.Any(e => e.文章id == id)).GetValueOrDefault();
        }
    }
}
