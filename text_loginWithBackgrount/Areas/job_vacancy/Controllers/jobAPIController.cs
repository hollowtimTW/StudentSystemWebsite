using Class_system_Backstage_pj.Areas.job_vacancy.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using text_loginWithBackgrount.Areas.job_vacancy.DTO;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    public class jobAPIController : Controller
    {
        private readonly studentContext _studentContext;

        public jobAPIController(studentContext studentContext)
        {
            _studentContext = studentContext;

            // 設置 QuestPDF 的授權類型為社區版
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // GET: job_vacancy/jobapi/ExportToPDF
        [Route("/job_vacancy/jobapi/{Action=Index}")]
        public IActionResult ExportToPDF()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x
                            .FontSize(20) // 設定預設字體大小
                            .FontFamily("微軟正黑體")); // 設定預設字體

                    page.Header()
                        .Text("Hello PDF!")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text("文字文字");
                            x.Item().Image(Placeholders.Image(200, 100)); // 自動產生柔和漸變圖片
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("第");
                            x.CurrentPageNumber();
                            x.Span("頁");
                        });
                });
            });

            // 將 PDF 內容保存到記憶體中
            MemoryStream memoryStream = new MemoryStream();
            document.GeneratePdf(memoryStream);
            memoryStream.Position = 0;

            // 確保 exportfile 子資料夾存在
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exportfile");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 構建文件保存路徑
            string fileName = "test.pdf";
            string filePath = Path.Combine(folderPath, fileName);

            // 將 PDF 內容保存到文件中
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                memoryStream.WriteTo(fileStream);
            }

            // 返回文件作為下載連結
            return File(memoryStream.ToArray(), "application/pdf", fileName);
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
