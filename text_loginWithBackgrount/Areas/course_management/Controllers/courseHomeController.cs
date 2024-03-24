using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using text_loginWithBackgrount.Areas.course_management.Model;
using text_loginWithBackgrount.Areas.course_management.ViewModel.courseHome;
using static text_loginWithBackgrount.Areas.course_management.Model.RateModel;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class courseHomeController : Controller
    {
        private readonly studentContext _context;

        public courseHomeController(studentContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 這個方法用於登入系統中的老師使用者可以進行行政管理
        /// </summary>
        ///<returns>成功登入並點選後，返回排課系統管理頁面。</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 這個方法用於登入系統中的老師使用者，以進入其教授課程的頁面。
        /// </summary>
        /// <param name="id">老師的老師id。</param>
        /// <returns>成功登入後，返回教授課程的頁面；同時撈資料呈現class、teacher、courseTime的資料，如果有null前端呈現不同訊息。</returns>
        public async Task<IActionResult> TeacherCourseIndex(int? id)
        {
            try
            {
                var classContext = await _context.T課程班級科目s
                    .Include(t => t.班級)
                    .Include(t => t.科目)
                    .Where(t => t.老師id == id)
                    .Select(t => new
                    {
                        班級 = t.班級,
                        班級科目 = t,
                        科目 = t.科目
                    })
                    .ToListAsync();

                //如果Class null前端處理呈現不同的html元素
                ViewBag.Class = classContext;

                var teacherContext = await _context.T會員老師s
                    .Where(t => t.老師id == id)
                    .FirstOrDefaultAsync();

                //如果Teacher null前端處理呈現不同的html元素
                ViewBag.Teacher = teacherContext;

                //撈所有課程資料和撈這個老師id的班級科目資料去比較，如果相同就撈出
                var courses = await _context.T課程課程s.ToListAsync();
                var classSubjects = await _context.T課程班級科目s
                    .Where(t => t.老師id == id)
                    .ToListAsync();

                var courseTimeContext =
                    (from course in courses
                     join classSubject in classSubjects on course.班級科目id equals classSubject.班級科目id
                     select new TeacherCourseTimeViewModel
                     {
                         課程 = course,
                         科目 = classSubject,
                         班級id = classSubject.班級id,
                         班級名稱 = classSubject.班級.班級名稱,

                     }).ToList();

                //如果courseTime null的話前端的行事曆插件不會報錯
                return View(courseTimeContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }
        }


        /// <summary>
        /// 這個方法用於確認班級科目表的資料狀態欄位，等於7代表這個班級科目已經開放學生能填寫表單。
        /// </summary>
        /// <param name="id">班級科目id。</param>
        /// <returns>返回ok,data:true表示開啟表單；否則返回ok,data:false表示已經開啟過了。</returns>
        public async Task<IActionResult> toggleStatus(int? id)
        {
            try
            {
                var t課程班級科目 = await _context.T課程班級科目s.FindAsync(id);

                if (t課程班級科目 == null)
                {
                    return NotFound();
                }

                if (t課程班級科目.狀態 == 7)
                {
                    //已經開啟過
                    return Ok(new { data = true });
                }
                else
                {

                    //尚未開啟: 1.班級科目狀態改成7 2.撈班級科目的班級中所有學生 3.把所有學生對評分主表新增

                    t課程班級科目.狀態 = 7;

                    var studentIds =
                      await _context.T課程學生班級s
                     .Where(tc => tc.班級id == t課程班級科目.班級id)
                     .Select(tc => tc.學生id)
                     .ToListAsync();

                    foreach (var StudentId in studentIds)
                    {
                        var t課程評分主表 = new T課程評分主表()
                        {
                            班級科目id = t課程班級科目.班級科目id,
                            學生id = StudentId,
                            提交時間 = DateTime.Now,
                            狀態 = 1,
                            改進意見 = "",
                        };
                        _context.Add(t課程評分主表);
                        await _context.SaveChangesAsync();

                        var FormId = t課程評分主表.評分主表id;

                        var Model = new RateModel();
                        foreach (var 分類 in Model.評分類別列表)
                        {
                            foreach (var 題目 in 分類.題目)
                            {
                                var t課程評分詳細表 = new T課程評分
                                {
                                    評分主表id = FormId,
                                    評分分類 = 分類.分類名稱,
                                    評分題目 = 題目.題目內容,
                                    評分 = 0,
                                    狀態 = 1
                                };
                                _context.Add(t課程評分詳細表);
                            }
                        }

                        await _context.SaveChangesAsync();

                    }


                    return Ok(new { data = false });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");

            }
        }


        /// <summary>
        /// 這個方法用於確認通知表的中有沒有這個老師id尚未被讀到的通知。
        /// </summary>
        /// <param name="id">老師id。</param>
        /// <returns>返回true表示有尚未讀到的；返回false表示都讀過了。</returns>        
        public bool notificationState(int? id)
        {
            try
            {
                var messageUnread = _context
                    .T課程通知表s
                    .Where(t => t.接收者類型 == "T" && t.接收者id == id && t.狀態 == 1)
                    .Count();

                //如果有尚未有讀的，回傳要有紅點
                if (messageUnread > 0)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return false;
            }

        }


        /// <summary>
        /// 這個方法用於返回現在這個老師id的通知頁面，小鈴鐺點即進入後的頁面。1.更新這個老師的所有通知都已讀
        /// </summary>
        /// <param name="id">老師id。</param>
        /// <returns>老師通知表頁面</returns>      
        async public Task<IActionResult> notificationView(int? id)
        {

            try
            {

                //如果messageList為null前端處理呈現不同的資訊
                var messageList = await _context.T課程通知表s
               .Where(t => t.接收者類型 == "T" && t.接收者id == id)
               .OrderByDescending(t => t.時間)
               .ToListAsync();

                if (messageList.Count <= 0)
                {
                    //model為null
                    return View(messageList);
                }
                else
                {

                    //1.先更新為2，表示為已讀
                    foreach (var message in messageList)
                    {
                        message.狀態 = 2;
                        _context.Update(message);

                    }

                    await _context.SaveChangesAsync();
                    //2. 撈這個資料的發送者和接收者
                    var notificationList = messageList.Select(message => new Notification
                    {
                        訊息id = message.訊息id,
                        發送者類型 = message.發送者類型,
                        發送者id = message.發送者id,
                        接收者類型 = message.接收者類型,
                        接收者id = message.接收者id,
                        發送訊息內容 = message.發送訊息內容,
                        狀態 = message.狀態,
                        時間 = message.時間,
                        SenderName = message.發送者類型 == "T" ?
                        _context.T會員老師s.FirstOrDefault(s => s.老師id == message.發送者id)?.姓名 :
                        _context.T會員學生s.FirstOrDefault(s => s.學生id == message.發送者id)?.姓名
                    }).ToList();

                    return View(notificationList);


                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }


        }


        /// <summary>
        /// 這個方法用於返回現在這個班級科目id的老師端直播畫面。
        /// </summary>
        /// <param name="id">班級科目id。</param>
        /// <returns>classFeature頁面</returns>  
        public IActionResult classFeature(int id)
        {
            try
            {
                if (id != null || id != 0)
                {
                    var classsCourseContext = _context.T課程班級科目s.Where(t => t.班級科目id == id).Include(t => t.科目).Include(t => t.班級).FirstOrDefault();
                    if (classsCourseContext != null)
                    {
                        ViewBag.classCourseId = id;
                        return View(classsCourseContext);
                    }
                }

                return View("Errors");

            }
            catch (Exception ex)
            {

                Console.WriteLine($"發生錯誤: {ex.Message}");
                return View("Errors");
            }

        }

        /// <summary>
        /// 這個方法用於返回現在這個班級科目id的老師端白板畫面。
        /// </summary>
        /// <param name="id">班級科目id。</param>
        /// <returns>classFeature頁面</returns>  
        public IActionResult whiteBoard(int id)
        {
            ViewBag.classCourseId = id;
            return View();
        }
    }
}

