﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Class_system_Backstage_pj.Models;
using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Class_system_Backstage_pj.Areas.job_vacancy.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("job_vacancy")]
    [Route("/job_vacancy/Resume/{Action=Index}/{resumeID?}")]
    public class ResumeController : Controller
    {
        private readonly studentContext _context;

        public ResumeController(studentContext context)
        {
            _context = context;
        }

        // GET: job_vacancy/Resume
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增履歷1：輸入學生身分證號，確認學生存在並提取部分學生資料。
        /// </summary>
        /// <param name="IDnumber">學生的身分證號</param>
        // POST: job_vacancy/Resume/CreateVerify/5
        [HttpPost]
        public async Task<IActionResult> CreateVerify(string IDnumber)
        {
            try
            {
                var theStudent = await _context.T會員學生s.FirstOrDefaultAsync(data => data.身分證字號 == IDnumber);

                if (theStudent != null)
                {
                    var viewModel = new ResumeCreateViewModel
                    {
                        StudentID = theStudent.學生id,
                        Name = theStudent.姓名,
                        ResumeTitle = theStudent.姓名 + "的履歷",
                        Photo = theStudent.圖片,
                        Gender = theStudent.性別,
                        Birth = theStudent.生日,
                        Email = theStudent.信箱,
                        Phone = theStudent.手機,
                        School = theStudent.學校,
                        Department = theStudent.科系,
                        Academic = theStudent.學位,
                        Graduated = theStudent.畢肄
                    };

                    // 將 viewModel 存到 Session 中，用於後續使用
                    var viewModelJson = JsonConvert.SerializeObject(viewModel);
                    var viewModelBytes = Encoding.UTF8.GetBytes(viewModelJson);
                    HttpContext.Session.Set("ResumeViewModel", viewModelBytes);

                    return Json(new { success = true, redirectUrl = Url.Action("Create", "Resume") });
                }
                else
                {
                    // 如果學生不存在，返回查無此學生
                    return Json(new { success = false, message = $"查無此學生 {IDnumber}" });
                }
            }
            catch (Exception ex)
            {
                // 如果發生異常，返回問題詳細訊息
                return Problem(detail: ex.Message + "異常");
            }
        }

        /// <summary>
        /// 新增履歷2：負責返回視圖。
        /// </summary>
        // GET: job_vacancy/Resume/Create
        public IActionResult Create()
        {
            //取出存在Session的資料
            var viewModelBytes = HttpContext.Session.Get("ResumeViewModel");
            if (viewModelBytes != null)
            {
                var viewModelJson = Encoding.UTF8.GetString(viewModelBytes);
                var viewModel = JsonConvert.DeserializeObject<ResumeCreateViewModel>(viewModelJson);
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 新增履歷3：將前端傳回的資料進行驗證，通過後存入資料庫。
        /// </summary>
        /// <param name="viewModel"></param>
        // POST: job_vacancy/Resume/CreateResume
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResume(
            [Bind("StudentID, Name, Birth, Photo, Gender, Phone, Email, School, Academic, Department, Graduated, ResumeTitle, ResumeStatus, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography")] ResumeCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //更新學生資料
                    var theStudent = await _context.T會員學生s.FindAsync(viewModel.StudentID);
                    if (theStudent == null)
                    {
                        return NotFound();
                    }

                    byte[]? photoData = await ReadUploadImage(Request.Form.Files["Photo"], null);

                    theStudent.姓名 = viewModel.Name;
                    theStudent.圖片 = photoData;
                    theStudent.性別 = viewModel.Gender;
                    theStudent.手機 = viewModel.Phone;
                    theStudent.生日 = viewModel.Birth;
                    theStudent.信箱 = viewModel.Email;
                    theStudent.學校 = viewModel.School;
                    theStudent.學位 = viewModel.Academic;
                    theStudent.科系 = viewModel.Department;
                    theStudent.畢肄 = viewModel.Graduated;
                    theStudent.修改日期 = DateTime.Now;
                    _context.Update(theStudent);

                    // 新增履歷資料
                    var newResume = new T工作履歷資料
                    {
                        F學員Id = viewModel.StudentID,
                        F履歷名稱 = viewModel.ResumeTitle,
                        F履歷狀態 = "公開",
                        F專長技能 = viewModel.Skill,
                        F語文能力 = viewModel.Language,
                        F有無工作經驗 = viewModel.WorkExperience,
                        F工作性質 = viewModel.WorkType,
                        F工作時段 = viewModel.WorkTime,
                        F配合輪班 = viewModel.WorkShift,
                        F希望職稱 = viewModel.HopeJobTitle,
                        F希望薪水待遇 = viewModel.HopeSalary,
                        F希望工作地點 = viewModel.HopeLocation,
                        F自傳 = viewModel.Autobiography,
                        F建立時間 = DateTime.Now,
                        F最後更新時間 = DateTime.Now,
                        F刪除狀態 = "0",
                        F刪除或關閉原因 = "0"
                    };

                    // 將新的履歷添加到資料庫中
                    _context.T工作履歷資料s.Add(newResume);
                    await _context.SaveChangesAsync();

                    return Json(new { redirectUrl = Url.Action("Index", "Resume") });

                }
                else
                {
                    string errorMessage = string.Join("；", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }

        /// <summary>
        /// 編輯履歷1：根據履歷編號提取相應的資料，並返回視圖。
        /// </summary>
        /// <param name="resumeID">履歷ID</param>
        // GET: job_vacancy/Resume/Edit/5
        public async Task<IActionResult> Edit(int resumeID)
        {
            var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);
            
            if (thisResume == null)
            {
                return NotFound("無這筆資料");
            }

            var theStudent = await _context.T會員學生s.FirstOrDefaultAsync(data => data.學生id == thisResume.F學員Id);
            if (theStudent == null)
            {
                return NotFound("無這筆資料");
            }

            var viewModel = new ResumeCreateViewModel
            {
                ResumeID = resumeID,
                StudentID = thisResume.F學員Id,

                Name = theStudent.姓名,
                Photo = theStudent.圖片,
                Gender = theStudent.性別,
                Birth = theStudent.生日,
                Email = theStudent.信箱,
                Phone = theStudent.手機,
                School = theStudent.學校,
                Department = theStudent.科系,
                Academic = theStudent.學位,
                Graduated = theStudent.畢肄,

                ResumeTitle = thisResume.F履歷名稱,
                ResumeStatus = thisResume.F履歷狀態,
                Skill = thisResume.F專長技能,
                Language = thisResume.F語文能力,
                WorkExperience = thisResume.F有無工作經驗,
                WorkType = thisResume.F工作性質,
                WorkTime = thisResume.F工作時段,
                WorkShift = thisResume.F配合輪班,
                HopeJobTitle = thisResume.F希望職稱,
                HopeSalary = thisResume.F希望薪水待遇,
                HopeLocation = thisResume.F希望工作地點,
                Autobiography = thisResume.F自傳,
            };
            return View(viewModel);
        }

        /// <summary>
        /// 編輯履歷2：更新資料庫的履歷資料。
        /// </summary>
        /// <param name="resumeID">履歷ID</param>
        /// <param name="viewModel">包含更新資訊的履歷視圖模型</param>
        // POST: job_vacancy/Resume/UpdateResume/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResume(int resumeID,
            [Bind("ResumeID, StudentID, Name, Birth, Photo, Gender, Phone, Email, School, Department, Academic, Graduated, ResumeTitle, ResumeStatus, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography")] ResumeCreateViewModel viewModel)
        {
            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);
                if (thisResume == null || resumeID != viewModel.ResumeID)
                {
                    return NotFound();
                }

                var theStudent = await _context.T會員學生s.FindAsync(thisResume.F學員Id);
                if (theStudent == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    //更新學生資料
                    var originalPhoto = theStudent.圖片;
                    byte[]? photoData = await ReadUploadImage(Request.Form.Files["Photo"], originalPhoto);

                    theStudent.姓名 = viewModel.Name;
                    theStudent.圖片 = photoData;
                    theStudent.性別 = viewModel.Gender;
                    theStudent.手機 = viewModel.Phone;
                    theStudent.生日 = viewModel.Birth;
                    theStudent.信箱 = viewModel.Email;
                    theStudent.學校 = viewModel.School;
                    theStudent.學位 = viewModel.Academic;
                    theStudent.科系 = viewModel.Department;
                    theStudent.畢肄 = viewModel.Graduated;
                    theStudent.修改日期 = DateTime.Now;
                    _context.Update(theStudent);

                    //更新履歷資料
                    thisResume.F履歷名稱 = viewModel.ResumeTitle;
                    thisResume.F履歷狀態 = viewModel.ResumeStatus;
                    thisResume.F專長技能 = viewModel.Skill;
                    thisResume.F語文能力 = viewModel.Language;
                    thisResume.F有無工作經驗 = viewModel.WorkExperience;
                    thisResume.F工作性質 = viewModel.WorkType;
                    thisResume.F工作時段 = viewModel.WorkTime;
                    thisResume.F配合輪班 = viewModel.WorkShift;
                    thisResume.F希望職稱 = viewModel.HopeJobTitle;
                    thisResume.F希望薪水待遇 = viewModel.HopeSalary;
                    thisResume.F希望工作地點 = viewModel.HopeLocation;
                    thisResume.F自傳 = viewModel.Autobiography;
                    thisResume.F最後更新時間 = DateTime.Now;


                    _context.Update(thisResume);
                    await _context.SaveChangesAsync();

                    return Json(new { redirectUrl = Url.Action("Index", "Resume") });
                }
                else
                {
                    string errorMessage = string.Join("；", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }

        /// <summary>
        /// 刪除履歷（軟刪除）
        /// </summary>
        /// <param name="resumeID">履歷ID</param>
        /// <param name="deleteReason">刪除履歷的原因</param>
        // POST: job_vacancy/Resume/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int resumeID, string deleteReason)
        {
            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);

                if (thisResume == null)
                {
                    return NotFound("無這筆資料");
                }

                thisResume.F刪除狀態 = "1";
                thisResume.F刪除或關閉原因 = deleteReason;
                thisResume.F最後更新時間 = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                return Problem("刪除失敗：" + ex.Message, statusCode: 500);
            }
        }

        // GET: job_vacancy/Resume/Detail/5
        public async Task<IActionResult> Detail(int resumeID)
        {

            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);

                if (thisResume == null)
                {
                    return NotFound("無這筆資料");
                }

                var theStudent = await _context.T會員學生s.FirstOrDefaultAsync(data => data.學生id == thisResume.F學員Id);
                if (theStudent == null)
                {
                    return NotFound("無這筆資料");
                }

                var viewModel = new ResumeCreateViewModel
                {
                    ResumeID = resumeID,
                    StudentID = thisResume.F學員Id,

                    Name = theStudent.姓名,
                    Photo = theStudent.圖片,
                    Gender = theStudent.性別,
                    Birth = theStudent.生日,
                    Email = theStudent.信箱,
                    Phone = theStudent.手機,
                    School = theStudent.學校,
                    Department = theStudent.科系,
                    Academic = theStudent.學位,
                    Graduated = theStudent.畢肄,

                    ResumeTitle = thisResume.F履歷名稱,
                    ResumeStatus = thisResume.F履歷狀態,
                    Skill = thisResume.F專長技能,
                    Language = thisResume.F語文能力,
                    WorkExperience = thisResume.F有無工作經驗,
                    WorkType = thisResume.F工作性質,
                    WorkTime = thisResume.F工作時段,
                    WorkShift = thisResume.F配合輪班,
                    HopeJobTitle = thisResume.F希望職稱,
                    HopeSalary = thisResume.F希望薪水待遇,
                    HopeLocation = thisResume.F希望工作地點,
                    Autobiography = thisResume.F自傳,
                };

                return PartialView("_DetailsPartial", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }


        /// <summary>
        /// 根據學生編號顯示照片。
        /// </summary>
        /// <param name="studentID">學生ID</param>
        // GET: job_vacancy/Resume/ShowPhoto/5
        public async Task<FileResult> ShowPhoto(int studentID)
        {
            var student = await _context.T會員學生s.FindAsync(studentID);
            byte[]? pic = student?.圖片;
            return File(pic, "image/jpeg");
        }

        /// <summary>
        /// 從上傳的圖片檔案中讀取圖片資料。
        /// 如果沒有提供新的圖片，則保留原始圖片資料；
        /// 如果沒有原始圖片資料且未提供新的圖片，則返回空值。
        /// </summary>
        /// <param name="photoFile">上傳的圖片檔案</param>
        /// <param name="originalPhoto">原始圖片資料</param>
        private async Task<byte[]?> ReadUploadImage(IFormFile? photoFile, byte[]? originalPhoto)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
            {
                // 如果沒有上傳新的圖片且沒有原始圖片資料，則返回空值
                return originalPhoto;
            }
        }
    }
}
