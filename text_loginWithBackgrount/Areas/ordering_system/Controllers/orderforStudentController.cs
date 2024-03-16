using Class_system_Backstage_pj.Areas.ordering_system.Models;
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
        public orderforStudentController(studentContext myDBContext)
        {
            _myDBContext = myDBContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Studentrestaurant()
        {
            ViewData["Title"] = "學生餐廳";
            return View(vMstoreCarIndices());
        }
        public IActionResult CreateOrder()
        {
            ViewData["Title"] = "訂單成立";
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
        public VMstore text(int id)
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
            var result = _myDBContext.T訂餐購物車s.Where(a => a.學員id == id && a.店家id == storeID && a.狀態 == "0").Select(a => new VMstoreStudentOrder { 餐點ID = a.餐點id, 數量 = a.數量 }).ToList();
            return result;
        }
        public List<VMstoreCarIndex> vMstoreCarIndices()
        {
            //將平均評價存入字典中使後續店家資料中看
            var evaluationDictionary = _myDBContext.T訂餐評論表s
                .Join(_myDBContext.T訂餐訂單資訊表s, item => item.訂單id, a => a.訂單id, (item, a) => new { item, a })
                .Join(_myDBContext.T訂餐訂單詳細資訊表s, x => x.a.訂單id, b => b.訂單id, (x, b) => new { x.item, x.a, b })
                .Join(_myDBContext.T訂餐店家資料表s, y => y.b.店家id, c => c.店家id, (y, c) => new { y.item, y.a, y.b, c })
                .Where(z => z.b.狀態 == "1")
                .GroupBy(z => z.c.店家id)
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var storedata = group.Select(item => item.item)
                            .GroupBy(item => item.滿意度星數)
                            .Select(b =>
                            {
                                var distinctItems = b.Select(item => item).Distinct(); // 在每个组内进行去重操作
                                return new
                                {
                                    滿意度星數 = Convert.ToInt32(b.Key),
                                    評論數量 = distinctItems.Count(),
                                    加權 = Convert.ToInt32(b.Key) * distinctItems.Count()
                                };
                            }).ToList();
                        int totalComments = storedata.Sum(item => item.評論數量);
                        int totalWeight = storedata.Sum(item => item.加權);
                        double evaluate = totalComments != 0 ? Math.Round((double)totalWeight / totalComments, 2) : 0.0;
                        return evaluate;
                    }
                );




            var result = _myDBContext.T訂餐店家資料表s.Select(a => new VMstoreCarIndex
            {
                店家id = a.店家id,
                電話 = a.電話,
                餐廳照片 = a.餐廳照片 ?? "/images/user.jpg",
                店家名稱 = a.店家名稱,
                餐廳介紹 = a.餐廳介紹,
                風味列表 = (from tagItem in _myDBContext.T訂餐店家資料表s
                        join tagE in _myDBContext.T訂餐店家風味表s on tagItem.店家id equals tagE.店家id
                        join tagF in _myDBContext.T訂餐口味總表s on tagE.口味id equals tagF.口味id
                        where tagItem.店家名稱 == a.店家名稱
                        select tagF.風味名稱).ToList(),
                平均評論 = evaluationDictionary.ContainsKey(a.店家id) ? (evaluationDictionary[a.店家id]).ToString() : "新店家"
            }).ToList();
            return result;
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
                           where c.店家id == id && b.狀態 == "1"
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
            double evaluate = (totalWeight != 0) ? totalWeight / totalComments : 0;
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
