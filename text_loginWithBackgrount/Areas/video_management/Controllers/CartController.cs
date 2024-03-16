using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.video_management.Repositories;

namespace text_loginWithBackgrount.Areas.video_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("video_management")]
    //[Route("video_management/Cart/[action]")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;/// <summary>
        private readonly studentContext _studentContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(studentContext studentContext, IHttpContextAccessor httpContextAccessor, ICartRepository cartRepo)
        {
            _studentContext = studentContext;
            _httpContextAccessor = httpContextAccessor;
            _cartRepo = cartRepo;
        }                           /// 學生登入後會看到的顯示頁面

     
        //[HttpGet("{FVideoId}")]
        public async Task<IActionResult> AddItem(int FVideoId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(FVideoId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        //[HttpGet("{FVideoId}")]
        public async Task<IActionResult> RemoveItem(int FVideoId)
        {
            var cartCount = await _cartRepo.RemoveItem(FVideoId);
            return RedirectToAction("GetUserCart");
        }
        //public async Task<IActionResult> RemoveItem(int bookId, int qty = 1, int redirect = 0)
        //{
        //    var cartCount = await _cartRepo.RemoveItem(bookId, qty);
        //    if (redirect == 0)
        //        return Ok(cartCount);
        //    return RedirectToAction("GetUserCart");
        //}
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int userId = GetUserId();
            int cartItem = await _cartRepo.GetCartItemCount(userId);
            return Ok(cartItem);
        }
        private int GetUserId()
        {
            string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "StudentId")?.Value;
            int studentId;
            int.TryParse(userId, out studentId);
            return studentId;
        }
        public async Task<IActionResult> Checkout()
        {
            bool isCheckedOut = await _cartRepo.DoCheckout();
            if (!isCheckedOut)
                throw new Exception("Something happen in server side");
            return RedirectToAction("videoshopping", "videoforStudent");
        }


    }

} 

