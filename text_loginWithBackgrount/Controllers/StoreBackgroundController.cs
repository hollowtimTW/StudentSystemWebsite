using Class_system_Backstage_pj.Areas.ordering_system.Models;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.ordering_system.Controllers;

namespace text_loginWithBackgrount.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "store")]
    public class StoreBackgroundController : Controller
    {
        private readonly studentContext _myDBContext;
        private readonly IEmailSender _emailSender;
        public StoreBackgroundController(studentContext myDBContext, IEmailSender emailSender) 
        {
            _myDBContext=myDBContext;
            _emailSender=emailSender;
        }
        /// <summary>
        /// 店家個人資料首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var user = HttpContext.User.Claims.ToList();
            var userName = user.Where(a => a.Type == "teacherID").First().Value;//找登入的使用ID

            return View(storeDeatail(Convert.ToInt32(userName)));
        }
        /// <summary>
        /// 餐點管理頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Menumanagement()
        {
            ViewBag.type = "Menumanagement";
            return View();
        }
        /// <summary>
        /// 訂單管理頁
        /// </summary>
        /// <returns></returns>
        public IActionResult ordermanger()
        {
            ViewBag.type = "ordermanger";
            return View();
        }
        /// <summary>
        /// 財務分析頁
        /// </summary>
        /// <returns></returns>
        public IActionResult moneymanger()
        {
            ViewBag.type = "moneymanger";
            return View();
        }
        /// <summary>
        /// 透過店家ID查詢回傳需要的VMstoreInformation
        /// </summary>
        /// <param name="id">店家PK</param>
        /// <returns></returns>
        public VMstoreInformation storeDeatail(int id) 
        {
            var result = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.店家id == id).Count();
            var result1 = Convert.ToInt32(_myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.店家id == id).Sum(a => a.金額小記));
            var result3 = (from item in _myDBContext.T訂餐評論表s
                           join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                           join b in _myDBContext.T訂餐訂單詳細資訊表s on a.訂單id equals b.訂單id
                           join c in _myDBContext.T訂餐店家資料表s on b.店家id equals c.店家id
                           where c.店家id == id
                           select item).Distinct();
            var storedata = result3.GroupBy(a => a.滿意度星數).Select(b =>
            new
            {
                滿意度星數 = Convert.ToInt32(b.Key),
                評論數量 = b.Count(),
                加權 = Convert.ToInt32(b.Key) * b.Count()
            });
            int totalComments = storedata.Sum(item => item.評論數量);
            int totalWeight = storedata.Sum(item => item.加權);
            double evaluate = totalWeight / totalComments;
            VMstoreInformation storeInformationVM = new VMstoreInformation()
            {
                turnover = result1,
                historyorder = result,
                evaluate = evaluate.ToString("0.0"),
                commentsNum = totalComments
            };
            return storeInformationVM;
        }
    }
}
