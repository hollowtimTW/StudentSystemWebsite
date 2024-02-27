using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Class_system_Backstage_pj.Areas.course_management.ViewModel.T課程班級;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T班級創建Controller : Controller
    {
        private readonly studentContext _context;

        public T班級創建Controller(studentContext context)
        {
            _context = context;
        }

       

        //GET: course_management/T課程班級/Create
        public IActionResult Index()
        {
            ViewData["班級導師id"] = new SelectList(_context.T會員老師s, "老師id", "姓名");          
            ViewBag.course = courseIndex();
            return View();
        }

        // POST: course_management/T課程班級/Create
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var jsonData = await reader.ReadToEndAsync();

                // 將 JSON 數據轉換為 classCreateViewModel 對象
                var data = JsonConvert.DeserializeObject<classCreateViewModel>(jsonData);

                if(data == null)
                {
                    return NotFound();
                }
                // 將數據存入資料庫
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var t課程班級 = new T課程班級
                        {
                            班級名稱 = data.班級名稱,
                            班級導師id = data.班級導師id,
                            入學日期 = data.入學日期,
                            結訓日期 = data.結訓日期,
                            狀態 = 1
                        };
                        _context.Add(t課程班級);

                        await _context.SaveChangesAsync(); 

                        var 班級id = t課程班級.班級id;
                        if (data.班級科目 != null && 班級id != 0)
                        {
                            foreach (var subjectTeacherPair in data.班級科目)
                            {
                                var 科目id = subjectTeacherPair.CourseId;
                                var 老師id = subjectTeacherPair.TeacherId;

                                // 將科目名稱和成績存入資料庫
                                var t班級科目 = new T課程班級科目
                                    {
                                        班級id = 班級id,
                                        科目id = Convert.ToInt32(科目id),
                                        老師id = 老師id,
                                        狀態=1
                                    };
                                    _context.Add(t班級科目);
                                
                            }

                            await _context.SaveChangesAsync(); 
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(); // 回滾事務
                        Console.WriteLine(ex);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error: " + ex.Message); // 處理全局異常
            }
        }


        //GET: course_management/T課程科目
        public SelectList courseIndex()
        {
            SelectList coursesSelectList = new SelectList(_context.T課程科目s.Select(c => new
            {
                coursesid = c.科目id,
                coursesName = c.科目名稱
            }), "coursesid", "coursesName");


            return coursesSelectList;

        }

        public async Task<IActionResult> GetTeachersBySubjectId(int? id)
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
    }
}
