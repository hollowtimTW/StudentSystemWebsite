using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.video_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("video_management")]
    public class EntitiesController : Controller
    {
        private readonly studentContext _context;

        public EntitiesController(studentContext context)
        {
            _context = context;
        }

        // GET: video_management/Entities
        public async Task<IActionResult> Index()
        {
            var studentContext = _context.T影片Videos.Include(t => t.科目);
            return View(await studentContext.ToListAsync());
        }

        // GET: video_management/Entities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.T影片Videos == null)
            {
                return NotFound();
            }

            var t影片Video = await _context.T影片Videos
                .Include(t => t.科目)
                .FirstOrDefaultAsync(m => m.FVideoId == id);
            if (t影片Video == null)
            {
                return NotFound();
            }

            return View(t影片Video);
        }

        // GET: video_management/Entities/Create
        public IActionResult Create()
        {
            ViewData["科目id"] = new SelectList(_context.T課程科目s, "科目id", "科目名稱");
            return View();
        }

        // POST: video_management/Entities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FVideoId,FVideoTitle,FUrl,科目id,FPrice,FImagePath")] T影片Video t影片Video)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t影片Video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["科目id"] = new SelectList(_context.T課程科目s, "科目id", "科目名稱", t影片Video.科目id);
            return View(t影片Video);
        }

        // GET: video_management/Entities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T影片Videos == null)
            {
                return NotFound();
            }

            var t影片Video = await _context.T影片Videos.FindAsync(id);
            if (t影片Video == null)
            {
                return NotFound();
            }
            ViewData["科目id"] = new SelectList(_context.T課程科目s, "科目id", "科目名稱", t影片Video.科目id);
            return View(t影片Video);
        }

        // POST: video_management/Entities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FVideoId,FVideoTitle,FUrl,科目id,FPrice,FImagePath")] T影片Video t影片Video)
        {
            if (id != t影片Video.FVideoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t影片Video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T影片VideoExists(t影片Video.FVideoId))
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
            ViewData["科目id"] = new SelectList(_context.T課程科目s, "科目id", "科目名稱", t影片Video.科目id);
            return View(t影片Video);
        }

        // GET: video_management/Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T影片Videos == null)
            {
                return NotFound();
            }

            var t影片Video = await _context.T影片Videos
                .Include(t => t.科目)
                .FirstOrDefaultAsync(m => m.FVideoId == id);
            if (t影片Video == null)
            {
                return NotFound();
            }

            return View(t影片Video);
        }

        // POST: video_management/Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T影片Videos == null)
            {
                return Problem("Entity set 'studentContext.T影片Videos'  is null.");
            }
            var t影片Video = await _context.T影片Videos.FindAsync(id);
            if (t影片Video != null)
            {
                _context.T影片Videos.Remove(t影片Video);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T影片VideoExists(int id)
        {
          return (_context.T影片Videos?.Any(e => e.FVideoId == id)).GetValueOrDefault();
        }
    }
}
