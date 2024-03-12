using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程科目;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程科目分類;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程科目分類Controller : Controller
    {
        private readonly studentContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public T課程科目分類Controller(studentContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }

        // GET: course_management/T課程科目分類
        public  IActionResult Index()
        {
            return View();

        }

        public async Task<JsonResult> IndexJson()
        {

            var viewModel = await _context.T課程科目分類s
           .Where(t => t.狀態 == 1)
           .ToListAsync();

            return Json(viewModel);      

        }

        //course_management/T課程科目分類/Details
        [HttpGet, ActionName("Details")]
        public async Task<IActionResult> DetailsPartialView(int? id)
        {
            if (id == null || _context.T課程科目分類s == null)
            {
                return NotFound();
            }

            var t課程科目分類 = await _context.T課程科目分類s
                .FirstOrDefaultAsync(m => m.科目類別id == id);
            if (t課程科目分類 == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", t課程科目分類);

        }

        //course_management/T課程科目分類/Create
        [HttpGet, ActionName("Create")]
        public IActionResult CreatePartialView()
        {
       
            return PartialView("_CreatePartial");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryPictureViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

           
            string fileName = "默認圖片.jpg";
            if (viewmodel.科目類別封面 != null && viewmodel.科目類別封面.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "t課程科目類別");
                var extension = Path.GetExtension(viewmodel.科目類別封面.FileName);
                fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewmodel.科目類別封面.CopyToAsync(stream);
                }
            }

            var t課程科目分類 = new T課程科目分類
            {
                科目類別名稱 = viewmodel.科目類別名稱,
                科目類別封面 = fileName,
                狀態 = 1
            };

            _context.Add(t課程科目分類);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        //course_management/T課程科目分類/Edit
        [HttpGet, ActionName("Edit")]

        public async Task<IActionResult> EditPartialView(int? id)
        {
            if (id == null || _context.T課程科目分類s == null)
            {
                return NotFound();
            }

            var t課程科目分類 = await _context.T課程科目分類s.FindAsync(id);
            if (t課程科目分類 == null)
            {
                return NotFound();
            }

            var ViewModel  = new CategoryPictureViewModel
            {
                科目類別id = t課程科目分類.科目類別id,
                科目類別名稱 = t課程科目分類.科目類別名稱,
                科目類別封面名稱 = t課程科目分類.科目類別封面,               
                狀態 = t課程科目分類.狀態,
            };

            return PartialView("_EditPartial", ViewModel);

        }

        // POST: course_management/T課程科目分類/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryPictureViewModel viewModel)
        {
            if (id != viewModel.科目類別id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string fileName = viewModel.科目類別封面名稱 ?? "默認圖片.jpg";

                    if (viewModel.科目類別封面 != null && viewModel.科目類別封面.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "t課程科目類別");
                        var extension = Path.GetExtension(viewModel.科目類別封面.FileName);
                        fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.科目類別封面.CopyToAsync(stream);
                        }
                    }

                    var t課程科目分類 = new T課程科目分類
                    {
                        科目類別id = viewModel.科目類別id,
                        科目類別名稱 = viewModel.科目類別名稱,
                        科目類別封面 = fileName,
                        狀態 = 1
                    };

                    _context.Update(t課程科目分類);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程科目分類Exists(viewModel.科目類別id))
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
            return RedirectToAction(nameof(Index));
        }


        // GET: course_management/T課程科目分類/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程科目分類s == null)
            {
                return NotFound();
            }

            var t課程科目分類 = await _context.T課程科目分類s
                .FirstOrDefaultAsync(m => m.科目類別id == id);
            if (t課程科目分類 == null)
            {
                return NotFound();
            }
           
            return PartialView("_DeletePartial", t課程科目分類);

        }

        // POST: course_management/T課程科目分類/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程科目分類s == null)
            {
                return Problem("Entity set 'studentContext.T課程科目分類s'  is null.");
            }
            var t課程科目分類 = await _context.T課程科目分類s.FindAsync(id);
            if (t課程科目分類 != null)
            {
                _context.T課程科目分類s.Remove(t課程科目分類);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T課程科目分類Exists(int id)
        {
          return (_context.T課程科目分類s?.Any(e => e.科目類別id == id)).GetValueOrDefault();
        }
    }
}
