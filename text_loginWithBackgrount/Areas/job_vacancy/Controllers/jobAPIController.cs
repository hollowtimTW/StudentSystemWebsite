using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        }

        // GET: job_vacancy/jobapi/GetResumeTitles/5
        [Route("/job_vacancy/jobapi/{Action=Index}/{studentID}")]
        public async Task<IActionResult> GetResumeTitles(int studentID)
        {
            var resumeTitles = await _studentContext.T工作履歷資料s
                              .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                              .OrderByDescending(r => r.F最後更新時間)
                              .Select(r => r.F履歷名稱)
                              .ToListAsync();

            return PartialView("_ApplyModalPartial", resumeTitles);
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
                    //UpdateTime = job.F最後更新時間.HasValue ? job.F最後更新時間.Value : DateTime.MinValue,
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
