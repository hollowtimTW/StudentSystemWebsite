using Microsoft.AspNetCore.Mvc;

namespace TemplateResearch.Controllers
{
    public class TemplateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About() { return View(); }
        public IActionResult Clients() { return View(); }
        public IActionResult StatsCounter() { return View(); }
        public IActionResult CallToAction() { return View(); }
        public IActionResult Services() { return View(); }
        public IActionResult Testimonials() { return View(); }
        public IActionResult Portfolio() { return View(); }
        public IActionResult PortfolioDetails() { return View(); }
        public IActionResult Team() { return View(); }
        public IActionResult Pricing() { return View(); }
        public IActionResult Faq() { return View(); }
        public IActionResult RecentPosts() { return View(); }
        public IActionResult BlogDetails() { return View(); }
        public IActionResult Blog() { return View(); }
        public IActionResult Contact() { return View(); }
        public IActionResult Blank() { return View(); }
    }
}
