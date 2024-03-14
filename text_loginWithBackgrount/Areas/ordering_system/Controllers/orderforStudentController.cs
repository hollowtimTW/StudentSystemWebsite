using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.ordering_system.Models;
using text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO;

namespace text_loginWithBackgrount.Areas.ordering_system.Controllers
{
    /// <summary>
    /// 學生登入後會看到的顯示頁面
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    [Area("ordering_system")]
    public class orderforStudentController : Controller
    {
        private readonly studentContext _myDBContext;
        public orderforStudentController(studentContext myDBContext) {
            _myDBContext = myDBContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Studentrestaurant() 
        {
            ViewData["Title"] = "學生餐廳";
            return View();
        }
        public IActionResult StoreMeanu(int id) 
        {
            ViewData["Title"] = "店家點餐";
            ViewData["storeID"] = id;
            VMstore vMstore = text(id);
            var user = HttpContext.User.Claims.ToList();
            var userID = user.Where(a => a.Type == "StudentId").First().Value;
            var studentID = Convert.ToInt32(userID);
            ViewData["ordered"] = chickstoreOrder(studentID, id);
            return View(vMstore);
        }
        public  VMstore text(int id)
        {
            var storemeal = _myDBContext.T訂餐餐點資訊表s.Where(a => a.店家id == id && a.上架.Trim() == "1").ToList();
            var timeOpen = _myDBContext.T訂餐營業時間表s.Where(a => a.店家id == id && a.顯示.Trim() == "1").ToList();
            var taglist = _myDBContext.T訂餐店家資料表s
                .Where(tagItem => tagItem.店家id == id)
                .SelectMany(tagItem => _myDBContext.T訂餐店家風味表s
                    .Where(tagE => tagE.店家id == tagItem.店家id)
                    .Select(tagE => tagE.口味id))
                .ToList();

            var fridentStore = _myDBContext.T訂餐店家資料表s
                .Where(item => taglist.Contains(item.店家id))
                .Select(item => new CminStore
                {
                    店家ID = item.店家id,
                    店家名稱 = item.店家名稱
                })
                .ToList();


            var store = _myDBContext.T訂餐店家資料表s.Where(a => a.店家id == id).Select(b => new VMstore
            {
                店家id = b.店家id,
                店家名稱 = b.店家名稱,
                地址 = b.地址,
                餐廳照片 = b.餐廳照片 ?? "/images/user.jpg",
                餐點列表 = storemeal,
                營業時間表 = timeOpen,
                相關店家 = fridentStore
            }).FirstOrDefault();
            return store;
        }
        /// <summary>
        /// 檢查該學員於這家店中的餐點與數量
        /// </summary>
        /// <param name="id">學員id</param>
        /// <param name="storeID">店家id</param>
        /// <returns></returns>
        public List<VMstoreStudentOrder> chickstoreOrder(int id, int storeID)
        {
            var result = _myDBContext.T訂餐購物車s.Where(a => a.學員id == id && a.店家id == storeID).Select(a => new VMstoreStudentOrder { 餐點ID = a.餐點id, 數量 = a.數量 }).ToList();
            return result;
        }
    }
}
