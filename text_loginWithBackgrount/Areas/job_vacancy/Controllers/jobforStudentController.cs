using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("job_vacancy")]
    [Route("/job_vacancy/jobforStudent/{Action=Index}/{resumeID?}")]
    public class jobforStudentController : Controller
    {
        private readonly studentContext _context;

        public jobforStudentController(studentContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ResumeDtails()
        {
            return View();
        }

        /// <summary>
        /// 新增履歷1：返回視圖。
        /// </summary>
        // GET: job_vacancy/jobforStudent/Create/5
        [Route("/job_vacancy/jobforStudent/{Action=Index}/{studentID?}")]
        public async Task<IActionResult> Create(int studentID)
        {
            try
            {
                var theStudent = await _context.T會員學生s.FirstOrDefaultAsync(data => data.學生id == studentID);

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

                    return PartialView("_CreatePartial", viewModel);
                }
                else
                {
                    // 如果學生不存在，返回查無此學生
                    return NotFound("查無此學生");
                }
            }
            catch (Exception ex)
            {
                // 如果發生異常，返回問題詳細訊息
                return Problem(detail: ex.Message + "異常");
            }
        }

        /// <summary>
        /// 新增履歷2：將前端傳回的資料進行驗證，通過後存入資料庫。
        /// </summary>
        /// <param name="viewModel"></param>
        // POST: job_vacancy/jobforStudent/CreateResume
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResume(
            [Bind("StudentID, Name, Birth, Photo, Gender, Phone, Email, School, Academic, Department, Graduated, ResumeTitle, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography")] ResumeCreateViewModel viewModel)
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

                    return Json(new { success = true, message = "新增成功" });

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
        // GET: job_vacancy/jobforStudent/EditResume/5
        public async Task<IActionResult> EditResume(int resumeID)
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

            var studentWorkExp = await _context.T工作工作經驗s
                                .Where(data => data.F學員Id == thisResume.F學員Id)
                                .ToListAsync();

            var thisResumeWorkExpIDs = await _context.T工作履歷表工作經驗s
                                       .Where(data => data.F履歷Id == thisResume.FId)
                                       .Select(data => data.FId)
                                       .ToListAsync();

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
                ThisResumeWorkExpIDs = thisResumeWorkExpIDs,
                StudentWorkExp = studentWorkExp
            };
            //return View(viewModel);
            return PartialView("_EditPartial", viewModel);
        }


        /// <summary>
        /// 編輯履歷2：更新資料庫的履歷資料。
        /// </summary>
        /// <param name="resumeID">履歷ID</param>
        /// <param name="viewModel">包含更新資訊的履歷視圖模型</param>
        // POST: job_vacancy/jobforStudent/UpdateResume/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResume(int resumeID,
            [Bind("ResumeID, Name, Birth, Photo, Gender, Phone, Email, School, Department, Academic, Graduated, ResumeTitle, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography")] ResumeCreateViewModel viewModel)
        {
            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);
                if (thisResume == null || resumeID != viewModel.ResumeID)
                {
                    return NotFound("無此履歷");
                }

                var theStudent = await _context.T會員學生s.FindAsync(thisResume.F學員Id);
                if (theStudent == null)
                {
                    return NotFound("無此學生");
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
                    thisResume.F履歷狀態 = "公開";  //已拔除更改狀態的功能，固定為公開狀態
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

                    return Json(new { success = true, message = "修改成功" });
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
        // POST: job_vacancy/jobforStudent/Delete/
        [HttpPost]
        public async Task<IActionResult> Delete(int resumeID, string deleteReason)
        {
            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(resumeID);

                if (thisResume == null)
                {
                    return NotFound("無這筆履歷資料");
                }

                thisResume.F刪除狀態 = "1";
                thisResume.F刪除或關閉原因 = deleteReason;
                thisResume.F最後更新時間 = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "刪除失敗：" + ex.Message });
            }
        }

        /// <summary>
        /// 根據學生編號顯示照片。
        /// </summary>
        /// <param name="studentID">學生ID</param>
        // GET: job_vacancy/jobforStudent/ShowPhoto/5
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



        // GET: job_vacancy/jobforStudent/GetMyWorkExp
        [Route("/job_vacancy/jobforStudent/{Action=Index}")]
        public async Task<IActionResult> GetMyWorkExp(int studentID, int? resumeID = null)
        {
            var workData = await _context.T工作工作經驗s
                            .Where(w => w.F學員Id == studentID)
                            .ToListAsync();

            var viewModelList = new List<WorkExpViewModel>();

            foreach (var data in workData)
            {
                var isInResume = false; // 預設為 false

                // 如果提供了 resumeID，則檢查是否存在對應的工作經驗
                if (resumeID != null)
                {
                    // 查詢履歷表中是否存在與當前工作經驗相關聯的記錄
                    isInResume = _context.T工作履歷表工作經驗s.Any(r => r.F工作經驗Id == data.FId && r.F履歷Id == resumeID);
                }


                var viewModel = new WorkExpViewModel
                {
                    WorkExpID = data.FId,
                    CompanyName = data.F公司名稱,
                    JobTitle = data.F職務名稱,
                    Start = data.F起始年月,
                    End = data.F結束年月,
                    Salary = data.F薪水待遇,
                    JobContent = data.F工作內容,
                    IsInResume = isInResume

                };
                viewModelList.Add(viewModel);
            }

            return PartialView("_workExpPartial", viewModelList);
        }

        /// <summary>
        /// 新增工作經驗1：返回填寫表單的視圖
        /// </summary>
        /// <returns></returns>
        // GET: job_vacancy/jobforStudent/GetAddWorkExpView
        [Route("/job_vacancy/jobforStudent/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetAddWorkExpView(int studentID)
        {
            var student = await _context.T會員學生s
                          .Where(s => s.學生id == studentID)
                          .FirstOrDefaultAsync();

            int currentYear = DateTime.Now.Year;
            int startYear = currentYear - 18; // 預設起始年為18年前
            if (student != null && student.生日.HasValue)
            {
                startYear = student.生日.Value.Year + 18;
            }

            // 設定工作經驗年份範圍
            List<int> availableYears = new List<int>();
            for (int year = startYear; year <= currentYear; year++)
            {
                availableYears.Add(year);
            }

            var viewModel = new WorkExpViewModel
            {
                AvailableYears = availableYears,
            };

            return PartialView("_CreateWorkExpPartialView", viewModel);
        }


        /// <summary>
        /// 新增工作經驗2：將前端傳回的資料進行驗證，通過後存入資料庫。
        /// </summary>
        /// <param name="viewModel"></param>
        // POST: job_vacancy/jobforStudent/AddNewWorkExp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewWorkExp(
            [Bind("StudentID, CompanyName, JobTitle, Start, End, Salary, JobContent")] WorkExpViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 新增履歷資料
                    var newWorkExp = new T工作工作經驗
                    {
                        F學員Id = viewModel.StudentID,
                        F公司名稱 = viewModel.CompanyName,
                        F職務名稱 = viewModel.JobTitle,
                        F起始年月 = viewModel.Start,
                        F結束年月 = viewModel.End,
                        F薪水待遇 = viewModel.Salary,
                        F工作內容 = viewModel.JobContent
                    };

                    // 將新的履歷添加到資料庫中
                    _context.T工作工作經驗s.Add(newWorkExp);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "新增成功" });

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
        /// 編輯工作經驗1：返回填寫表單的視圖
        /// </summary>
        /// <returns></returns>
        // GET: job_vacancy/jobforStudent/GetEditWorkExpView
        [Route("/job_vacancy/jobforStudent/{Action=Index}/{workExpID}")]
        public async Task<IActionResult> GetEditWorkExpView(int workExpID)
        {

            try
            {
                var theWorkExp = await _context.T工作工作經驗s
                                 .Where(w => w.FId == workExpID)
                                 .FirstOrDefaultAsync();

                if (theWorkExp != null)
                {
                    var studentID = theWorkExp.F學員Id;
                    var student = await _context.T會員學生s
                          .Where(s => s.學生id == studentID)
                          .FirstOrDefaultAsync();

                    int currentYear = DateTime.Now.Year;
                    int startYear = currentYear - 18; // 預設起始年為18年前
                    if (student != null && student.生日.HasValue)
                    {
                        startYear = student.生日.Value.Year + 18;
                    }

                    // 設定工作經驗年份範圍
                    List<int> availableYears = new List<int>();
                    for (int year = startYear; year <= currentYear; year++)
                    {
                        availableYears.Add(year);
                    }


                    var viewModel = new WorkExpViewModel
                    {
                        WorkExpID = workExpID,
                        StudentID = studentID,
                        CompanyName = theWorkExp.F公司名稱,
                        JobTitle = theWorkExp.F職務名稱,
                        Start = theWorkExp.F起始年月,
                        End = theWorkExp.F結束年月,
                        Salary = theWorkExp.F薪水待遇,
                        JobContent = theWorkExp.F工作內容,
                        AvailableYears = availableYears
                    };

                    return PartialView("_EditWorkExpPartialView", viewModel);
                }
                else
                {
                    return NotFound("查無此資料");
                }
            }
            catch (Exception ex)
            {
                // 如果發生異常，返回問題詳細訊息
                return Problem(detail: ex.Message + "異常");
            }

        }

        /// <summary>
        /// 編輯履歷2：更新資料庫的履歷資料。
        /// </summary>
        /// <param name="resumeID">履歷ID</param>
        /// <param name="viewModel">包含更新資訊的履歷視圖模型</param>
        // POST: job_vacancy/jobforStudent/UpdateResume
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWorkExp(
            [Bind("WorkExpID, StudentID, CompanyName, JobTitle, Start, End, Salary, JobContent")] WorkExpViewModel viewModel)
        {
            try
            {
                var thisWorkWxp = await _context.T工作工作經驗s.FindAsync(viewModel.WorkExpID);
                if (thisWorkWxp == null)
                {
                    return NotFound("無此工作經歷");
                }

                var theStudent = await _context.T會員學生s.FindAsync(thisWorkWxp.F學員Id);
                if (theStudent == null)
                {
                    return NotFound("無此學生");
                }

                if (ModelState.IsValid)
                {

                    thisWorkWxp.FId = viewModel.WorkExpID;
                    thisWorkWxp.F學員Id = viewModel.StudentID;
                    thisWorkWxp.F公司名稱 = viewModel.CompanyName;
                    thisWorkWxp.F職務名稱 = viewModel.JobTitle;
                    thisWorkWxp.F起始年月 = viewModel.Start;
                    thisWorkWxp.F結束年月 = viewModel.End;
                    thisWorkWxp.F薪水待遇 = viewModel.Salary;
                    thisWorkWxp.F工作內容 = viewModel.JobContent;

                    _context.Update(thisWorkWxp);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "修改成功" });
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
        /// 刪除工作經驗（硬刪除）
        /// </summary>
        // POST: job_vacancy/jobforStudent/DeleteWorkExp
        [HttpPost]
        [Route("/job_vacancy/jobforStudent/{Action=Index}/{workExpID}")]
        public async Task<IActionResult> DeleteWorkExp(int workExpID)
        {
            try
            {
                var thisWorkExp = await _context.T工作工作經驗s.FindAsync(workExpID);

                if (thisWorkExp == null)
                {
                    return NotFound("無這筆工作經驗資料");
                }

                var resumeWorkExp = _context.T工作履歷表工作經驗s
                                    .Where(w => w.F工作經驗Id == workExpID)
                                    .ToList();

                foreach (var exp in resumeWorkExp)
                {
                    _context.T工作履歷表工作經驗s.Remove(exp);
                }

                _context.T工作工作經驗s.Remove(thisWorkExp);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "刪除失敗：" + ex.Message });
            }
        }

    }
}

