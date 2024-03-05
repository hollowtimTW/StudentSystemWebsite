using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.Data.Common;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;
using text_loginWithBackgrount.Controllers;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    public class jobController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly studentContext _context;

        public jobController(ILogger<HomeController> logger, studentContext context)
        {
            _logger = logger;
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
                    return NotFound("找不到此公司資料");
                }

                var viewModel = new CompanyDetailViewModel
                {
                    companyID = thisCompany.FId,
                    taxID = thisCompany.F統一編號,
                    CompanyName = thisCompany.F公司名稱,
                    Principal = !string.IsNullOrEmpty(thisCompany.F負責人) ? thisCompany.F負責人 : "暫不提供",
                    CompanyPhone = !string.IsNullOrEmpty(thisCompany.F公司電話) ? thisCompany.F公司電話 : "暫不提供",
                    CompanyAddress = !string.IsNullOrEmpty(thisCompany.F公司地址) ? thisCompany.F公司地址 : "暫不提供",
                    CompanyProfile = !string.IsNullOrEmpty(thisCompany.F公司簡介) ? thisCompany.F公司簡介.Replace("\\n", "\n") : "暫不提供",
                    ContactPerson = !string.IsNullOrEmpty(thisCompany.F聯絡人) ? thisCompany.F聯絡人 : "請洽平台系統管理員",
                    ContactPhone = !string.IsNullOrEmpty(thisCompany.F聯絡人電話) ? thisCompany.F聯絡人電話 : "請洽平台系統管理員",
                    ContactEmail = !string.IsNullOrEmpty(thisCompany.F聯絡人Email) ? thisCompany.F聯絡人Email : "請洽平台系統管理員"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }

        // GET: job_vacancy/job/JobDetails/5
        [Route("/job_vacancy/job/{Action=Index}/{jobID}")]
        public async Task<IActionResult> JobDetails(int jobID)
        {

            try
            {
                var thisJob = await _context.T工作職缺資料s.FindAsync(jobID);

                if (thisJob == null)
                {
                    return NotFound("找不到此職缺資料");
                }

                var thisCompany = await _context.T工作公司資料s.FirstOrDefaultAsync(company => company.FId == thisJob.F公司Id);

                if (thisCompany == null)
                {
                    return NotFound("找不到此職缺的相應公司資料，請聯絡平台系統管理員");
                }

                var viewModel = new JobDetailViewModel
                {

                    JobTitle = thisJob.F職務名稱,
                    UpdateTime = thisJob.F最後更新時間.HasValue ? thisJob.F最後更新時間.Value.ToString("yyyy/MM/dd HH:mm:ss") + "更新" : string.Empty,
                    JobContent = !string.IsNullOrEmpty(thisJob.F工作內容) ? thisJob.F工作內容.Replace("\\n", "\n") : "暫不提供",
                    Salary = !string.IsNullOrEmpty(thisJob.F薪水待遇) ? thisJob.F薪水待遇 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(thisJob.F工作性質) ? thisJob.F工作性質 : "暫不提供",
                    JobLocation = !string.IsNullOrEmpty(thisJob.F工作地點) ? thisJob.F工作地點 : "暫不提供",
                    JobTime = !string.IsNullOrEmpty(thisJob.F工作時段) ? thisJob.F工作時段 : "暫不提供",
                    ShiftRequirement = !string.IsNullOrEmpty(thisJob.F輪班需求) ? (thisJob.F輪班需求 == "Y" ? "需要輪班" : "不需輪班") : "暫不提供",
                    RequiredPeople = !string.IsNullOrEmpty(thisJob.F需求人數) ? thisJob.F需求人數 : "暫不提供",
                    AcademicRequirement = !string.IsNullOrEmpty(thisJob.F學歷要求) ? thisJob.F學歷要求.Replace("\\n", "\n") : "不拘",
                    LanguageCondition = !string.IsNullOrEmpty(thisJob.F語文條件) ? thisJob.F語文條件.Replace("\\n", "\n") : "不拘",
                    WorkAbility = !string.IsNullOrEmpty(thisJob.F工作技能) ? thisJob.F工作技能.Replace("\\n", "\n") : "無特別要求",
                    OtherCondition = !string.IsNullOrEmpty(thisJob.F其他條件) ? thisJob.F其他條件.Replace("\\n", "\n") : "無特別要求",

                    CompanyID = thisCompany.FId,
                    CompanyName = thisCompany.F公司名稱,
                    ContactPerson = !string.IsNullOrEmpty(thisCompany.F聯絡人) ? thisCompany.F聯絡人 : "請洽平台系統管理員",
                    ContactPhone = !string.IsNullOrEmpty(thisCompany.F聯絡人電話) ? thisCompany.F聯絡人電話 : "請洽平台系統管理員",
                    ContactEmail = !string.IsNullOrEmpty(thisCompany.F聯絡人Email) ? thisCompany.F聯絡人Email : "請洽平台系統管理員"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }

        }


        public IActionResult ResumePreview()
        {
            return View();
        }
    }
}
