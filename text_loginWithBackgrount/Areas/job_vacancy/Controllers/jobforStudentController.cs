using Azure;
using Azure.AI.TextAnalytics;
using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// 學生專區 - 就業媒合主要頁面（我的履歷）。
        /// 進入此區即啟動推薦系統。
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            //獲取登入學生的ID
            var user = HttpContext.User.Claims.ToList();
            var loginID = Convert.ToInt32(user.Where(a => a.Type == "StudentId").First().Value);

            RecommandSystem(loginID);

            return View();
        }

        /// <summary>
        /// 新增履歷1：返回 partial view。
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
            [Bind("StudentID, Name, Birth, Photo, Gender, Phone, Email, School, Academic, Department, Graduated, ResumeTitle, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography, ThisResumeWorkExpIDs")] ResumeCreateViewModel viewModel)
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
                    _context.T工作履歷資料s.Add(newResume);
                    await _context.SaveChangesAsync();


                    //新增履歷表工作經驗資料
                    var resumeWorkExpIDs = viewModel.ThisResumeWorkExpIDs;
                    if (resumeWorkExpIDs != null && resumeWorkExpIDs.Any())
                    {
                        for (int i = 0; i < resumeWorkExpIDs.Count; i++)
                        {
                            T工作履歷表工作經驗 resumeWorkExp = new T工作履歷表工作經驗
                            {
                                F履歷Id = newResume.FId,
                                F工作經驗Id = resumeWorkExpIDs[i]
                            };

                            _context.T工作履歷表工作經驗s.Add(resumeWorkExp);
                        }
                    }
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
        /// 編輯履歷1：根據履歷編號提取相應的資料，並返回 partial view。
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
        /// <param name="viewModel">包含更新資訊的履歷視圖模型</param>
        // POST: job_vacancy/jobforStudent/UpdateResume/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResume([Bind("ResumeID, Name, Birth, Photo, Gender, Phone, Email, School, Department, Academic, Graduated, ResumeTitle, HopeJobTitle, HopeSalary, HopeLocation, Skill, Language, WorkExperience, WorkType, WorkTime, WorkShift, Autobiography, ThisResumeWorkExpIDs")] ResumeCreateViewModel viewModel)
        {
            
            try
            {
                var thisResume = await _context.T工作履歷資料s.FindAsync(viewModel.ResumeID);
                if (thisResume == null)
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
                    // 進行關鍵字提取
                    // 將表單資料轉換為 JSON 物件

                    //去除履歷資料的 HTML 標籤
                    string plainTextContent = Regex.Replace(viewModel.Autobiography, "<.*?>", System.String.Empty);
                    var jsonData = new
                    {
                        //學位 = viewModel.Academic,
                        科系 = viewModel.Department,
                        專長技能 = viewModel.Skill,
                        語文能力 = viewModel.Language,
                        //工作性質 = viewModel.WorkType,
                        //工作時段 = viewModel.WorkTime,
                        //配合輪班 = viewModel.WorkShift,
                        //希望職稱 = viewModel.HopeJobTitle,
                        //希望薪水待遇 = viewModel.HopeSalary,
                        //希望工作地點 = viewModel.HopeLocation,
                        自傳 = plainTextContent
                    };

                    // 將 JSON 物件序列化為 JSON 字串
                    var jsonString = JsonConvert.SerializeObject(jsonData);

                    //關鍵字擷取服務
                    AzureKeyCredential credentials = new AzureKeyCredential("b8c172267f6b4d8399ca5953a265126e");
                    Uri endpoint = new Uri("https://analysisrecommendation.cognitiveservices.azure.com/");

                    TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);
                    Response<KeyPhraseCollection> response = client.ExtractKeyPhrases(jsonString);
                    KeyPhraseCollection resumeKeyPhrases = response.Value;

                    //將集合結合為字串
                    string resumeKeyString = string.Join(", ", resumeKeyPhrases);


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
                    thisResume.F關鍵字 = resumeKeyString;

                    //更新履歷表工作經驗資料
                    var resumeWorkExpIDs = viewModel.ThisResumeWorkExpIDs;
                    if (resumeWorkExpIDs != null && resumeWorkExpIDs.Any())
                    {
                        //刪除舊資料
                        var oldData = _context.T工作履歷表工作經驗s.Where(w => w.F履歷Id == viewModel.ResumeID);
                        _context.T工作履歷表工作經驗s.RemoveRange(oldData);

                        //新增
                        for (int i = 0; i < resumeWorkExpIDs.Count; i++)
                        {
                            T工作履歷表工作經驗 resumeWorkExp = new T工作履歷表工作經驗
                            {
                                F履歷Id = viewModel.ResumeID,
                                F工作經驗Id = resumeWorkExpIDs[i]
                            };

                            _context.T工作履歷表工作經驗s.Add(resumeWorkExp);
                        }
                    }

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
        /// 返回履歷詳細頁中，履歷工作經驗 partial view
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="resumeID"></param>
        /// <returns></returns>
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
        /// 新增工作經驗1：返回填寫表單 partial view
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
        /// 編輯工作經驗1：返回填寫表單 partial view
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
                return originalPhoto;
            }
        }


        /// <summary>
        /// 推薦系統
        /// </summary>
        /// <returns></returns>
        public void RecommandSystem(int studentID)
        {
            // 建立字典，用於存儲職缺ID和對應的總體相似度
            Dictionary<int, double> jobSimilarityMap = new Dictionary<int, double>();

            // 提取最新一份履歷資料中的對應欄位
            var resumeData = _context.T工作履歷資料s
                .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                .Include(r => r.F學員)
                .OrderByDescending(r => r.F最後更新時間)
                .Select(r => new {
                    resumeID = r.FId,
                    學位 = r.F學員.學位,
                    科系 = r.F學員.科系,
                    專長技能 = r.F專長技能,
                    語文能力 = r.F語文能力,
                    工作性質 = r.F工作性質,
                    工作時段 = r.F工作時段,
                    希望職稱 = r.F希望職稱,
                    希望薪水待遇 = r.F希望薪水待遇,
                    希望工作地點 = r.F希望工作地點,
                    自傳 = r.F自傳,
                    關鍵字 = r.F關鍵字
                })
                .FirstOrDefault();

            // 提取所有職缺資料中的對應欄位
            var allJobData = _context.T工作職缺資料s
                .Where(j => j.F刪除狀態 == "0" && j.F職缺狀態 == "公開")
                .Include(j => j.F公司)
                .Select(j => new {
                    jobID = j.FId,
                    學歷要求 = j.F學歷要求,
                    其他條件 = j.F其他條件,
                    工作技能 = j.F工作技能,
                    工作內容 = j.F工作內容,
                    語文條件 = j.F語文條件,
                    工作性質 = j.F工作性質,
                    工作時段 = j.F工作時段,
                    職務名稱 = j.F職務名稱,
                    薪水待遇 = j.F薪水待遇,
                    工作地點 = j.F工作地點,
                    公司簡介 = j.F公司.F公司簡介,
                    關鍵字 = j.F關鍵字
                })
                .ToList();

            int number = 1;
            // 計算每個欄位之間的相似度並判斷是否匹配
            foreach (var job in allJobData)
            {
                double overallSimilarity = 0.0;

                // 計算工作性質相似度
                double 工作性質Similarity = (resumeData.工作性質 == job.工作性質) ? 1.0 : 0.0;
                overallSimilarity += 工作性質Similarity;

                // 計算工作時段相似度
                double 工作時段Similarity = CalculateSimilarity(resumeData.工作時段 ?? "", job.工作時段 ?? "");
                overallSimilarity += 工作時段Similarity;

                // 計算職務名稱相似度
                double 職務名稱Similarity = CalculateSimilarity(resumeData.希望職稱 ?? "", job.職務名稱 ?? "");
                overallSimilarity += 職務名稱Similarity;

                // 計算工作地點相似度
                double 工作地點Similarity = CalculateSimilarity(resumeData.希望工作地點 ?? "", job.工作地點 ?? "");
                overallSimilarity += 工作地點Similarity;


                // 計算關鍵字相似度
                var theJobKeywords = job.關鍵字?.Split(',')?.ToList() ?? new List<string>();
                var resumeKeyword = resumeData.關鍵字?.Split(',')?.ToList() ?? new List<string>();

                // 建立詞彙表，將履歷關鍵字和這份職缺關鍵字合併成一個集合
                HashSet<string> vocabulary = new HashSet<string>(resumeKeyword.Concat(theJobKeywords));

                double keywordSimilarity = 0.0;

                // 遍歷每個履歷關鍵字
                foreach (var keyword in resumeKeyword)
                {
                    // 使用當前的履歷關鍵字建立文檔向量
                    double[] resumeVector = CreateDocumentVector(new List<string> { keyword }, vocabulary);

                    // 使用當前的職缺關鍵字建立文檔向量
                    double[] jobVector = CreateDocumentVector(theJobKeywords, vocabulary);

                    // 計算當前履歷和職缺的相似度
                    double similarity = CalculateCosineSimilarity(resumeVector, jobVector);

                    keywordSimilarity += similarity;
                }

                // 將關鍵字相似度添加到總體相似度中
                overallSimilarity += keywordSimilarity;

                // 將總體相似度存入字典中
                jobSimilarityMap.Add(job.jobID, overallSimilarity);

                number++;
            }

            // 根據相似度排序
            var sortedJobs = jobSimilarityMap.OrderByDescending(kv => kv.Value);

            // 取前五個職缺的ID和對應的 overallSimilarity
            var topFiveJobs = sortedJobs.Take(5).Select(kv => new { JobID = kv.Key, Similarity = kv.Value }).ToList();

            // 格式化相似度到小數點後兩位
            var formattedTopFiveJobs = topFiveJobs.Select(j => new { JobID = j.JobID, Similarity = Math.Round(j.Similarity, 2) });

            //更新推薦職缺驗資料
            if (formattedTopFiveJobs != null && formattedTopFiveJobs.Any())
            {
                //刪除舊資料
                var oldData = _context.T工作推薦職缺s.Where(d => d.F學員Id == studentID);
                _context.T工作推薦職缺s.RemoveRange(oldData);

                //新增
                foreach (var item in formattedTopFiveJobs)
                {
                    T工作推薦職缺 recommendRecord = new T工作推薦職缺
                    {
                        F學員Id = studentID,
                        F職缺Id = item.JobID,
                        F推薦程度 = item.Similarity.ToString(),
                    };

                    _context.T工作推薦職缺s.Add(recommendRecord);
                }
            }

            _context.SaveChanges();
        }

        public double CalculateSimilarity(string str1, string str2)
        {
            // 假設這裡使用 Levenshtein 距離作為相似度計算方法
            int distance = LevenshteinDistance(str1, str2);
            int maxLength = Math.Max(str1.Length, str2.Length);
            return 1 - (double)distance / maxLength;
        }

        public int LevenshteinDistance(string str1, string str2)
        {
            int[,] dp = new int[str1.Length + 1, str2.Length + 1];
            for (int i = 0; i <= str1.Length; i++)
            {
                dp[i, 0] = i;
            }
            for (int j = 0; j <= str2.Length; j++)
            {
                dp[0, j] = j;
            }
            for (int i = 1; i <= str1.Length; i++)
            {
                for (int j = 1; j <= str2.Length; j++)
                {
                    int cost = str1[i - 1] == str2[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }
            return dp[str1.Length, str2.Length];
        }


        /// <summary>
        /// 推薦系統--關鍵字（目前沒用）
        /// </summary>
        /// <returns></returns>
        [Route("/job_vacancy/jobforStudent/{Action=Index}/{studentID}")]
        public IActionResult RecommandSystemByKeyword(int studentID = 17)
        {
            //建立結果列表
            var result = new List<string>();

            var theReusmesAllKeywords = _context.T工作履歷資料s
                                        .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                                        .Select(r => r.F關鍵字 ?? "")
                                        .ToList() // 將查詢結果檢索到記憶體中（可能導致性能問題）
                                        .SelectMany(k => k.Split(',')) // 進行處理
                                        .Distinct()
                                        .ToList();

            var allJobKeywords = _context.T工作職缺資料s
                                 .Where(j => j.F刪除狀態 == "0" && j.F職缺狀態 == "公開")
                                 .Select(j => j.F關鍵字)
                                 .ToList();

            // 遍歷每個職缺關鍵字清單
            foreach (var jobKeywords in allJobKeywords)
            {
                var theJobKeywords = jobKeywords.Split(',').ToList();

                // 建立詞彙表，將履歷關鍵字和這份職缺關鍵字合併成一個集合
                HashSet<string> vocabulary = new HashSet<string>(theReusmesAllKeywords.Concat(theJobKeywords));

                // 遍歷每個履歷關鍵字
                foreach (var resumeKeyword in theReusmesAllKeywords)
                {
                    // 使用當前的履歷關鍵字建立文檔向量
                    double[] resumeVector = CreateDocumentVector(new List<string> { resumeKeyword }, vocabulary);

                    // 使用當前的職缺關鍵字建立文檔向量
                    double[] jobVector = CreateDocumentVector(theJobKeywords, vocabulary);

                    // 計算當前履歷和職缺的相似度
                    double similarity = CalculateCosineSimilarity(resumeVector, jobVector);

                    // 將相似度計算結果添加到結果列表
                    result.Add($"履歷關鍵字：{resumeKeyword}，職缺關鍵字：{string.Join(",", jobKeywords)}，相似度：{similarity}");
                }
            }

            // 返回相似度計算結果列表
            return Ok(result);
        }

        // 建立文檔向量的方法
        public static double[] CreateDocumentVector(List<string> keywords, HashSet<string> vocabulary)
        {
            double[] vector = new double[vocabulary.Count];
            foreach (string keyword in keywords)
            {
                int index = Array.IndexOf(vector, 0); // 找到向量中第一個空位
                vector[index] = keywords.Count(kw => kw == keyword); // 計算該關鍵字在文檔中出現的次數，作為向量的元素值
            }
            return vector;
        }

        // 計算餘弦相似度的方法
        public static double CalculateCosineSimilarity(double[] vector1, double[] vector2)
        {
            double dotProduct = DotProduct(vector1, vector2);
            double magnitude1 = Math.Sqrt(DotProduct(vector1, vector1));
            double magnitude2 = Math.Sqrt(DotProduct(vector2, vector2));
            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0; // 避免除以0
            }
            return dotProduct / (magnitude1 * magnitude2);
        }

        // 計算兩個向量的內積的方法
        public static double DotProduct(double[] vector1, double[] vector2)
        {
            double result = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                result += vector1[i] * vector2[i];
            }
            return result;
        }
    }
}

