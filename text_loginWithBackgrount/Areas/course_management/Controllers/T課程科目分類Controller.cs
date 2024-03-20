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

        //course_management/T課程科目分類/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能。
        // </summary>
        // <returns>成功進入該頁面後，返回空的沒有模型的頁面，讓頁面上的datatable去render</returns>
        public IActionResult Index()
        {
            return View();

        }

        //course_management/T課程科目分類/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能頁面的datatable數據來源。
        // </summary>
        // <returns>成功，返回josn的數據</returns>
        public async Task<JsonResult> IndexJson()
        {
            try
            {
                var viewModel = await _context.T課程科目分類s
               .ToListAsync();

                return Json(viewModel);
            } catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                //前端會處理error的屬性
                return Json(new { error = "Failed to retrieve data. Please try again later." });
            }

        }

        //course_management/T課程科目分類/Details
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能頁面的detail。
        // </summary>
        // <param name="id">科目類別id。</param>
        // <returns>成功，返回"_DetailsPartial" partial</returns>
        [HttpGet, ActionName("Details")]
        public async Task<IActionResult> DetailsPartialView(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目分類s == null)
                {
                    return View("Errors");
                }

                var t課程科目分類 = await _context.T課程科目分類s
                    .FirstOrDefaultAsync(m => m.科目類別id == id);

                if (t課程科目分類 == null)
                {
                    return View("Errors");
                }

                return PartialView("_DetailsPartial", t課程科目分類);
            } catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");

            }
        }


        //course_management/T課程科目分類/Create
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能頁面的create。
        // </summary>
        // <returns>成功，返回"_CreatePartial" partial</returns>
        [HttpGet, ActionName("Create")]
        public IActionResult CreatePartialView()
        {
       
            return PartialView("_CreatePartial");

        }

        //course_management/T課程科目分類/Create
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目分類子功能頁面的create的表單資料。
        // </summary>
        // <param name="viewmodel">CategoryPictureViewModel類別 物件。</param>
        // <returns>成功，回到index </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryPictureViewModel viewmodel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Errors");
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

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }



        //course_management/T課程科目分類/Edit
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能頁面的edit功能的畫面。
        // </summary>
        // <param name="id">課程科目分類id。</param>
        // <returns>成功，返回 "_EditPartial" partial</returns>
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditPartialView(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目分類s == null)
                {
                    return View("Errors");
                }

                var t課程科目分類 = await _context.T課程科目分類s.FindAsync(id);
                if (t課程科目分類 == null)
                {
                    return View("Errors");
                }

                var ViewModel = new CategoryPictureViewModel
                {
                    科目類別id = t課程科目分類.科目類別id,
                    科目類別名稱 = t課程科目分類.科目類別名稱,
                    科目類別封面名稱 = t課程科目分類.科目類別封面,
                    狀態 = t課程科目分類.狀態,
                };
                return PartialView("_EditPartial", ViewModel);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }

        }

        // POST: course_management/T課程科目分類/Edit
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目分類子功能頁面的表單資料。
        // </summary>
        // <param name="viewModel">CategoryPictureViewModel類別的 model。</param>
        // <param name="id">課程科目分類id。</param>

        // <returns>成功，返回 "_EditPartial" partial</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryPictureViewModel viewModel)
        {
            if (id != viewModel.科目類別id || !T課程科目分類Exists(viewModel.科目類別id))
            {
                return View("Errors");
            }

            if (!ModelState.IsValid)
            {
                return View("Errors");
            }

            try
            {

                //先預設有值才能確認每一個資料都有圖，如果送來有圖用科目類別封面名稱，沒有就默認圖片
                string fileName = viewModel.科目類別封面名稱 ?? "默認圖片.jpg";

                if (viewModel.科目類別封面 != null && viewModel.科目類別封面.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "t課程科目類別");
                    var extension = Path.GetExtension(viewModel.科目類別封面.FileName);
                    //給一個新的fileName，代表真正圖片的名稱
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
            catch (Exception ex)
            {
                return View("Errors");

            }
            return RedirectToAction(nameof(Index));
        }


        // GET: course_management/T課程科目分類/Delete
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目分類子功能頁面的delete的功能。
        // </summary>
        // <param name="id">科目類別id。</param>
        // <returns>成功，返回 "_DeletePartial" partial</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目分類s == null)
                {
                    return View("Errors");

                }

                var t課程科目分類 = await _context.T課程科目分類s
                    .FirstOrDefaultAsync(m => m.科目類別id == id);

                if (t課程科目分類 == null)
                {
                    return View("Errors");
                }

                return PartialView("_DeletePartial", t課程科目分類);

            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }

        }

        // POST: course_management/T課程科目分類/Delete
        // <summary>
        // 這個方法用於處理科目管理系統中的課程科目分類子功能頁面的delete的指定要刪除的資料實體。
        // </summary>
        // <param name="id">科目類別id。</param>
        // <returns>成功，返回 "Index"</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
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

            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        private bool T課程科目分類Exists(int id)
        {
          return (_context.T課程科目分類s?.Any(e => e.科目類別id == id)).GetValueOrDefault();
        }
    }
}
