using Class_system_Backstage_pj.Areas.video_management.Controllers;
using Class_system_Backstage_pj.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.video_management.DTO;
using text_loginWithBackgrount.Areas.video_management.Repositories;


namespace text_loginWithBackgrount.Areas.video_management.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("video_management")]
    public class videoforStudentController : Controller
    {
        private readonly ILogger<videoforStudentController> _logger;
        private readonly studentContext _studentContext;
        private readonly IHomeRepository _homeRepository;
        public videoforStudentController(ILogger<videoforStudentController> logger, studentContext studentContext, IHomeRepository homeRepository)
        {
            _logger = logger;
            _studentContext = studentContext;
            _homeRepository = homeRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> videoshopping (string sTerm = "", int genreId = 0) 
        {
            IEnumerable<T影片Video> videos = await _homeRepository.GetVideos(sTerm, genreId);
            IEnumerable<T影片Genre> genres = await _homeRepository.Genres();

            VideoDisPlay videoModel = new VideoDisPlay
            {
                Videos = videos,
                Genres = genres,
                STerm = sTerm,
                GenreId = genreId
            };

            return View(videoModel);

        }
        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0)
        {
            
            
                IEnumerable<T影片Video> videos = await _homeRepository.GetVideos(sTerm, genreId);
                IEnumerable<T影片Genre> genres = await _homeRepository.Genres();

                VideoDisPlay videoModel = new VideoDisPlay
                {
                    Videos = videos,
                    Genres = genres,
                    STerm = sTerm,
                    GenreId = genreId
                };

                return View(videoModel);
            
       
        }


    }
}
