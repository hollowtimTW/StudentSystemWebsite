using Class_system_Backstage_pj.Areas.video_management.Controllers;
using Class_system_Backstage_pj.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.video_management.DTO;
using text_loginWithBackgrount.Areas.video_management.Repositories;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUserOrderRepository _userOrderRepository;
        private readonly ICartRepository _cartRepo;
        public videoforStudentController(ILogger<videoforStudentController> logger, studentContext studentContext, IHomeRepository homeRepository, IUserOrderRepository userOrderRepository, ICartRepository cartRepo)
        {
            _logger = logger;
            _studentContext = studentContext;
            _homeRepository = homeRepository;
            _userOrderRepository = userOrderRepository;
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _userOrderRepository.UserOrders();
            return View(orders);
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
        public async Task<IActionResult> confirm()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        //public async Task<IActionResult> Index(string sTerm = "", int genreId = 0)
        //{
            
            
        //        IEnumerable<T影片Video> videos = await _homeRepository.GetVideos(sTerm, genreId);
        //        IEnumerable<T影片Genre> genres = await _homeRepository.Genres();

        //        VideoDisPlay videoModel = new VideoDisPlay
        //        {
        //            Videos = videos,
        //            Genres = genres,
        //            STerm = sTerm,
        //            GenreId = genreId
        //        };

        //        return View(videoModel);
            
       
        //}


    }
}
