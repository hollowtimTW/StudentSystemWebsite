using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using text_loginWithBackgrount.Areas.job_vacancy.DTO;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    public class jobAPIController : Controller
    {
        private readonly studentContext _studentContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public jobAPIController(studentContext studentContext, IWebHostEnvironment hostingEnvironment)
        {
            _studentContext = studentContext;
            _hostingEnvironment = hostingEnvironment;

            // 設置 QuestPDF 的授權類型為社區版
            QuestPDF.Settings.License = LicenseType.Community;

        }

        // GET: job_vacancy/jobapi/ExportToPDF/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{resumeID}")]
        public async Task<IActionResult> ExportToPDF(int resumeID)
        {
            try
            {

                var thisResume = await _studentContext.T工作履歷資料s.FindAsync(resumeID);
                if (thisResume == null)
                {
                    return Json(new { success = false, message = "找不到指定的履歷" });
                }

                var theStudent = await _studentContext.T會員學生s.FindAsync(thisResume.F學員Id);
                if (theStudent == null)
                {
                    return Json(new { success = false, message = "找不到指定的學生" });
                }

                List<T工作工作經驗> thisResumeWorkExp = null;
                if (thisResume.F有無工作經驗 == "Y")
                {
                    thisResumeWorkExp = await (
                        from workExp in _studentContext.T工作工作經驗s
                        join resumeWorkExp in _studentContext.T工作履歷表工作經驗s
                            on workExp.FId equals resumeWorkExp.F工作經驗Id
                        where resumeWorkExp.F履歷Id == resumeID
                        select workExp
                    ).ToListAsync();
                }

                var resumeData = new ResumeDataDTO
                {
                    Resume = thisResume,
                    Student = theStudent,
                    WorkExperience = thisResumeWorkExp
                };

                QuestPDF.Settings.EnableDebugging = true;

                var document = Document.Create(container =>
                {

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x
                                .FontSize(12) // 設定預設字體大小
                                .FontFamily("源石黑體")); // 設定預設字體

                        // 獲取LOGO圖片的相對路徑
                        string logoFilePath = Path.Combine("images", "logo.jpg");
                        // 組合圖片的完整路徑
                        string logoFullPath = Path.Combine(_hostingEnvironment.WebRootPath, logoFilePath);

                        page.Header().Background("#008374")
                                     .Height(35);
                        page.Footer().Background("#008374").Height(35);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .PaddingHorizontal(2, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Item()
                                    .Height(30)
                                    .Width(100)
                                    .Image(logoFullPath);

                                x.Spacing(20);
                                x.Item()
                                    .Width(1, Unit.Inch)
                                    .Image(resumeData?.Student?.圖片)
                                    .WithCompressionQuality(ImageCompressionQuality.Medium);

                                x.Item().Text($"{resumeData?.Student?.姓名} {resumeData?.Student?.性別}");
                                x.Item().Text(resumeData?.Student?.生日.ToString());
                                x.Item().Text($"連絡電話　{resumeData?.Student?.手機}");
                                x.Item().Text($"聯絡信箱　{resumeData?.Student?.信箱}");

                                x.Item().Text($"學歷　{resumeData?.Student?.學校} {resumeData?.Student?.學位}  {resumeData?.Student?.科系}  {resumeData?.Student?.畢肄}");
                                x.Item().Text("專長技能");
                                x.Item().Text(resumeData?.Resume?.F專長技能);
                                x.Item().Text("語文能力");
                                x.Item().Text(resumeData?.Resume?.F語文能力);

                                if (thisResumeWorkExp != null && thisResumeWorkExp.Count > 0)
                                {
                                    x.Item().Text("工作經驗");
                                    foreach (var data in thisResumeWorkExp)
                                    {
                                        x.Item().Text($"{data.F公司名稱} {data.F職務名稱}");
                                        x.Item().Text($"{data.F起始年月} ~ {data.F結束年月}");
                                        x.Item().Text($"{data.F薪水待遇}");
                                        x.Item().Text($"工作內容");
                                        x.Item().Text($"{data.F工作內容}");
                                        x.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                                    }
                                }
                                else
                                {
                                    x.Item().Text($"工作經驗　無");
                                }

                                x.Item().Text($"希望職稱　{resumeData?.Resume?.F希望職稱}");
                                x.Item().Text($"工作性質　{resumeData?.Resume?.F工作性質}");
                                x.Item().Text($"工作時段　{resumeData?.Resume?.F工作時段}");
                                x.Item().Text($"配合輪班　{resumeData?.Resume?.F配合輪班}");
                                x.Item().Text($"希望薪水待遇　{resumeData?.Resume?.F希望薪水待遇}");
                                x.Item().Text($"希望工作地點　{resumeData?.Resume?.F希望工作地點}");

                                x.Item().Text("自傳");
                                x.Item().Text(resumeData?.Resume?.F自傳);
                            });
                    });
                });

                document.ShowInPreviewer();

                // 將 PDF 內容保存到記憶體中
                MemoryStream memoryStream = new MemoryStream();
                document.GeneratePdf(memoryStream);
                memoryStream.Position = 0;

                // 確保資料夾存在
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "Rasengan學生服務系統");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 構建文件保存路徑
                string baseFileName = $"{thisResume.F履歷名稱}_{DateTime.Today.ToString("yyyyMMdd")}";
                string fileName = $"{baseFileName}.pdf";
                string filePath = Path.Combine(folderPath, fileName);

                //確認檔名的唯一性
                //int count = 1;
                //while (System.IO.File.Exists(filePath))
                //{
                //    fileName = $"{baseFileName}({count}).pdf";
                //    filePath = Path.Combine(folderPath, fileName);
                //    count++;
                //}

                // 將 PDF 內容保存到文件中
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    memoryStream.WriteTo(fileStream);
                }

                return Json(new { success = true, message = "履歷檔案位於 桌面/Rasengan學生服務系統 資料夾中。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: job_vacancy/jobapi/GetResumeTitles/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetMyResumes(int studentID)
        {
            var resumeData = await _studentContext.T工作履歷資料s
                            .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                            .OrderByDescending(r => r.F最後更新時間)
                            .Select(r => new { r.FId, r.F履歷名稱, r.F最後更新時間, r.F希望職稱, r.F工作性質 })
                            .ToListAsync();

            var viewModelList = new List<MyResumesViewModel>();

            foreach (var data in resumeData)
            {
                var viewModel = new MyResumesViewModel
                {
                    StudentID = studentID,
                    ResumeID = data.FId,
                    ResumeTitle = data.F履歷名稱,
                    HopeJobTitle = data.F希望職稱,
                    WorkType = data.F工作性質,
                    LastUpdate = data.F最後更新時間
                };
                viewModelList.Add(viewModel);
            }

            return PartialView("_MyResumesPartial", viewModelList);
        }

        /// <summary>
        /// 回傳應徵紀錄的視圖
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        // GET: job_vacancy/jobapi/GetMyApplyRecords/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetMyApplyRecords(int studentID)
        {

            var recordData = await _studentContext.T工作應徵工作紀錄s
                        .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                        .Include(r => r.F職缺)
                        .ThenInclude(j => j.F公司)
                        .OrderByDescending(r => r.F應徵時間)
                        .ToListAsync();


            var viewModelList = new List<MyApplyRecordsViewModel>();

            foreach (var data in recordData)
            {
                var viewModel = new MyApplyRecordsViewModel
                {
                    ApplyRecordID = data.FId,
                    JobID = data.F職缺Id,
                    JobTitle = data.F職缺.F職務名稱,
                    LetterContent = data.F應徵信內容,
                    ApplyTime = data.F應徵時間,

                    CompanyID = data.F職缺.F公司Id,
                    CompanyName = data.F職缺.F公司.F公司名稱,
                    Salary = !string.IsNullOrEmpty(data.F職缺.F薪水待遇) ? data.F職缺.F薪水待遇 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(data.F職缺.F工作性質) ? data.F職缺.F工作性質 : "暫不提供",
                    JobLocation = !string.IsNullOrEmpty(data.F職缺.F工作地點) ? data.F職缺.F工作地點 : "暫不提供",
                    UpdateTime = data.F職缺.F最後更新時間
                };
                viewModelList.Add(viewModel);
            }

            return PartialView("_MyApplyRecordsPartial", viewModelList);
        }

        // GET: job_vacancy/jobapi/GetMyFavoritesJobs/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetMyFavoritesJobs(int studentID)
        {

            var jobData = await _studentContext.T工作儲存工作紀錄s
                          .Where(r => r.F學員Id == studentID)
                          .Include(r => r.F職缺)
                          .ThenInclude(j => j.F公司)
                          .OrderByDescending(r => r.F儲存時間)
                          .ToListAsync();

            var viewModelList = new List<MyFavoritesJobsViewModel>();

            foreach (var data in jobData)
            {
                var viewModel = new MyFavoritesJobsViewModel
                {
                    FavoriteID = data.FId,
                    JobID = data.F職缺Id,
                    JobTitle = data.F職缺.F職務名稱,
                    AddTime = data.F儲存時間,

                    CompanyID = data.F職缺.F公司Id,
                    CompanyName = data.F職缺.F公司.F公司名稱,
                    Salary = !string.IsNullOrEmpty(data.F職缺.F薪水待遇) ? data.F職缺.F薪水待遇 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(data.F職缺.F工作性質) ? data.F職缺.F工作性質 : "暫不提供",
                    JobLocation = !string.IsNullOrEmpty(data.F職缺.F工作地點) ? data.F職缺.F工作地點 : "暫不提供",
                    UpdateTime = data.F職缺.F最後更新時間
                };
                viewModelList.Add(viewModel);
            }

            return PartialView("_MyFavoritesJobsPartial", viewModelList);
        }

        // GET: job_vacancy/jobapi/GetMyRecommendedJobs/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetMyRecommendedJobs(int studentID)
        {

            var jobData = await _studentContext.T工作推薦職缺s
                          .Where(r => r.F學員Id == studentID)
                          .Include(r => r.F職缺)
                          .ThenInclude(j => j.F公司)
                          .ToListAsync();

            var viewModelList = new List<MyRecommendedJobsViewModel>();

            foreach (var data in jobData)
            {
                var viewModel = new MyRecommendedJobsViewModel
                {
                    RecommendedID = data.FId,
                    JobID = data.F職缺Id,
                    JobTitle = data.F職缺.F職務名稱,
                    Score = data.F推薦程度,

                    CompanyID = data.F職缺.F公司Id,
                    CompanyName = data.F職缺.F公司.F公司名稱,
                    Salary = !string.IsNullOrEmpty(data.F職缺.F薪水待遇) ? data.F職缺.F薪水待遇 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(data.F職缺.F工作性質) ? data.F職缺.F工作性質 : "暫不提供",
                    JobLocation = !string.IsNullOrEmpty(data.F職缺.F工作地點) ? data.F職缺.F工作地點 : "暫不提供",
                    UpdateTime = data.F職缺.F最後更新時間
                };
                viewModelList.Add(viewModel);
            }

            return PartialView("_MyRecommendedJobsPartial", viewModelList);
        }


        [Route("/job_vacancy/jobapi/{Action=Index}/{tabName}")]
        public async Task<IActionResult> GetJobsByTabName(string? tabName, int page = 1)
        {
            int pageSize = 8; // 每頁顯示的資料量

            IQueryable<JobListDTO> queryData = _studentContext.T工作職缺資料s
                .Select(job => new JobListDTO
                {
                    JobID = job.FId,
                    JobTitle = job.F職務名稱,
                    CompanyID = job.F公司Id,
                    CompanyName = job.F公司.F公司名稱,
                    JobLocation = !string.IsNullOrEmpty(job.F工作地點) ? job.F工作地點 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(job.F工作性質) ? job.F工作性質 : "暫不提供",
                    Salary = !string.IsNullOrEmpty(job.F薪水待遇) ? job.F薪水待遇 : "暫不提供",
                    UpdateTime = job.F最後更新時間,
                    RequiredPeople = !string.IsNullOrEmpty(job.F需求人數) ? job.F需求人數 : "暫不提供"
                });

            // 根據 tab 參數選擇性地篩選資料
            if (!string.IsNullOrEmpty(tabName))
            {
                switch (tabName)
                {
                    case "fulltime":
                        queryData = queryData.Where(job => job.JobType == "全職");
                        break;
                    case "parttime":
                        queryData = queryData.Where(job => job.JobType == "兼職");
                        break;
                    case "dispatch":
                        queryData = queryData.Where(job => job.JobType == "派遣");
                        break;
                    default:
                        break;
                }
            }

            // 計算當前頁面的資料偏移量
            int skipAmount = (page - 1) * pageSize;

            // 只返回指定頁數的資料
            var jobs = await queryData.OrderByDescending(job => job.UpdateTime)
                                      .Skip(skipAmount)
                                      .Take(pageSize)
                                      .ToListAsync();

            return Ok(jobs);
        }


        [HttpPost]
        public IActionResult CompanyJoblist([FromBody] SearchJobDTO searchJobDTO)
        {

            var thisCompanyJobs = _studentContext.T工作職缺資料s.Where(j => j.F公司Id == searchJobDTO.CompanyID && j.F刪除狀態 == "0");

            //如果無資料... 待補
            if (thisCompanyJobs == null)
            {
                return Json("無資料");
            }

            //整理出工作地點所在的行政區（不重複）
            var uniqueLocations = thisCompanyJobs
            .Where(j => !string.IsNullOrWhiteSpace(j.F工作地點))
            .Select(j => j.F工作地點.Substring(0, Math.Min(6, j.F工作地點.Length)))
            .Distinct()
            .ToList();

            //根據工作地點讀取資料
            //var location = _search.Location ;

            //根據關鍵字搜尋職務名稱
            if (!string.IsNullOrEmpty(searchJobDTO.Keyword))
            {
                thisCompanyJobs = thisCompanyJobs
                                  .Where(s => s.F職務名稱.Contains(searchJobDTO.Keyword));
            }

            //排序
            switch (searchJobDTO.SortBy)
            {
                case "jobTitle":
                    thisCompanyJobs = searchJobDTO.SortType == "asc" ?
                                      thisCompanyJobs.OrderBy(s => s.F職務名稱) :
                                      thisCompanyJobs.OrderByDescending(s => s.F職務名稱);
                    break;
                case "jobType":
                    thisCompanyJobs = searchJobDTO.SortType == "asc" ?
                                      thisCompanyJobs.OrderBy(s => s.F工作性質) :
                                      thisCompanyJobs.OrderByDescending(s => s.F工作性質);
                    break;
                case "jobLocation":
                    thisCompanyJobs = searchJobDTO.SortType == "asc" ?
                                      thisCompanyJobs.OrderBy(s => s.F工作地點) :
                                      thisCompanyJobs.OrderByDescending(s => s.F工作地點);
                    break;
                default:
                    thisCompanyJobs = searchJobDTO.SortType == "asc" ?
                                      thisCompanyJobs.OrderBy(s => s.F最後更新時間) :
                                      thisCompanyJobs.OrderByDescending(s => s.F最後更新時間);
                    break;
            }

            //分頁
            int totalCount = thisCompanyJobs.Count();
            int pageSize = searchJobDTO.PageSize ?? 5;  //預設為一頁5筆
            int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int page = searchJobDTO.Page ?? 1;  //預設為第1頁
            thisCompanyJobs = thisCompanyJobs.Skip((int)(page - 1) * pageSize).Take(pageSize);


            //回傳資料
            CompanyJobPagingDTO thisCompanyJobsPaging = new CompanyJobPagingDTO();
            thisCompanyJobsPaging.TotalPages = totalPage;
            thisCompanyJobsPaging.CompanyJobsResult = thisCompanyJobs.ToList();

            return Json(thisCompanyJobsPaging);
        }



        

    }
}
