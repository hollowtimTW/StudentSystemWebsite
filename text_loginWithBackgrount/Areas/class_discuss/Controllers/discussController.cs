using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Articles(int board_id)
        {
            T討論看板? ID = _DBContext.T討論看板s.FirstOrDefault(a => a.看板id == board_id);
            ViewBag.Name = ID.名稱;
            ViewBag.Id = board_id;
            return View();
        }

        public IActionResult ArticleDetails()
        {
            return View();
        }

        public IActionResult ArticleEdit()
        {
            return View();
        }

        public IActionResult Announcement()
        {
            return View();
        }
    }
}
