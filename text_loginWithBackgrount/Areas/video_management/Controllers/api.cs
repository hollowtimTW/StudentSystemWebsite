using Class_system_Backstage_pj.Areas.video_management.ViewModel;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace Class_system_Backstage_pj.Areas.video_management.Controllers
{
    //https://localhost:7295/api/subjectInformation
    public class api : Controller
    {
        private readonly studentContext _myDBContext;
        private readonly IWebHostEnvironment _env;
        public api(studentContext myDBContext, IWebHostEnvironment webHostEnvironment) 
        {
            _myDBContext = myDBContext;
            _env = webHostEnvironment;
        }
        public IActionResult subjectInformation() 
        {
            var result = _myDBContext.T課程科目s.Select(a => new 
            {
                ID=a.科目id,
                title=a.科目名稱
            });
            return Json(result);
        }
        public IActionResult genreInformation() 
        {
            var result = _myDBContext.T影片Genres.Select(a => new
            {
                ID = a.Id,
                title = a.GenreName
            });
            return Json(result);
        }
        [HttpPost]
        public IActionResult createvideo(videocreatViewModel videocreatViewModel, IFormFile videoUrl)
        {
            if (videoUrl.Length > 0)
            {
                var filesName = videoUrl.FileName;
                var rootDirectory = _env.ContentRootPath;
                var uploadDirectory = rootDirectory + "\\wwwroot\\"; //指定位子
                var filescombine = Path.Combine(uploadDirectory, filesName);  //存進資料庫的影片位置
                using (var system = System.IO.File.Create(filescombine)) //**補充
                {
                    videoUrl.CopyTo(system);
                }
               
                    var memberEntity = new T影片Video
                    {
                        FVideoTitle = videocreatViewModel.videoTitle,
                        科目id = Convert.ToInt32(videocreatViewModel.subjectid),
                        FUrl = filescombine,
                        FPrice = Convert.ToDecimal(videocreatViewModel.videoPrice),
						FVideoName = filesName,
                        GenreId = Convert.ToInt32(videocreatViewModel.GenreId)
                    };
                _myDBContext.T影片Videos.Add(memberEntity);
                _myDBContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
    }
}
