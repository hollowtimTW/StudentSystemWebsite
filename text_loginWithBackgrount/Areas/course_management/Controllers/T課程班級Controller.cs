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

        //course_management/T課程班級/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的班級管理子功能。
        // </summary>
        // <returns>成功進入該頁面後，返回空的沒有模型的頁面，讓頁面上的datatable去render</returns>
        public async Task<IActionResult> Index()
        {
        
          return View();
        }

        //course_management/T課程科目/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的班級管理子功能頁面的datatable數據來源。
        // </summary>
        // <returns>成功，返回josn的數據</returns>
        public async Task<IActionResult> Indexjson()
        {
            try
            {
                var T課程班級 = await _context.T課程班級s
                .Where(t => t.狀態 == 1)
                .Join(
                    _context.T會員老師s,
                    班級 => 班級.班級導師id,
                    導師 => 導師.老師id,
                    (班級, 導師) => new
                    {
                        班級 = 班級,
                        導師 = 導師
                    })
                .Select(joinResult => new ClassIndexViewModel
                {
                    班級id = joinResult.班級.班級id,
                    班級名稱 = joinResult.班級.班級名稱,
                    入學日期 = joinResult.班級.入學日期,
                    結訓日期 = joinResult.班級.結訓日期,
                    班級導師姓名 = joinResult.導師.姓名,
                })
                .ToListAsync();

                return Json(T課程班級);
            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                //前端會處理error的屬性
                return Json(new { error = "Failed to retrieve data. Please try again later." });
            }

        }


        //course_management/T課程班級/Edit
        // <summary>
        // 這個方法用於呈現科目管理系統中的班級管理子功能頁面的edit。
        // </summary>
        // <param name="id">課程班級id。</param>
        // <returns>成功，返回_EditPartial的 partialView</returns>
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程班級s == null)
                {
                    return View("Errors");
                }

                var t課程班級 = await _context.T課程班級s.FindAsync(id);
                if (t課程班級 == null)
                {
                    return View("Errors");
                }
                //如果null前端要處理
                ViewData["班級導師id"] = new SelectList(_context.T會員老師s, "老師id", "姓名", t課程班級.班級導師id);
                return PartialView("_EditPartial", t課程班級);

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        //course_management/T課程班級/Edit
        // <summary>
        // 這個方法用於接收科目管理系統中的班級管理子功能頁面的edit回傳的資料 透過model去接收。
        // </summary>
        // <param name="t課程科目">T課程科目類別的model。</param>
        // <param name="id">班級id。</param>
        // <returns>成功，返回index，並更新課程科目表的對應資料</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("班級id,班級名稱,班級導師id,入學日期,結訓日期,狀態")] T課程班級 t課程班級)
        {
            try
            {
                if (id != t課程班級.班級id || !T課程班級Exists(t課程班級.班級id))
                {
                    return View("Errors");
                }

                if (!ModelState.IsValid){
                    return View("Errors");
                }
                _context.Update(t課程班級);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch  (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        // GET: course_management/T課程班級/Delete
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程班級子功能頁面的delete的功能。
        // </summary>
        // <param name="id">班級id。</param>
        // <returns>成功，返回 "_DeletePartial" partial</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程班級s == null)
                {
                    return View("Errors");

                }

                var t課程班級 = await _context.T課程班級s
                    .Include(t => t.班級導師)
                    .FirstOrDefaultAsync(m => m.班級id == id);

                if (t課程班級 == null)
                {
                    return View("Errors");
                }

                return PartialView("_DeletePartial", t課程班級);

            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        // POST: course_management/T課程班級/Delete
        // <summary>
        // 這個方法用於處理科目管理系統中的課程班級功能頁面的delete的指定要刪除的資料實體。
        // </summary>
        // <param name="id">班級id。</param>
        // <returns>成功，返回 "Index"</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.T課程班級s == null)
                {
                    return Problem("Entity set 'studentContext.T課程班級s'  is null.");
                }
                var t課程班級 = await _context.T課程班級s.FindAsync(id);

                if (t課程班級 != null)
                {
                    //先刪班級科目表
                    _context.T課程班級s.Remove(t課程班級);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        // POST: course_management/T課程班級/Details
        // <summary>
        // 這個方法用於處理科目管理系統中的課程班級管理功能頁面的Details頁。
        // </summary>
        // <param name="id">班級id。</param>
        // <returns>成功，返回 "Details"</returns>
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程班級s == null)
                {
                    return View("Error");
                }

                var t課程班級 = await _context.T課程班級s
                    .Include(t => t.班級導師)
                    .FirstOrDefaultAsync(m => m.班級id == id);
                if (t課程班級 == null)
                {
                    return View("Error");
                }

                return View(t課程班級);
            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Error");

            }
        }


        // POST: course_management/T課程班級/checkClassCourse
        // <summary>
        // 這個方法用於確認傳入的班級科目id的資料實體是否再班級科目表中有相關的資料。
        // </summary>
        // <param name="id">班級id。</param>
        // <returns>有相關資料回傳true，沒有會有問題就是回傳false</returns>
        public async Task<bool> checkClassCourse(int? id)
        {
            try
            {
                var number = _context.T課程班級科目s
                .Where(t => t.班級id == id)
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
            catch(Exception ex) { 
            
                    Console.WriteLine($"發生錯誤: {ex.Message}");
                    return false;
            }
        } 

       
       

       

        private bool T課程班級Exists(int id)
        {
            return (_context.T課程班級s?.Any(e => e.班級id == id)).GetValueOrDefault();
        }
    }
}
