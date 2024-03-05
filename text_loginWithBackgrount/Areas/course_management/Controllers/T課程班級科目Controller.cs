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
using Class_system_Backstage_pj.Areas.ordering_system.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using text_loginWithBackgrount.Areas.course_management.Model;
using Microsoft.AspNetCore.SignalR;

namespace Class_system_Backstage_pj.Areas.course_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("course_management")]
    public class T課程班級科目Controller : Controller
    {
        private readonly studentContext _context;
        private readonly IEmailSender _emailSender;

        public T課程班級科目Controller(studentContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
 
        public async Task<IActionResult> classCourseIndex(int? id)
        {
            var studentContext = _context.T課程班級科目s
            .Include(t => t.班級)
            .Include(t => t.科目)
            .Include(t => t.老師)
            .Where(t => t.班級id == id);

            var className = _context.T課程班級s.FirstOrDefault(c => c.班級id == id)?.班級名稱;
            ViewData["ClassName"] = className;
            ViewData["ClassId"] = id;
            return View(await studentContext.ToListAsync());

        }

        public async Task<IActionResult> courseIndex()
        {
            SelectList coursesSelectList = new SelectList(_context.T課程科目s.Select(c => new
            {
                coursesid = c.科目id,
                coursesName = c.科目名稱
            }), "coursesid", "coursesName");
            ViewBag.Course = coursesSelectList;


            return PartialView("_createPartial");

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



        [HttpPost]
        public async Task<IActionResult> Create()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var jsonData = await reader.ReadToEndAsync();

                // 將 JSON 數據轉換為 ClassCourseViewModel 對象
                var data = JsonConvert.DeserializeObject<ClassCourseViewModel>(jsonData);

                if (data == null)
                {
                    return NotFound();
                }

                if (data.班級id == null)
                {
                    return NotFound();
                }
                foreach (var subjectTeacherPair in data.班級科目)
                {
                    var 科目id = subjectTeacherPair.CourseId;
                    var 老師id = subjectTeacherPair.TeacherId;

                    // 將科目名稱和成績存入資料庫
                    var t班級科目 = new T課程班級科目
                    {
                        班級id = data.班級id,
                        科目id = Convert.ToInt32(科目id),
                        老師id = 老師id,
                        狀態 = 1
                    };
                    _context.Add(t班級科目);

                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(classCourseIndex));

                }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error: " + ex.Message); // 處理全局異常
            }

        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.T課程班級科目s == null)
            {
                return Problem("Entity set 'studentContext.T課程班級科目s'  is null.");
            }
            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(id);
            if (t課程班級科目 != null)
            {
                _context.T課程班級科目s.Remove(t課程班級科目);
            }

            await _context.SaveChangesAsync();
            return Redirect("/course_management/T%E8%AA%B2%E7%A8%8B%E7%8F%AD%E7%B4%9A%E7%A7%91%E7%9B%AE/classCourseIndex/"+ t課程班級科目.班級id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.T課程班級科目s == null)
            {
                return NotFound();
            }

            var t課程班級科目 = await _context.T課程班級科目s
                .Include(t => t.班級)
                .Include(t => t.科目)
                .Include(t => t.老師)
                .FirstOrDefaultAsync(m => m.班級科目id == id);
            if (t課程班級科目 == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", t課程班級科目);
        }


        // GET: course_management/T課程班級科目/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.T課程班級科目s == null)
            {
                return NotFound();
            }

            var t課程班級科目 = await _context.T課程班級科目s.FindAsync(id);
            if (t課程班級科目 == null)
            {
                return NotFound();
            }
            var 科目名稱 = 
                await _context.T課程科目s
                .Where(s => s.科目id == t課程班級科目.科目id)
                .Select(s => s.科目名稱)
                .FirstOrDefaultAsync();

            ViewData["科目名稱"] = 科目名稱;
            ViewData["老師id"] = new SelectList(_context.T會員老師s, "老師id", "姓名", t課程班級科目.老師id);

            return PartialView("_EditPartial", t課程班級科目);
        }

        // POST: course_management/T課程班級科目/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("班級科目id,班級id,科目id,老師id,狀態")] T課程班級科目 t課程班級科目)
        {
            if (id != t課程班級科目.班級科目id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t課程班級科目);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T課程班級科目Exists(t課程班級科目.班級科目id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
      
            return Redirect("/course_management/T%E8%AA%B2%E7%A8%8B%E7%8F%AD%E7%B4%9A%E7%A7%91%E7%9B%AE/classCourseIndex/" + t課程班級科目.班級id);
        }

        [HttpPost]
        public IActionResult sendMail(string email)
        {
            var htmlMessage = MailClassHtml.htmlMessage();
            _emailSender.SendEmailAsync(email, "通知開課", htmlMessage);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> notification(int receiveId, int sendId, int classCourseId)
        {
            var 班級科目 =
                _context.T課程班級科目s
                .Include(t => t.班級)
                .Include(t => t.科目)
                .Where(s => classCourseId == s.班級科目id)
                .FirstOrDefault();

            var 發送訊息內容 = $"已經安排您教授 {班級科目.班級.班級名稱} 班級的 {班級科目.科目.科目名稱} 課程，請查閱您的課程!";

            var t課程訊息 = new T課程通知表
            {
                發送者類型 = "T",
                發送者id = sendId,
                接收者類型 = "T",
                發送訊息內容 = 發送訊息內容,
                接收者id = receiveId,
                時間 = DateTime.Now,
                狀態 = 1
            };
            _context.Add(t課程訊息);
            await _context.SaveChangesAsync();

            return Ok();

        }
        private bool T課程班級科目Exists(int id)
        {
          return (_context.T課程班級科目s?.Any(e => e.班級科目id == id)).GetValueOrDefault();
        }
    }
}
