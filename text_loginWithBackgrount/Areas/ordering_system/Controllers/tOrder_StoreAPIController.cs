using Class_system_Backstage_pj.Areas.ordering_system.Data;
using Class_system_Backstage_pj.Areas.ordering_system.Models;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using text_loginWithBackgrount.Areas.ordering_system.Models;
using text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO;
using TEXTpie_chart.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace text_loginWithBackgrount.Areas.ordering_system.Controllers
{
    public class tOrder_StoreAPIController : Controller
    {
        private readonly studentContext _myDBContext;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        public tOrder_StoreAPIController(studentContext myDBContext, IEmailSender emailSender, IWebHostEnvironment env)
        {
            _myDBContext = myDBContext;
            _emailSender = emailSender;
            _env = env;
        }
        /// <summary>
        /// 呼叫所有店家資料
        /// </summary>
        /// <param name="myDBContext"></param>
        public IActionResult storeInformation()
        {
            var result = _myDBContext.T訂餐店家資料表s
                .Select(a => new
                {
                    a.店家id,
                    a.電話,
                    餐廳照片 = a.餐廳照片 ?? "/images/user.jpg",
                    a.店家名稱,
                    a.電子信箱,
                    a.餐廳介紹,
                    風味列表 = (from tagItem in _myDBContext.T訂餐店家資料表s
                            join tagE in _myDBContext.T訂餐店家風味表s on tagItem.店家id equals tagE.店家id
                            join tagF in _myDBContext.T訂餐口味總表s on tagE.口味id equals tagF.口味id
                            where tagItem.店家名稱 == a.店家名稱
                            select tagF.風味名稱).ToList(),
                });

            return Json(result);
        }
        /// <summary>
        /// 查詢店家平均評價分數
        /// </summary>
        /// <param name="id">店家id</param>
        /// <returns>店家加權的平均評價分數</returns>
        private static double StorefeedbakAvg(int id, studentContext _myDBContext)
        {
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
            return evaluate;
        }

        /// <summary>
        /// 依照店家id給予各菜單資訊
        /// </summary>
        /// <param name="id">指定店家</param>
        /// <returns>菜單資料回傳</returns>
        public IActionResult storeMenu(int id)
        {
            var result = _myDBContext.T訂餐餐點資訊表s.Where(a => a.店家id == id).Select(a => new
            {
                a.餐點名稱,
                餐點定價 = Convert.ToInt32(a.餐點定價),
                餐點照片 = a.餐點照片 ?? "/images/user.jpg",
            });
            return Json(result);
        }

        /// <summary>
        /// 傳送目前訂單數量、店家營業額加總、回傳評論平均
        /// </summary>
        /// <returns>首頁上方的方塊資料數量</returns>
        public IActionResult orderCount()
        {
            var result = _myDBContext.T訂餐訂單資訊表s.Count();
            var result1 = Convert.ToInt32(_myDBContext.T訂餐訂單詳細資訊表s.Where(x => x.狀態 == "1").Sum(a => a.金額小記));
            var result3 = Math.Round(_myDBContext.T訂餐評論表s.Average(a => Convert.ToDouble(a.滿意度星數)), 1);
            VMstoreInformation storeInformationVM = new VMstoreInformation()
            {
                turnover = result1,
                historyorder = result,
                evaluate = result3.ToString("0.0")
            };
            return Json(storeInformationVM);
        }
        /// <summary>
        /// 傳送指定店家訂單數量、店家營業額加總、回傳評論平均
        /// </summary>https://localhost:7150/tOrder_StoreAPI/orderCount01/5
        /// <returns>首頁上方的方塊資料數量</returns>
        public IActionResult orderCount01(int id)
        {
            var result = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.店家id == id).Count();
            var result1 = Convert.ToInt32(_myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.店家id == id && a.狀態 == "1").Sum(a => a.金額小記));
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
            double evaluate = totalWeight!=0?totalWeight / totalComments:0;
            VMstoreInformation storeInformationVM = new VMstoreInformation()
            {
                turnover = result1,
                historyorder = result,
                evaluate = evaluate.ToString("0.0"),
                commentsNum = totalComments
            };
            return Json(storeInformationVM);
        }
        /// <summary>
        /// 表格顯示資料筆數用
        /// </summary>
        /// <param name="_search">接收前端需要顯示的資料筆數</param>
        /// <returns>依照規定回傳需要顯示的筆數</returns>
        [HttpPost]
        public IActionResult jsonsort([FromBody] StoreShow _search)
        {
            var spots = _search.storeID == 0 ? _myDBContext.T訂餐店家資料表s : _myDBContext.T訂餐店家資料表s.Where(s => s.店家id == _search.storeID);
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.店家名稱.Contains(_search.keyword) || s.電子信箱.Contains(_search.keyword) || s.電話.Contains(_search.keyword));
            }
            int totalCount = spots.Count(); //總共幾筆
            int pagesize = _search.pageSize ?? 5; //一頁有幾筆資料
            int page = _search.page ?? 1; //目前顯示哪一頁
            int totalpage = (int)Math.Ceiling((decimal)(totalCount) / pagesize); //將小數點無條件進位，
            spots = spots.Skip((int)(page - 1) * pagesize).Take(pagesize);
            //轉換輸出格式，為了前端需求輸出
            PagingDTO sportsPagingDTO = new PagingDTO();
            sportsPagingDTO.TotalPages = totalpage;
            sportsPagingDTO.spotImagesSpots = spots.ToList();
            return Json(sportsPagingDTO);
        }
        /// <summary>
        /// 回傳評論星數數量給圓餅圖(全部不指定)
        /// </summary>
        /// <returns>圓餅圖需要的名稱與value</returns>
        public IActionResult pieComment()
        {
            List<VMjsonpie> jsonpies = new List<VMjsonpie>();
            var result = _myDBContext.T訂餐評論表s.GroupBy(a => a.滿意度星數).Select(b =>
            new
            {
                滿意度星數 = b.Key,
                評論數量 = b.Count()
            });
            foreach (var item in result)
            {
                VMjsonpie vMjsonpie = new VMjsonpie()
                {
                    itemLabel = (item.滿意度星數).Trim() + "星",
                    itemValue = item.評論數量
                };
                jsonpies.Add(vMjsonpie);
            }
            return Json(jsonpies);
        }


        /// <summary>
        /// 依照年分、月份計算營業額為多少
        /// </summary>
        /// <param name="yaer">希望年分</param>
        /// <param name="id">店家id預設為null顯示全部，有指定時顯示指定</param>
        /// <returns>一到十二月營業額陣列</returns>
        public IActionResult barchart_monthly_revenue(string yaer = "2023", int? id = null)
        {
            int[] month = new int[12];
            var result = from item in _myDBContext.T訂餐訂單詳細資訊表s
                         join a in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals a.餐點id
                         join b in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals b.訂單id
                         where b.訂單狀態.Contains("完成") && b.訂單時間.Substring(0, 4) == yaer &&
                         (id == null || item.店家id == id)
                         group item.金額小記 by b.訂單時間.Substring(4, 2)
                         into grouped
                         select new
                         {
                             年分月 = grouped.Key,
                             訂單總額 = grouped.Sum(item => item.Value)
                         };
            for (int i = 1; i < 13; i++)
            {
                foreach (var item in result)
                {
                    if (item.年分月 == i.ToString("00"))
                    {
                        month[i - 1] = Convert.ToInt32(item.訂單總額);
                        break;
                    }
                    else
                    {
                        month[i - 1] = 0;
                    }
                }
            }
            return Json(month);
        }
        /// <summary>
        /// 回傳評論星數數量給圓餅圖(指定店家)
        /// </summary>
        /// <returns>圓餅圖需要的名稱與value</returns>
        public IActionResult pieComment_withstore(int id)
        {
            List<VMjsonpie> jsonpies = new List<VMjsonpie>();
            var result = (from item in _myDBContext.T訂餐評論表s
                          join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                          join b in _myDBContext.T訂餐訂單詳細資訊表s on a.訂單id equals b.訂單id
                          join c in _myDBContext.T訂餐店家資料表s on b.店家id equals c.店家id
                          where c.店家id == id
                          select item).Distinct();
            var storedata = result.GroupBy(a => a.滿意度星數).Select(b =>
            new
            {
                滿意度星數 = b.Key,
                評論數量 = b.Count()
            });
            foreach (var item in storedata)
            {
                VMjsonpie vMjsonpie = new VMjsonpie()
                {
                    itemLabel = (item.滿意度星數).Trim() + "星",
                    itemValue = item.評論數量
                };
                jsonpies.Add(vMjsonpie);
            }
            return Json(jsonpies);
        }
        /// <summary>
        /// 依照店家ID回傳所有相關詳細資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult store_Detail_Information(int id)
        {
            var store = _myDBContext.T訂餐店家資料表s.Where(a => a.店家id == id).Select(b => new
            {
                店家id = b.店家id,
                店家名稱 = b.店家名稱,
                地址 = b.地址,
                電話 = b.電話,
                餐廳介紹 = b.餐廳介紹,
                餐廳照片 = b.餐廳照片 ?? "/images/user.jpg",
                電子信箱 = b.電子信箱,
                密碼 = b.密碼,
                風味列表 = (from tagItem in _myDBContext.T訂餐店家資料表s
                        join tagE in _myDBContext.T訂餐店家風味表s on tagItem.店家id equals tagE.店家id
                        join tagF in _myDBContext.T訂餐口味總表s on tagE.口味id equals tagF.口味id
                        where tagItem.店家名稱 == b.店家名稱
                        select tagF.風味名稱).ToList()
            }).FirstOrDefault();
            if (store != null)
            {
                //將店家id存於Session之中
                HttpContext.Session.SetString("storeID", (store.店家id).ToString());
                return Json(store);
            }
            return NotFound();
        }
        /// <summary>
        /// 生成五位數驗證碼
        /// </summary>
        /// <returns></returns>
        private string GenerateCaptcha()
        {
            // 生成一個包含隨機數字的驗證碼
            Random random = new Random();
            string captcha = random.Next(10000, 99999).ToString();
            return captcha;
        }
        /// <summary>
        /// 依照店家資料發送驗證信，將驗證碼存在Session中
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult YourAction(string email)
        {
            string password = GenerateCaptcha();
            //將驗證碼存儲在 Session 中
            HttpContext.Session.SetString("VerificationCode", password);
            var htmlMessage = C_presentationEmailhtml.htmlMessage(password);
            _emailSender.SendEmailAsync(email, "修改會員資料驗證信", htmlMessage);
            return Ok();
        }
        /// <summary>
        /// 核對驗證碼是否正確，正確從快取中刪除驗證碼
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult check_verification(string id)
        {
            //從 Session 中讀取驗證碼
            string? verify = HttpContext.Session.GetString("VerificationCode");
            if (verify != null)
            {
                if (verify != id)
                {
                    return NotFound();
                }
                // 從 Session 中刪除驗證碼
                HttpContext.Session.Remove("VerificationCode");
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// 輸入密碼驗證是否符合規定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Contact(PasswordviewModel model)
        {
            int? storeID = Convert.ToInt32(HttpContext.Session.GetString("storeID"));
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                return Json(new { isValid = false, errors = errors });
            }
            //通過驗證，抓取特定店家修改密碼
            var store = _myDBContext.T訂餐店家資料表s.FirstOrDefault(a => a.店家id == storeID);
            if (store != null)
            {
                store.密碼 = model.newPassword;
                await _myDBContext.SaveChangesAsync();
                //HttpContext.Session.Remove("storeID");
            }
            return Json(new { isValid = true });
        }
        /// <summary>
        /// 透過輸入驗證，指定店家資料修改，將店家id存於Session之中，進行資料的修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> store_deatail_form(storeinformationViewModel model)
        {
            string fileLocation = "";
            int? storeID = Convert.ToInt32(HttpContext.Session.GetString("storeID"));
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                return Json(new { isValid = false, errors = errors });
            }
            if (model.storeImg != null && model.storeImg.Length > 0)
            {
                var guid = Guid.NewGuid();
                var filesName = $"{guid}_{model.storeImg.FileName}";
                var rootDirectory = _env.ContentRootPath;
                var uploadDirectory = rootDirectory + @"\wwwroot\images\t訂餐\店家照片\"; // 指定路径
                var filescombine = Path.Combine(uploadDirectory, filesName);  //存進資料庫的影片位置
                fileLocation = "/images/t訂餐/店家照片/" + filesName;
                using (var system = System.IO.File.Create(filescombine)) //**補充
                {
                    await model.storeImg.CopyToAsync(system);
                }
            }
            //通過驗證，抓取特定店家修改店家資料
            var store = _myDBContext.T訂餐店家資料表s.FirstOrDefault(a => a.店家id == storeID);
            if (store != null)
            {
                if (fileLocation.IsNullOrEmpty())
                {
                    store.餐廳照片 = store.餐廳照片.IsNullOrEmpty() ? "/images/t訂餐/店家照片/jai西門.jpg" : store.餐廳照片;
                }
                else
                {
                    store.餐廳照片 = fileLocation;
                }
                store.店家名稱 = model.storeName;
                store.電子信箱 = model.storeEmail;
                store.電話 = model.storePhone;
                store.地址 = model.storeAdress;
                store.餐廳介紹 = model.storeinformation;
                var oldtag = (_myDBContext.T訂餐店家風味表s.Where(a => a.店家id == storeID)).ToList();
                _myDBContext.RemoveRange(oldtag);
                _myDBContext.SaveChanges();
                if (model.taglist != null)
                {
                    List<string> tagArray = (model.taglist.Split(",")).ToList();
                    tagArray.RemoveAt(tagArray.Count - 1);
                    foreach (var tag in tagArray)
                    {
                        int tagid = Convert.ToInt32(_myDBContext.T訂餐口味總表s.FirstOrDefault(a => a.風味名稱.Trim() == tag).口味id);
                        var newTag = new T訂餐店家風味表
                        {
                            店家id = (int)storeID,
                            口味id = tagid
                        };
                        _myDBContext.T訂餐店家風味表s.Add(newTag);
                        _myDBContext.SaveChanges();
                    }
                }
                _myDBContext.SaveChanges();
                //HttpContext.Session.Remove("storeID");
            }
            return Json(new { isValid = true });
        }
        /// <summary>
        /// 將店家歷史訂單依照需求分頁顯示，且判斷年月資料
        /// </summary>
        /// <param name="_search"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult history_order([FromBody] StoreShow _search)
        {
            var result = from item in _myDBContext.T訂餐訂單詳細資訊表s
                         join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                         join b in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals b.餐點id
                         join c in _myDBContext.T訂餐店家資料表s on item.店家id equals c.店家id
                         where c.店家id == _search.storeID && (a.訂單時間).Substring(0, 4) == (_search.year).ToString() && (a.訂單時間).Substring(4, 2) == (_search.mounth).ToString().PadLeft(2, '0') && item.狀態 != "-1"
                         select new orderdeatialViewModel
                         {
                             訂單編號 = (item.訂單詳細表id),
                             餐點名稱 = b.餐點名稱,
                             數量 = item.餐點數量,
                             金額 = Convert.ToInt32(b.餐點定價 * item.餐點數量),
                             支付方式 = (a.支付方式).Trim(),
                             訂單狀態 = item.狀態 == "1" ? "完成" : item.狀態 == "0" ? "進行中" : item.狀態 == "-1" ? "取消" : "取消",
                             訂單日期 = a.訂單時間,
                             type = item.狀態 == "1" ? "completed" : item.狀態 == "0" ? "process" : item.狀態 == "-1" ? "pending" : "completed"
                         };
            switch (_search.sortBy)
            {
                case "訂單日期":
                    result = _search.sortType == "asc" ? result.OrderBy(a => a.訂單日期) : result.OrderByDescending(a => a.訂單日期);
                    break;
                case "訂單狀態":
                    result = _search.sortType == "asc" ? result.OrderBy(a => a.訂單狀態) : result.OrderByDescending(a => a.訂單狀態);
                    break;
                case "訂單編號":
                    result = _search.sortType == "asc" ? result.OrderBy(a => a.訂單編號) : result.OrderByDescending(a => a.訂單編號);
                    break;

            }
            int totalCount = result.Count(); //總共幾筆
            int pagesize = _search.pageSize ?? 5; //一頁有幾筆資料
            int page = _search.page ?? 1; //目前顯示哪一頁
            int totalpage = (int)Math.Ceiling((decimal)(totalCount) / pagesize); //將小數點無條件進位，
            result = result.Skip((int)(page - 1) * pagesize).Take(pagesize);
            //轉換輸出格式，為了前端需求輸出
            PagingDTO sportsPagingDTO = new PagingDTO();
            sportsPagingDTO.TotalPages = totalpage;
            sportsPagingDTO.spotOrderdeatial = result.ToList();
            return Json(sportsPagingDTO);
            //return Json(result);
        }
        /// <summary>
        /// 該筆訂單評論顯示內容與百分比
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult order_Evaluate(int id)
        {
            var order = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.訂單詳細表id == id).FirstOrDefault();
            if (order != null)
            {
                var orderDeatailList = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.訂單id == order.訂單id).Select(b => b.訂單詳細表id).ToArray();
                var result = (from item in _myDBContext.T訂餐訂單詳細資訊表s
                              join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                              join b in _myDBContext.T會員學生s on a.學員id equals b.學生id
                              join c in _myDBContext.T訂餐評論表s on a.訂單id equals c.訂單id
                              where item.訂單詳細表id == id
                              select new
                              {
                                  評價內容 = c.評論,
                                  評價分數 = (c.滿意度星數).Trim(),
                                  訂單日期 = a.訂單時間,
                                  訂單人名稱 = b.姓名,
                                  百分比 = (Convert.ToInt32(c.滿意度星數) % 5) * 20 == 0 ? 100 : (Convert.ToInt32(c.滿意度星數) % 5) * 20,
                                  關聯訂單 = orderDeatailList
                              }).ToArray();
                if (result.Length != 0) { return Json(result); }
            }
            return NotFound();
        }
        /// <summary>
        /// 顯示該子訂單的所有關聯訂單
        /// </summary>
        /// <param name="id">詳細訂單</param>
        /// <returns></returns>
        public IActionResult order_showdeatial(int id)
        {
            var order = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.訂單詳細表id == id).Select(b => b.訂單id).First();
            if (order != null)
            {
                var result = (from item in _myDBContext.T訂餐訂單詳細資訊表s
                              join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                              join b in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals b.餐點id
                              join c in _myDBContext.T訂餐店家資料表s on item.店家id equals c.店家id
                              where a.訂單id == order && item.狀態 != "-1"
                              select new
                              {
                                  訂單編號 = (item.訂單詳細表id),
                                  店家名稱 = (c.店家名稱).Trim(),
                                  餐點名稱 = b.餐點名稱,
                                  數量 = item.餐點數量,
                                  金額 = Convert.ToInt32(b.餐點定價 * item.餐點數量),
                                  支付方式 = (a.支付方式).Trim(),
                              }).ToList();
                var 總金額 = result.Sum(x => x.金額);
                var order_showdeatial = new
                {
                    訂單詳細 = result,
                    總金額
                };
                return Json(order_showdeatial);
            }
            return NotFound();
        }
        /// <summary>
        /// 依照2024年訂單金額與評論星數交叉最高前五名排名
        /// </summary>
        /// <returns></returns>
        public IActionResult bestStoreTop5()
        {
            var order2023totalByStore = (from item in _myDBContext.T訂餐訂單詳細資訊表s
                                         join a in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals a.餐點id
                                         join b in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals b.訂單id
                                         join c in _myDBContext.T訂餐店家資料表s on item.店家id equals c.店家id
                                         join d in _myDBContext.T訂餐評論表s on b.訂單id equals d.訂單id
                                         join e in _myDBContext.T訂餐店家風味表s on c.店家id equals e.店家id
                                         join f in _myDBContext.T訂餐口味總表s on e.口味id equals f.口味id
                                         where (b.訂單狀態).Trim() == "完成" && (item.狀態).Trim()=="1"&& (b.訂單時間).Substring(0, 4) == "2024"
                                         group new { item, b, a, c, d, e, f } by c into g
                                         select new
                                         {
                                             店家id = g.Key.店家id,
                                             店家名稱 = g.Key.店家名稱,
                                             店家介紹 = g.Key.餐廳介紹,
                                             店家圖片 = g.Key.餐廳照片 ?? "/images/user.jpg",
                                             訂單總額 = g.Sum(x => x.item.餐點數量 * x.a.餐點定價),
                                             評價星數 = Math.Round(g.Average(x => Convert.ToInt32(x.d.滿意度星數))),
                                             風味列表 = (from tagItem in _myDBContext.T訂餐店家資料表s
                                                     join tagE in _myDBContext.T訂餐店家風味表s on tagItem.店家id equals tagE.店家id
                                                     join tagF in _myDBContext.T訂餐口味總表s on tagE.口味id equals tagF.口味id
                                                     where tagItem.店家名稱 == g.Key.店家名稱
                                                     select tagF.風味名稱).ToList()
                                         }).OrderByDescending(c => c.訂單總額).ThenByDescending(a => a.評價星數).Take(5).ToList();

            return Json(order2023totalByStore);
        }
        /// <summary>
        /// 取得口味總表的清單
        /// </summary>
        /// <returns></returns>
        public IActionResult storeStyle()
        {
            var data = _myDBContext.T訂餐口味總表s.Select(a => a.風味名稱).ToList();
            return Ok(data);
        }
        /// <summary>
        /// 回傳找出店家中銷售最高的前五名菜單
        /// </summary>
        /// <returns></returns>
        public IActionResult topfivemenu(int id)
        {
            var data = (from item in _myDBContext.T訂餐訂單詳細資訊表s
                        join a in _myDBContext.T訂餐店家資料表s on item.店家id equals a.店家id
                        join b in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals b.餐點id
                        where a.店家id == id && item.狀態 == "1"
                        group item by b into g
                        select new
                        {
                            餐點名稱 = g.Key.餐點名稱,
                            銷售數量 = g.Sum(x => x.餐點數量)
                        }).OrderByDescending(a => a.銷售數量).Take(5).ToList();

            return Ok(data);
        }
        /// <summary>
        /// 菜單餐點意見回饋，計算優良佔筆與熱門菜單
        /// </summary>https://localhost:7150/tOrder_StoreAPI/ReviewExcellentRatio/2
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ReviewExcellentRatio(int id)
        {
            var orderCount = _myDBContext.T訂餐訂單詳細資訊表s.Where(a => a.店家id == id).Count();

            var totalStars = (from item in _myDBContext.T訂餐訂單詳細資訊表s
                              join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                              join b in _myDBContext.T訂餐評論表s on a.訂單id equals b.訂單id
                              where item.店家id == id && Convert.ToInt32(b.滿意度星數) >= 4
                              select b).Count();
            var hotmeal = from item in _myDBContext.T訂餐訂單詳細資訊表s
                          join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                          join b in _myDBContext.T訂餐評論表s on a.訂單id equals b.訂單id
                          join c in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals c.餐點id
                          where item.店家id == id && Convert.ToInt32(b.滿意度星數) >= 4
                          group new { c, b } by c.餐點名稱 into g
                          select new
                          {
                              餐點名稱 = g.Key,
                              評價分數 = g.Select(x => x.b.滿意度星數).FirstOrDefault(),
                              筆數 = g.Count(),
                              加總評分 = (Convert.ToInt32(g.Select(x => x.b.滿意度星數).FirstOrDefault())) * g.Count()
                          };
            var hotmealest = hotmeal.OrderByDescending(a => a.加總評分).FirstOrDefault();
            var icemeal = from item in _myDBContext.T訂餐訂單詳細資訊表s
                          join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                          join b in _myDBContext.T訂餐評論表s on a.訂單id equals b.訂單id
                          join c in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals c.餐點id
                          where item.店家id == id && Convert.ToInt32(b.滿意度星數) <= 3
                          group new { c, b } by c.餐點名稱 into g
                          select new
                          {
                              餐點名稱 = g.Key,
                              評價分數 = g.Select(x => x.b.滿意度星數).FirstOrDefault(),
                              筆數 = g.Count(),
                              加總評分 = (Convert.ToInt32(g.Select(x => x.b.滿意度星數).FirstOrDefault())) * g.Count()
                          };

            var lowmealest = icemeal.OrderBy(a => a.加總評分).First();
            var dataobject = new
            {
                總筆數 = orderCount,
                優良筆數 = totalStars,
                總訂單評價優良比 = (totalStars != 0) ? Math.Round((double)totalStars / (double)orderCount * 100, 1) : 0,
                最熱門菜單 = hotmealest.餐點名稱,
                熱門菜單平均評價 = (hotmealest.加總評分 != null) ? Math.Round((double)(hotmealest.加總評分 / hotmealest.筆數), 1) : 0,
                熱門佔總訂單的比率 = (hotmealest != null) ? Math.Round(((double)hotmealest.筆數 / orderCount) * 100, 1) : 0,
                最冷門菜單 = lowmealest.餐點名稱,
                冷門平均評價 = (lowmealest.加總評分 != null) ? Math.Round(((double)lowmealest.加總評分 / lowmealest.筆數), 1) : 0,
                冷門菜單總訂單的比率 = (lowmealest != null) ? Math.Round(((double)lowmealest.筆數 / orderCount) * 100, 1) : 0,
            };
            return Ok(dataobject);
        }
        /// <summary>
        /// 透過顯示呼叫特定店家的所有餐點
        /// </summary>
        /// <param name="_search"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult storeMenu([FromBody] StoreShow _search)
        {
            var spots = _search.storeID == 0 ? _myDBContext.T訂餐餐點資訊表s : _myDBContext.T訂餐餐點資訊表s.Where(s => s.店家id == _search.storeID);
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.餐點名稱.Contains(_search.keyword) || s.餐點描述.Contains(_search.keyword));
            }
            switch (_search.sortBy)
            {
                case "上架":
                    spots = _search.sortType == "asc" ? spots.OrderBy(a => a.上架) : spots.OrderByDescending(a => a.上架);
                    break;
                case "餐點定價":
                    spots = _search.sortType == "asc" ? spots.OrderBy(a => a.餐點定價) : spots.OrderByDescending(a => a.餐點定價);
                    break;
                case "餐點名稱":
                    spots = _search.sortType == "asc" ? spots.OrderBy(a => a.餐點名稱) : spots.OrderByDescending(a => a.餐點名稱);
                    break;
                case "餐點描述":
                    spots = _search.sortType == "asc" ? spots.OrderBy(a => a.餐點描述) : spots.OrderByDescending(a => a.餐點描述);
                    break;

            }
            int totalCount = spots.Count(); //總共幾筆
            int pagesize = _search.pageSize ?? 5; //一頁有幾筆資料
            int page = _search.page ?? 1; //目前顯示哪一頁
            int totalpage = (int)Math.Ceiling((decimal)(totalCount) / pagesize); //將小數點無條件進位，
            spots = spots.Skip((int)(page - 1) * pagesize).Take(pagesize);
            //轉換輸出格式，為了前端需求輸出
            PagingDTO sportsPagingDTO = new PagingDTO();
            sportsPagingDTO.TotalPages = totalpage;
            sportsPagingDTO.spotMeauList = spots.ToList();
            return Json(sportsPagingDTO);
        }
        /// <summary>
        /// 顯示該筆餐點的詳細資料
        /// </summary>
        /// <param name="id">餐點ID</param>
        /// <returns></returns>
        public IActionResult MenuDeatail(int id)
        {
            var result = _myDBContext.T訂餐餐點資訊表s.Where(a => a.餐點id == id);
            return Json(result);
        }
        /// <summary>
        /// 新增或是修改餐點資料
        /// </summary>
        /// <param name="model">Menumanagement餐點資訊表格</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> meal_deatail_form(menuDeatailDTO model)
        {
            int ID = model.menuID != 0 ? model.menuID : 0;
            var fileLocation = "";
            if (string.IsNullOrWhiteSpace(model.餐點售價.ToString()))
            {
                ModelState.AddModelError("餐點售價", "餐點售價不可空白");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                return Json(new { isValid = false, errors = errors });
            }
            if (model.file != null && model.file.Length > 0)
            {
                var guid = Guid.NewGuid();
                var filesName = $"{guid}_{model.file.FileName}";
                var rootDirectory = _env.ContentRootPath;
                var uploadDirectory = rootDirectory + @"\wwwroot\images\t訂餐\餐點\"; // 指定路径
                var filescombine = Path.Combine(uploadDirectory, filesName);  //存進資料庫的影片位置
                fileLocation = "/images/t訂餐/餐點/" + filesName;
                using (var system = System.IO.File.Create(filescombine)) //**補充
                {
                    await model.file.CopyToAsync(system);
                }
            }
            //通過驗證，抓取特定餐點資料
            var store = _myDBContext.T訂餐餐點資訊表s.FirstOrDefault(a => a.餐點id == model.menuID);
            if (store != null)
            {
                if (fileLocation.IsNullOrEmpty())
                {
                    store.餐點照片 = store.餐點照片.IsNullOrEmpty() ? "/images/t訂餐/餐點/noimage.jpg" : store.餐點照片;
                }
                else
                {
                    store.餐點照片 = fileLocation;
                }
                store.餐點名稱 = model.餐點名稱;
                store.餐點定價 = model.餐點售價;
                store.餐點描述 = model.餐點描述;
                _myDBContext.SaveChanges();
            }
            else
            {
                T訂餐餐點資訊表 newMenu = new T訂餐餐點資訊表
                {
                    店家id = model.storeID,
                    餐點名稱 = model.餐點名稱,
                    餐點定價 = model.餐點售價,
                    餐點描述 = model.餐點描述,
                    上架 = "1",
                    餐點照片 = fileLocation.IsNullOrEmpty() ? "/images/t訂餐/餐點/noimage.jpg" : fileLocation
                };
                _myDBContext.T訂餐餐點資訊表s.Add(newMenu);
                _myDBContext.SaveChanges();
            }
            return Json(new { isValid = true });
        }
        /// <summary>
        /// 選擇餐點修改上下架餐點
        /// </summary>
        /// <param name="id">餐點ID</param>
        /// <returns></returns>
        public IActionResult MenuShelves(int id)
        {
            var result = _myDBContext.T訂餐餐點資訊表s.Where(a => a.餐點id == id).FirstOrDefault();
            if (result != null)
            {
                if (result.上架.Trim() == "1")
                {
                    result.上架 = "0";
                }
                else
                {
                    result.上架 = "1";
                }
                _myDBContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
        /// <summary>
        /// 本日訂單數、當月新訂單比率、過往訂單回購率
        /// </summary>https://localhost:7150/tOrder_StoreAPI/Orderanalysis/2
        /// <param name="id">店家ID</param>
        /// <returns></returns>
        public IActionResult Orderanalysis(int id)
        {
            DateTime date = DateTime.Now;
            string[] formattedDate = date.ToString("yyyy-MM-dd").Split("-");
            string year = formattedDate[0];
            string month = formattedDate[1];
            string day = formattedDate[2];

            var resultToday = from item in _myDBContext.T訂餐訂單詳細資訊表s
                              join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                              join b in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals b.餐點id
                              join c in _myDBContext.T訂餐店家資料表s on item.店家id equals c.店家id
                              where c.店家id == id && (a.訂單時間).Substring(0, 4) == year && (a.訂單時間).Substring(4, 2) == month && (a.訂單時間).Substring(6, 2) == day
                              select new orderdeatialViewModel
                              {
                                  訂單編號 = (item.訂單詳細表id),
                                  餐點名稱 = b.餐點名稱,
                                  數量 = item.餐點數量,
                                  金額 = Convert.ToInt32(b.餐點定價 * item.餐點數量),
                                  支付方式 = (a.支付方式).Trim(),
                                  訂單狀態 = (a.訂單狀態).Trim(),
                                  訂單日期 = (a.訂單時間).Substring(9, 5),
                                  type = (item.狀態).Trim()
                              };
            //回傳本日訂單數資料
            int todayOrderCount = resultToday.Count();
            //店家當月份母訂單資料
            var intermediateResult = (from item in _myDBContext.T訂餐訂單資訊表s
                                      join a in _myDBContext.T訂餐訂單詳細資訊表s on item.訂單id equals a.訂單id
                                      where a.店家id == id && item.訂單時間.Substring(0, 4) == year && (item.訂單時間).Substring(4, 2) == month
                                      select new { item, a }).ToList();
            //當月中是首次購買的訂單數量
            var result = (intermediateResult
                        .GroupBy(x => x.item.學員id) // 按照學員ID分組
                        .Where(g => g.Count() == 1) // 只出现一次的訂單ID
                        .SelectMany(g => g.Select(x => x.item.訂單id))).Distinct().ToList().Count();

            //回傳當月訂單中是首次購買的比率
            double firstOrderMonth = result != 0 ? ((double)result / (double)intermediateResult.Select(a => a.item.訂單id).Distinct().Count()) * 100 : 0;

            //店家全部母訂單資料
            var orderTotal = (from item in _myDBContext.T訂餐訂單資訊表s
                              join a in _myDBContext.T訂餐訂單詳細資訊表s on item.訂單id equals a.訂單id
                              where a.店家id == id
                              select new { item, a }).ToList();
            var result1 = (orderTotal.GroupBy(x => x.item.學員id) // 按照學員ID分組
                           .Where(g => g.Count() > 1) // 出現超過一次的訂單ID
                          .SelectMany(g => g.Select(x => x.item.訂單id))).Distinct().ToList().Count();
            var todayFinishOrder = resultToday.Where(a => a.type == "1").ToList();
            //回傳全部訂單中的回購比率
            double notfirstOrder = result1 != 0 ? ((double)result1 / (double)orderTotal.Select(a => a.item.訂單id).Distinct().Count()) * 100 : 0;

            var resultObject = new
            {
                本日訂單數 = todayOrderCount,
                當月新訂單比率 = firstOrderMonth.ToString("0.00") + "%",
                過往訂單回購率 = notfirstOrder.ToString("0.00") + "%",
                本日進行中訂單 = resultToday.Where(a => a.type == "0").ToList(),
                本日完成或取消訂單 = resultToday.Where(a => a.type != "0").ToList(),
                完成訂單總額 = todayFinishOrder.Sum(a => a.金額),
            };
            return Ok(resultObject);
        }
        /// <summary>
        /// 指定詳細訂單修改狀態資訊
        /// </summary>
        /// <param name="id">詳細訂單ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OrdertypeOK(int id, string type)
        {
            var result = _myDBContext.T訂餐訂單詳細資訊表s.FirstOrDefault(a => a.訂單詳細表id == id);
            if (result != null)
            {
                result.狀態 = type;
                _myDBContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// 依照DataTable回傳詳細訂單的資料
        /// </summary>https://localhost:7150/tOrder_StoreAPI/OrdertypeForDataTable/3
        /// <param name="id">詳細訂單ID</param>
        /// <returns>詳細訂單資料</returns>
        [HttpGet]
        public IActionResult OrdertypeForDataTable(int id)
        {
            var result = from item in _myDBContext.T訂餐訂單詳細資訊表s
                         join a in _myDBContext.T訂餐訂單資訊表s on item.訂單id equals a.訂單id
                         join b in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals b.餐點id
                         join c in _myDBContext.T訂餐店家資料表s on item.店家id equals c.店家id
                         where item.訂單詳細表id == id
                         select new orderdeatialViewModel
                         {
                             訂單編號 = (item.訂單詳細表id),
                             餐點名稱 = b.餐點名稱,
                             數量 = item.餐點數量,
                             金額 = Convert.ToInt32(b.餐點定價 * item.餐點數量),
                             支付方式 = (a.支付方式).Trim(),
                             訂單狀態 = item.狀態 == "1" ? "完成" : item.狀態 == "0" ? "進行中" : item.狀態 == "-1" ? "取消" : "取消",
                             訂單日期 = (a.訂單時間).Substring(0, 4) + "-" + (a.訂單時間).Substring(4, 2) + "-" + (a.訂單時間).Substring(6, 2),
                             type = (item.狀態).Trim(),
                         };
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        /// <summary>
        /// 
        /// </summary>https://localhost:7150/tOrder_StoreAPI/storeTimeManger/2
        /// <param name="id">店家ID</param>
        /// <returns></returns>
        public IActionResult storeTimeManger(int id)
        {
            var result = _myDBContext.T訂餐營業時間表s
                .Where(a => a.店家id == id)
                .GroupBy(a => a.時段早中晚全)
                .Select(group => new
                {
                    時間表名稱 = group.Key,
                    顯示 = group.First().顯示.Trim(),
                    時間表 = group.ToList()
                })
                .ToList();

            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">店家ID</param>
        /// <param name="name">修改的時段名稱</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult storeTimeManger(int id, string name)
        {
            var result = _myDBContext.T訂餐營業時間表s.Where(a => a.店家id == id).ToList();
            if (result != null)
            {
                //將店家營業時間初始化
                foreach (var item in result)
                {
                    item.顯示 = "0";
                }
                var togaldata = result.Where(a => a.時段早中晚全.Trim() == name);
                foreach (var data in togaldata)
                {
                    data.顯示 = "1";
                }
                try
                {
                    _myDBContext.SaveChanges(); // 儲存更改到資料庫
                    return Ok(); // 返回成功回應
                }
                catch (Exception ex)
                {
                    // 可以記錄錯誤或根據需要進行其他處理
                    return StatusCode(500, "內部服務器錯誤"); // 返回 500 Internal Server Error
                }
            }
            return BadRequest();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vMtimeForm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult storeTimeAdd(VMtimeForm vMtimeForm)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                return Json(new { isValid = false, errors = errors });
            }
            T訂餐營業時間表 t = new T訂餐營業時間表
            {
                店家id = Convert.ToInt32(vMtimeForm.ID),
                星期 = vMtimeForm.weeklist.ToString(),
                時段早中晚全 = vMtimeForm.titletime,
                開始營業時間 = vMtimeForm.start,
                結束營業時間 = vMtimeForm.end,
                顯示 = vMtimeForm.timeShow,
            };
            try
            {
                _myDBContext.T訂餐營業時間表s.Add(t);
                _myDBContext.SaveChanges();
                return Json(new { isValid = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "內部服務器錯誤"); // 返回 500 Internal Server Error
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult storeTime(int id)
        {
            var result = _myDBContext.T訂餐營業時間表s.Where(a => a.營業時間表id == id).FirstOrDefault();
            if (result != null)
            {
                _myDBContext.T訂餐營業時間表s.Remove(result);
                _myDBContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult text(int id)
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
            return Ok(store);
            // https://localhost:7150/tOrder_StoreAPI/bestStoreTop5
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult shoppingCar(int id)
        {
            var query = (from item in _myDBContext.T訂餐購物車s
                         join a in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals a.餐點id
                         where item.學員id == id && item.狀態.Trim() == "0"
                         select new
                         {
                             購物車ID = item.購物車id,
                             餐點圖片 = a.餐點照片,
                             餐點數量 = item.數量,
                             餐點名稱 = a.餐點名稱,
                             餐點ID = item.餐點id,
                             小計 = Convert.ToInt32(item.數量 * a.餐點定價),
                         }).ToList();
            int 總額 = query.Sum(item => item.小計);
            int 項目 = query.Count();
            var result = new
            {
                待購物清單 = query,
                總額 = 總額,
                筆數 = 項目
            };
            if (query != null)
            {

                return Ok(result);
            }
            return BadRequest();

        }
        /// <summary>
        /// 如果該學員購物車中待結帳裡有，該餐點修改其數量，沒有的話新增該餐點至購物車中
        /// </summary>
        /// <param name="id">學員ID</param>
        /// <param name="menuID">餐點ID</param>
        /// <param name="count">餐點數量</param>
        /// <returns></returns>
        public IActionResult ModifyshoppingCar(int id, int menuID, int count)
        {
            var result = _myDBContext.T訂餐購物車s.Where(a => a.學員id == id && a.狀態 == "0" && a.餐點id == menuID).FirstOrDefault();
            if (result != null)
            {
                result.數量 = count;
                _myDBContext.SaveChanges();
                return Ok();
            }
            else
            {
                var storeID = _myDBContext.T訂餐餐點資訊表s.Where(a => a.餐點id == menuID).Select(b => b.店家id).FirstOrDefault();
                var price = _myDBContext.T訂餐餐點資訊表s.Where(a => a.餐點id == menuID).Select(a => a.餐點定價).FirstOrDefault();
                T訂餐購物車 newcart = new T訂餐購物車
                {
                    學員id = id,
                    店家id = storeID,
                    餐點id = menuID,
                    數量 = count,
                    狀態 = "0",
                    金額小記 = (price * count),
                };
                _myDBContext.T訂餐購物車s.Add(newcart);
                _myDBContext.SaveChanges();
                return Ok();
            }
        }
        /// <summary>
        /// 查找購物車中是否有已存在餐點的將其刪除
        /// </summary>
        /// <param name="id">學員ID</param>
        /// <param name="menuID">餐點ID</param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult removeShoppingCar(int id, int menuID)
        {
            var result = _myDBContext.T訂餐購物車s.Where(a => a.學員id == id && a.餐點id == menuID && a.狀態 == "0").FirstOrDefault();
            if (result != null)
            {
                _myDBContext.T訂餐購物車s.Remove(result);
                _myDBContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// 查詢學生基本資料
        /// </summary>
        /// <param name="id">學生ID</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult studentinformation(int id)
        {
            var result = from item in _myDBContext.T會員學生s
                         join a in _myDBContext.T課程學生班級s on item.學生id equals a.學生id
                         join b in _myDBContext.T課程班級s on a.班級id equals b.班級id
                         where item.學生id == id && b.狀態 == 1
                         select new
                         {
                             電子信箱 = item.信箱 ?? "未設定信箱",
                             電話 = item.手機 ?? "未設定手機",
                             班級 = b.班級名稱 ?? "目前沒有班級",
                             學生姓名 = item.姓名
                         };
            return Ok(result.FirstOrDefault());
        }
        /// <summary>
        /// 對照購物車成立訂單
        /// </summary>
        /// <param name="id">學生ID</param>
        /// <param name="type">付款方式</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreatOrder(int id, string type)
        {
            using (var transaction = _myDBContext.Database.BeginTransaction())
            {
                try
                {
                    T訂餐訂單資訊表 order = new T訂餐訂單資訊表
                    {
                        學員id = id,
                        訂單狀態 = "進行中",
                        訂單時間 = DateTime.Now.ToString("yyyyMMdd HH:mm"),
                        支付方式 = type
                    };
                    _myDBContext.T訂餐訂單資訊表s.Add(order);
                    _myDBContext.SaveChanges();
                    int orderID = order.訂單id;

                    var query = (from item in _myDBContext.T訂餐購物車s
                                 join a in _myDBContext.T訂餐餐點資訊表s on item.餐點id equals a.餐點id
                                 where item.學員id == id && item.狀態.Trim() == "0"
                                 select new T訂餐訂單詳細資訊表
                                 {
                                     訂單id = orderID,
                                     店家id = a.店家id,
                                     餐點id = a.餐點id,
                                     餐點數量 = item.數量,
                                     金額小記 = Convert.ToDecimal(item.數量 * a.餐點定價),
                                     狀態 = "0"
                                 }).ToList();
                    _myDBContext.T訂餐訂單詳細資訊表s.AddRange(query);

                    var shoppcart = _myDBContext.T訂餐購物車s.Where(a => a.學員id == id).ToList();
                    foreach (var a in shoppcart)
                    {
                        a.狀態 = "1";
                    }

                    _myDBContext.SaveChanges();
                    transaction.Commit(); // 提交事务
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // 回滚事务
                    return BadRequest(ex.Message);
                }
            }
        }
        /// <summary>
        /// 查找目前進行中的訂單
        /// </summary>
        /// <param name="id">學生ID</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult orderNotcomplent(int id)
        {
            var spots = from item in _myDBContext.T訂餐訂單資訊表s
                        join a in _myDBContext.T訂餐訂單詳細資訊表s on item.訂單id equals a.訂單id
                        join b in _myDBContext.T訂餐店家資料表s on a.店家id equals b.店家id
                        join c in _myDBContext.T訂餐餐點資訊表s on a.餐點id equals c.餐點id
                        where item.學員id == id && item.訂單狀態.Trim() == "進行中"
                        group new { a, c, b } by new { item.訂單id, item.訂單狀態, item.訂單時間, item.支付方式 } into g
                        select new 學生訂單
                        {
                            訂單id = g.Key.訂單id,
                            狀態 = g.Key.訂單狀態.Trim(),
                            日期 = g.Key.訂單時間,
                            支付方式 = g.Key.支付方式.Trim(),
                            總價 = "$" + Convert.ToInt32(g.Sum(x => x.a.餐點數量 * x.c.餐點定價)),
                            訂單詳細表 = g.Select(x => new 訂單詳細表
                            {
                                店家名稱 = x.b.店家名稱,
                                餐點名稱 = x.c.餐點名稱,
                                數量 = x.a.餐點數量,
                                小計 = Convert.ToInt32(x.a.餐點數量 * x.c.餐點定價),
                                信箱 = x.b.電子信箱,
                                電話 = x.b.電話.Trim(),
                                狀態 = x.a.狀態.Trim() == "1" ? "已完成" : x.a.狀態.Trim() == "0" ? "進行中" : x.a.狀態.Trim() == "-1" ? "取消" : "與客服聯繫"
                            }).ToList()
                        };
            return Ok(spots);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">訂單ID</param>
        /// <param name="start">評價星數</param>
        /// <param name="feedback">訂單評論</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult orderFinallcomplent(int id, string start, string feedback)
        {
            var order = _myDBContext.T訂餐訂單資訊表s.Where(a => a.訂單id == id).FirstOrDefault();
            if (order != null)
            {
                order.訂單狀態 = "完成";
                T訂餐評論表 orderfeedback = new T訂餐評論表
                {
                    訂單id = id,
                    滿意度星數 = start,
                    評論 = feedback
                };
                _myDBContext.T訂餐評論表s.Add(orderfeedback);
                _myDBContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        //https://localhost:7150/tOrder_StoreAPI/shoppingCar/7
    }
}
