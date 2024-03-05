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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TeacherCourseIndex(int? id)
        {
            var classContext = _context
                .T課程班級科目s
                .Include(t => t.班級)
                .Include(t => t.科目)
                .Where(t => t.老師id == id)
                .Select(t => new {
                    班級 = t.班級,
                    班級科目 = t,
                    科目 = t.科目
                })
                .ToList();

            ViewBag.Class = classContext;

            var teacherContext = _context
                .T會員老師s
            .Where(t => t.老師id == id)
                .FirstOrDefault();

            ViewBag.Teacher = teacherContext;

            var courses = _context.T課程課程s.ToList();
            var classSubjects = _context.T課程班級科目s
             .Where(t => t.老師id == id)
             .ToList();

            var courseTimeContext =
            (
                from course in courses
                join classSubject in classSubjects on course.班級科目id equals classSubject.班級科目id
                select new TeacherCourseTimeViewModel
                {
                    課程 = course,
                    科目 = classSubject,
                    班級id = classSubject.班級id,
                    班級名稱 = classSubject.班級.班級名稱,

                }
            )
            .ToList();


            return View(courseTimeContext);
        }

        public async Task<IActionResult> toggleStatus(int? id)
        {
            //id==班級科目id
            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(id);

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
                    // 意見主表記得要更改資料表結構
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

                   var FormId= t課程評分主表.評分主表id;

                    var Model=new RateModel();
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

        //目前當前使用者的id
        public bool notificationState(int? id)
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
        //目前當前使用者的id
        async public Task<IActionResult> notificationView(int? id)
        {
            var messageList = await _context.T課程通知表s
                .Where(t => t.接收者類型 == "T" && t.接收者id == id)
                .OrderByDescending(t => t.時間)
                .ToListAsync();

            try
            {
                foreach (var message in messageList)
                {
                    message.狀態 = 2;
                    _context.Update(message);

                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

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
                SenderName = message.發送者類型 == "T" ? _context.T會員老師s.FirstOrDefault(s => s.老師id == message.發送者id)?.姓名 :
                                                _context.T會員學生s.FirstOrDefault(s => s.學生id == message.發送者id)?.姓名
            }).ToList();

            return View(notificationList);
        }
    }
}

