using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Area("class_discuss")]
    public class discussController : Controller
    {
        private readonly studentContext _DBContext;

        public discussController(studentContext myDBContext)
        {
            _DBContext = myDBContext;
        }
        /// <summary>
        /// 訪客能看到的基本分頁
        /// </summary>
        public IActionResult Index()//選看板
        {
            return View();
        }

        public IActionResult Articles(int subid)//文章列表
        {
            T討論看板? ID = _DBContext.T討論看板s.FirstOrDefault(a => a.看板id == subid);
            ViewBag.subName = ID.名稱;
            ViewBag.subId = subid;
            return View();
        }

        public IActionResult ArticleDetails(int subid,int artid)//選擇文章 顯示內容
        {
            T討論看板? ID = _DBContext.T討論看板s.FirstOrDefault(a => a.看板id == subid);
            T討論文章? art = _DBContext.T討論文章s.FirstOrDefault(a => a.文章id == artid);
            int mes = _DBContext.T討論留言s.Where(a => a.文章id == artid).Count();
            ViewBag.subName = ID.名稱;
            ViewBag.subId = subid;
            ViewBag.typeId = art.子版id;
            ViewBag.artName = art.標題;
            ViewBag.artId = artid;
            ViewBag.writerId = art.學生id;
            ViewBag.userId = GetUserId();
            ViewBag.mesCount = mes;
            return View();
        }

        public IActionResult ArticleCreate(int subid)//新增文章畫面
        {
            T討論看板? ID = _DBContext.T討論看板s.FirstOrDefault(a => a.看板id == subid);
            ViewBag.subName = ID.名稱;
            ViewBag.subId = subid;

            var sub = _DBContext.T討論子版s.Where(s => s.看板id == subid).ToList();
            ViewData["子版id"] = new SelectList(sub, "子版id", "名稱");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleCreate(int subid, T討論文章 article)//新增文章功能
        {
            if (ModelState.IsValid)
            {
                article.看板id = subid;
                article.時間 = DateTime.Now.ToShortDateString().ToString();
                article.學生id = GetUserId();
                _DBContext.Add(article);
                await _DBContext.SaveChangesAsync();
                return RedirectToAction("Articles", new { subid = subid });
            }
            var sub = _DBContext.T討論子版s.Where(s => s.看板id == subid).ToList();
            ViewData["子版id"] = new SelectList(sub, "子版id", "名稱");
            return View(article);
        }

        public async Task<IActionResult> ArticleEdit(int? subid ,int? artid)//修改文章畫面
        {
            if (artid == null || _DBContext.T討論文章s == null)
            {
                return NotFound();
            }

            var article = await _DBContext.T討論文章s.FindAsync(artid);
            if (article == null)
            {
                return NotFound();
            }

            T討論看板? ID = _DBContext.T討論看板s.FirstOrDefault(a => a.看板id == subid);
            ViewBag.subName = ID.名稱;
            ViewBag.subId = subid;
            ViewBag.artId = artid;

            var sub = _DBContext.T討論子版s.Where(s => s.看板id == subid).ToList();
            ViewData["子版id"] = new SelectList(sub, "子版id", "名稱", article.子版id);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleEdit(int subid, T討論文章 article)//修改文章功能
        {
            if (ModelState.IsValid)
            {
                article.看板id = subid;
                article.時間 = DateTime.Now.ToShortDateString().ToString();
                article.學生id = GetUserId();
                _DBContext.Update(article);
                await _DBContext.SaveChangesAsync();
                return RedirectToAction("Articles", new { subid = subid });
            }
            var sub = _DBContext.T討論子版s.Where(s => s.看板id == subid).ToList();
            ViewData["子版id"] = new SelectList(sub, "子版id", "名稱", article.子版id);
            return View(article);
        }

        [HttpPost]
        public IActionResult ArticleDelete(int subid,int artid)//軟刪除文章功能
        {
            var article = _DBContext.T討論文章s.Find(artid);
            if (article == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                article.時間 = DateTime.Now.ToShortDateString().ToString();
                article.刪除 = "1";
                _DBContext.Update(article);
                _DBContext.SaveChangesAsync();
                return RedirectToAction("Articles", new { subid = subid });
            }
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> MessageCreate(string mes, int subid , int typeid , int artid)//新增留言功能
        {
            T討論留言 newMessage = new T討論留言();
            if (ModelState.IsValid)
            {
                newMessage.內容 = mes;
                newMessage.時間 = DateTime.Now.ToShortDateString().ToString();
                newMessage.看板id = subid;
                newMessage.子版id = typeid;
                newMessage.文章id = artid;
                newMessage.學生id = GetUserId();
                _DBContext.Add(newMessage);
                await _DBContext.SaveChangesAsync();
                return Ok();
            }
            return Ok();
        }
        public async Task<IActionResult> GetMessage(int mesid)//獲得留言
        {
            if (mesid == null || _DBContext.T討論文章s == null)
            {
                return NotFound();
            }

            var message = await _DBContext.T討論留言s.FindAsync(mesid);
            if (message == null)
            {
                return NotFound();
            }
            string mes = message.內容;
            return Content(message.內容);
        }
        [HttpPost]
        public IActionResult MessageEdit(string mes, int mesid)//修改留言功能
        {
            var message = _DBContext.T討論留言s.Find(mesid);
            if (message == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                message.時間 = DateTime.Now.ToShortDateString().ToString();
                message.內容 = mes;
                _DBContext.Update(message);
                _DBContext.SaveChangesAsync();
                return Ok();
            }
            return View(message);
        }
        [HttpPost]
        public IActionResult MessageDelete(int mesid)//軟刪除留言功能
        {
            var message = _DBContext.T討論留言s.Find(mesid);
            if (message == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                message.刪除 = "1";
                _DBContext.Update(message);
                _DBContext.SaveChangesAsync();
                return Ok();
            }
            return Ok();
        }
        public IActionResult Announcement()
        {
            return View();
        }

        private int GetUserId()
        {
            var studentId = User.FindFirst("StudentId")?.Value;
            int userId;
            int.TryParse(studentId, out userId);
            Console.WriteLine(studentId);
            return userId;
        }

        private bool articleExists(int id)
        {
            return (_DBContext.T討論文章s?.Any(e => e.文章id == id)).GetValueOrDefault();
        }
    }
}
