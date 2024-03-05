using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using text_loginWithBackgrount.Areas.job_vacancy.DTO;

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
