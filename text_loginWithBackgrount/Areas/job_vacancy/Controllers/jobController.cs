using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    public class jobController : Controller
    {
        private readonly studentContext _context;

        public jobController(studentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        // GET: job_vacancy/job/CompanyDetails/5
        [Route("/job_vacancy/job/{Action=Index}/{companyID}")]
        public async Task<IActionResult> CompanyDetails(int companyID)
        {

            try
            {
                var thisCompany = await _context.T工作公司資料s.FindAsync(companyID);

                if (thisCompany == null)
                {
                    return NotFound("無這筆資料");
                }

                var viewModel = new CompanyDetailViewModel
                {

                    taxID = thisCompany.F統一編號,
                    CompanyName = thisCompany.F公司名稱,
                    ContactPerson = thisCompany.F聯絡人,
                    ContactPhone = thisCompany.F聯絡人電話,
                    ContactEmail = thisCompany.F聯絡人Email,
                    Principal = thisCompany.F負責人,
                    CompanyPhone = thisCompany.F公司電話,
                    CompanyAddress = thisCompany.F公司地址,
                    CompanyProfile = thisCompany.F公司簡介
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
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
