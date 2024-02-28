using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.ordering_system.Controllers
{
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    [Area("ordering_system")]
    public class orderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
