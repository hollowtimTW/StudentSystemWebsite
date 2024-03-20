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



        //course_management/T班級創建/ [read]
        // <summary>
        // 這個方法用於呈現科目管理系統中的班級新增的子功能頁面。
        // </summary>
        // <returns>成功返回該頁面</returns>
        public IActionResult Index()
        {
            try
            {
                //如果為null，前端負責處理
                ViewData["班級導師id"] = TeacherInfo();
                ViewBag.course = courseIndex();
                return View();

            }catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        //course_management/T班級創建/ [api]
        // <summary>
        // 這個方法用於返回科目管理系統中的班級新增的子功能頁面所需要的科目資料select對象。
        // </summary>
        // <returns>成功返回SelectList對象</returns>
        public SelectList courseIndex()
        {
            try
            {
                SelectList coursesSelectList = new SelectList(_context.T課程科目s.Select(c => new
                {
                    coursesid = c.科目id,
                    coursesName = c.科目名稱
                }), "coursesid", "coursesName");


                return coursesSelectList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return null;
            }

        }

        //course_management/T班級創建/ [api]
        // <summary>
        // 這個方法用於返回科目管理系統中的班級新增的子功能頁面所需要的老師資料select對象。
        // </summary>
        // <returns>成功返回SelectList對象</returns>
        public SelectList TeacherInfo()
        {
            try
            {
                SelectList teacherSelectList = new SelectList(_context.T會員老師s, "老師id", "姓名");

                return teacherSelectList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return null;
            }

        }

        //course_management/T班級創建/ [api]
        // <summary>
        // 這個方法用於返回科目管理系統中的班級新增的子功能頁面所指定傳來的科目id能夠授課的老師資料。
        // </summary>
        // <param name="id">T課程科目id。</param>
        // <returns>成功返回SelectList對象</returns>
        public async Task<IActionResult> GetTeachersBySubjectId(int? id)
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
                return Json(Courseteachers);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }

        // POST: course_management/T班級創建/Create
        // <summary>
        // 這個方法用於處理科目管理系統中的班級新增的子功能頁面create所指定傳來的表單資料。
        // </summary>
        // <returns>成功返回SelectList對象</returns>
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
                    return View("Errors");
                }
                // 將數據存入資料庫，因為有多張表要處理 所以使用Transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //step1:先存入班級表
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

                        //step2:將剛剛新增的班級的ID拿來和表單裡的班級科目[data.班級科目]一起再存入班級科目表
                        var 班級id = t課程班級.班級id;

                        // 如果有選擇班級科目，則將其存入班級科目表
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
                        // 提交事務，確認所有的變更
                        await transaction.CommitAsync();
                        return RedirectToAction("Index", "T課程班級", new { area = "course_management" });

                    }
                    catch (Exception ex)
                    {
                        // 發生錯誤時回滾事務
                        await transaction.RollbackAsync(); 
                        Console.WriteLine(ex);
                        return View("Errors");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("Errors");
            }
        }




        
    }
}
