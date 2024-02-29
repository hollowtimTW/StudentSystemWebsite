using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    public class jobController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CompanyDetails()
        {
            return View();
        }

        public IActionResult JobDetails()
        {
            return View();
        }

        public IActionResult ResumePreview()
        {
            return View();
        }
    }
}
