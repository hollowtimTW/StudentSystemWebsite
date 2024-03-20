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

        //course_management/T課程科目/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能。
        // </summary>
        // <returns>成功進入該頁面後，返回空的沒有模型的頁面，讓頁面上的datatable去render</returns>
        public IActionResult Index()
        {
            return View();
        }

        //course_management/T課程科目/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的datatable數據來源。
        // </summary>
        // <returns>成功，返回josn的數據</returns>
        public async Task<JsonResult> IndexJson()
        {

            try
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
            catch (Exception ex)
            {
                
                Console.WriteLine($"發生錯誤: {ex.Message}");
                //前端會處理error的屬性
                return Json(new { error = "Failed to retrieve data. Please try again later." });
            }


        }

        //course_management/T課程科目/Create
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的create。
        // </summary>
        // <returns>成功，返回create 的 partialView</returns>
        [HttpGet, ActionName("Create")]
        public IActionResult CreatePartialView()
        {
            try
            {
                var categories = _context.T課程科目分類s.ToList();

                SelectList categorySelectList = 
                new SelectList(
                 _context.T課程科目分類s
                 .Select(c => new
                 {
                     Categoryid = c.科目類別id,
                     CategoryName = c.科目類別名稱
                 })
                 ,"Categoryid", "CategoryName");

                //如果SelectList是null 前端處理呈現不同資料
                ViewBag.Category = categorySelectList;

                return PartialView("_CreatePartial");

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }

        }

        //course_management/T課程科目/Create
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目子功能頁面的create回傳的資料 透過model去接收。
        // </summary>
        // <param name="t課程科目">T課程科目類別的model。</param>
        // <returns>成功，返回create 的 model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("科目名稱,科目類別id,狀態")] T課程科目 t課程科目)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(t課程科目);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }catch (Exception ex)
                {
                    Console.WriteLine($"發生錯誤: {ex.Message}");

                    return View("Errors");

                }

            }
            //如果驗證失敗就返回error
            return View("Errors");

        }

        //course_management/T課程科目/Edit
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的edit。
        // </summary>
        // <param name="id">課程科目id。</param>
        // <returns>成功，返回_EditPartial 的 partialView</returns>
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditPartialView(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目s == null)
                {
                    return View("Errors");

                }

                var t課程科目 = await _context.T課程科目s.FindAsync(id);
                if (t課程科目 == null)
                {
                    return View("Errors");

                }

                var categories = _context.T課程科目分類s.ToList();
                SelectList categorySelectList = new SelectList(_context.T課程科目分類s                 
                    .Select(c => new
                    {
                        Categoryid = c.科目類別id,
                        CategoryName = c.科目類別名稱
                    }), "Categoryid", "CategoryName");

                //如果null前端處理呈現不同資料
                ViewBag.Category = categorySelectList;

                return PartialView("_EditPartial", t課程科目);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        //course_management/T課程科目/Edit
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目子功能頁面的edit回傳的資料 透過model去接收。
        // </summary>
        // <param name="t課程科目">T課程科目類別的model。</param>
        // <param name="id">科目id。</param>
        // <returns>成功，返回index，並更新課程科目表的對應資料</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("科目id,科目名稱,科目類別id,狀態")] T課程科目 t課程科目)
        {
            try
            {

                if (id != t課程科目.科目id || !T課程科目Exists(t課程科目.科目id))
                {
                    return View("Errors");
                 }

                if (!ModelState.IsValid)
                {
                   
                    return View("Errors");

                }
               
                    _context.Update(t課程科目);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));            

            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }

        }

        // GET: course_management/T課程科目/Delete
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的delete。
        // </summary>
        // <param name="id">課程科目id。</param>
        // <returns>成功，返回delete 的 partialView</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目s == null)
                {
                    return View("Errors");
                }

                var t課程科目 = await _context.T課程科目s
                    .Include(t => t.科目類別)
                    .FirstOrDefaultAsync(m => m.科目id == id);

                if (t課程科目 == null)
                {
                    return View("Errors");
                }

                return PartialView("_DeletePartial", t課程科目);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");

            }

        }

        // POST: course_management/T課程科目/Delete
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目子功能頁面的delete回傳的資料 透過model去接收。。
        // </summary>
        // <param name="id">課程科目id。</param>
        // <returns>成功，刪除課程科目表內的對應資料，返回到index頁面</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程科目s == null)
            {
                return Problem("Entity set 'studentContext.T課程科目s'  is null.");
            }

            try
            {
                var t課程科目 = await _context.T課程科目s.FindAsync(id);
                if (t課程科目 != null)
                {
                    _context.T課程科目s.Remove(t課程科目);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
           
        }

        //course_management/T課程科目/Details
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的Details。
        // </summary>
        // <param name="id">課程科目id。</param>
        // <returns>成功，返回Details 的 partialView</returns>
        [HttpGet, ActionName("Details")]
        public async Task<IActionResult> DetailsPartialView(int? id)
        {
            if (id == null ||  id == 0 || _context.T課程科目s == null)
            {
                return View("Errors");
            }

            try
            {

                var t課程科目 = await _context.T課程科目s
                .Include(t => t.科目類別)
                .FirstOrDefaultAsync(m => m.科目id == id);
                if (t課程科目 == null)
                {
                    return View("Errors");
                }

                return PartialView("_DetailsPartial", t課程科目);
            } catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }


        //course_management/T課程科目/CourseTeacher
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的add teacher。
        // </summary>
        // <returns>成功，返回_IndexCourseteacherPartial 的 partialView</returns>
        [HttpGet, ActionName("CourseTeacher")]
        public  IActionResult IndexCourseteacherView()
        {
            
            return PartialView("_IndexCourseteacherPartial");

        }

        //course_management/T課程科目/CourseTeacher
        // <summary>
        // 這個方法用於接收科目管理系統中的課程科目子功能頁面的add teacher的表單。
        // </summary>
        // <param name="t課程老師科目">T課程老師科目類別的model。</param>
        // <returns>成功，存入老師科目表，返回Index</returns>
        [HttpPost, ActionName("CourseTeacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseTeacher([Bind("老師id,科目id,狀態")] T課程老師科目 t課程老師科目)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(t課程老師科目);
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        //course_management/T課程科目/CourseTeacher
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的add teacher裡，關於目前現有老師的資料，以幫助使用者選擇要新增哪個老師。
        // </summary>
        // <returns>成功，撈出所有老師資料，返回josn格式，成為datatable的數據流</returns>
        public async Task<JsonResult> allTeacherJson()
        {
            try
            {
                var teachers = await _context.T會員老師s.ToListAsync();
                //null datatable會處理
                return Json(teachers);

            }catch(Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return Json(new { error = "Failed to retrieve teachers data." });

            }

        }

        //course_management/T課程科目/CourseTeacher
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的add teacher裡，關於目前選定科目中能夠教授該科目的老師資料，以幫助使用者查看現有科目的老師有誰。
        // </summary>
        // <param name="id">課程科目id。</param>
        // <returns>成功，撈出所有能授課老師資料，返回josn格式，成為datatable的數據流</returns>
        public async Task<IActionResult> allcourseTeacherJson(int? id)
        {
            try
            {
                if (id == null || id == 0 || _context.T課程科目s == null)
                {
                    return View("Errors");

                }
                var t課程科目 = await _context.T課程科目s
                   .FirstOrDefaultAsync(m => m.科目id == id);

                if (t課程科目 == null)
                {
                    return View("Errors");

                }
                var Courseteachers = await _context.T課程老師科目s
                .Where(tc => tc.科目id == id)
                .Select(tc => tc.老師)
                .ToListAsync();


                //null datatable會處理
                return Json(Courseteachers);

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return Json(new { error = "Failed to retrieve teachers who can teach this subject." });
            }
            

        }

        //course_management/T課程科目/CourseTeacher
        // <summary>
        // 這個方法用於呈現科目管理系統中的課程科目子功能頁面的add teacher裡，刪除目前選定科目中能夠教授該科目的老師資料。
        // </summary>
        // <param name="teacherid">老師id。</param>
        // <param name="courseid">課程科目id。</param>
        // <returns>成功，刪除老師科目表內，指定科目id的指定老師id都符合的第一個資料，並返回index</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteCourseTeacherConfirmed(int teacherid, int courseid)
        {
            try
            {
                if (_context.T課程老師科目s == null)
                {
                    return Problem("Entity set 'studentContext.T課程科目s'  is null.");
                }

                var t課程老師科目 = await _context.T課程老師科目s
                  .FirstOrDefaultAsync(m => m.科目id == courseid & m.老師id == teacherid);

                if (t課程老師科目 != null)
                {
                    _context.T課程老師科目s.Remove(t課程老師科目);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return Json(new { error = "Failed to retrieve teachers who can teach this subject." });
            }
        }

        private bool T課程科目Exists(int id)
        {
          return (_context.T課程科目s?.Any(e => e.科目id == id)).GetValueOrDefault();
        }
    }
}
