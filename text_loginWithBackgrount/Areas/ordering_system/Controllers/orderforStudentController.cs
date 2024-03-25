using Class_system_Backstage_pj.Areas.ordering_system.Models;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using text_loginWithBackgrount.Areas.ordering_system.Models;
using text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static text_loginWithBackgrount.Areas.ordering_system.Models.HomeViewModel;

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
        public IActionResult Studentrestaurant(string searchKeyWord = null,string orderby= "評論")
        {
            ViewData["searchKeyWord"] =searchKeyWord;
            ViewData["Title"] = "學生餐廳";
            var data = vMstoreCarIndices();
            switch (orderby) {
                case "新店家":
                    data = data.OrderBy(a => a.平均評論).ToList();
                    break;
                case "評論":
                    data = data.OrderByDescending(a => a.平均評論).ToList();
                    break;
            }
            return View(data);
        }
        public IActionResult CreateOrder()
        {
            ViewData["Title"] = "訂單成立";
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            // 產生測試資訊
            ViewData["MerchantID"] = Config.GetSection("MerchantID").Value;
            ViewData["MerchantOrderNo"] = DateTime.Now.ToString("yyyyMMddHHmmss");  //訂單編號
            ViewData["ExpireDate"] = DateTime.Now.AddDays(3).ToString("yyyyMMdd"); //繳費有效期限
            ViewData["ReturnURL"] = $"{Request.Scheme}://{Request.Host}{Request.Path}ordering_system/orderforStudent/CreateOrder"; //支付完成返回商店網址
            ViewData["CustomerURL"] = $"{Request.Scheme}://{Request.Host}{Request.Path}ordering_system/orderforStudent/Index"; //商店取號網址
            ViewData["NotifyURL"] = $"{Request.Scheme}://{Request.Host}{Request.Path}ordering_system/orderforStudent/CreateOrder"; //支付通知網址
            ViewData["ClientBackURL"] = $"{Request.Scheme}://{Request.Host}{Request.Path}ordering_system/orderforStudent/CreateOrder"; //返回商店網址 
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
                        if (totalComments <5)
                        {
                            evaluate = 0.0;
                        }
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
                平均評論 = evaluationDictionary.ContainsKey(a.店家id) ? evaluationDictionary[a.店家id]==0?0: evaluationDictionary[a.店家id] : 0,
                營業資料= (from time in _myDBContext.T訂餐營業時間表s
                       where time.店家id == a.店家id && time.顯示.Trim() == "1"
                       select time).ToList(),
                口味總表=_myDBContext.T訂餐口味總表s.Select(a=>a.風味名稱).ToList(),
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
        /// <summary>
        /// 傳送訂單至藍新金流
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        public IActionResult SendToNewebPay(SendToNewebPayIn inModel)
        {
            //交易資料 AES 加解密
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            SendToNewebPayOut outModel = new SendToNewebPayOut();

            // 藍新金流線上付款

            //交易欄位
            List<KeyValuePair<string, string>> TradeInfo = new List<KeyValuePair<string, string>>();
            // 商店代號
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantID", "MS351926985"));
            // 回傳格式
            TradeInfo.Add(new KeyValuePair<string, string>("RespondType", "String"));
            // TimeStamp
            TradeInfo.Add(new KeyValuePair<string, string>("TimeStamp", DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).ToUnixTimeSeconds().ToString()));
            // 串接程式版本
            TradeInfo.Add(new KeyValuePair<string, string>("Version", "2.0"));
            // 商店訂單編號
            TradeInfo.Add(new KeyValuePair<string, string>("MerchantOrderNo", inModel.MerchantOrderNo));
            // 訂單金額
            TradeInfo.Add(new KeyValuePair<string, string>("Amt", inModel.Amt));
            // 商品資訊
            TradeInfo.Add(new KeyValuePair<string, string>("ItemDesc", inModel.ItemDesc));
            // 繳費有效期限(適用於非即時交易)
            TradeInfo.Add(new KeyValuePair<string, string>("ExpireDate", inModel.ExpireDate));
            // 支付完成返回商店網址
            TradeInfo.Add(new KeyValuePair<string, string>("ReturnURL", inModel.ReturnURL));
            // 支付通知網址
            TradeInfo.Add(new KeyValuePair<string, string>("NotifyURL", inModel.NotifyURL));
            // 商店取號網址
            TradeInfo.Add(new KeyValuePair<string, string>("CustomerURL", inModel.CustomerURL));
            // 支付取消返回商店網址
            TradeInfo.Add(new KeyValuePair<string, string>("ClientBackURL", inModel.ClientBackURL));
            // 付款人電子信箱
            TradeInfo.Add(new KeyValuePair<string, string>("Email", inModel.Email));
            // 付款人電子信箱 是否開放修改(1=可修改 0=不可修改)
            TradeInfo.Add(new KeyValuePair<string, string>("EmailModify", "0"));

            //信用卡 付款
            if (inModel.ChannelID == "CREDIT")
            {
                TradeInfo.Add(new KeyValuePair<string, string>("CREDIT", "1"));
            }
            //ATM 付款
            if (inModel.ChannelID == "VACC")
            {
                TradeInfo.Add(new KeyValuePair<string, string>("VACC", "1"));
            }
            string TradeInfoParam = string.Join("&", TradeInfo.Select(x => $"{x.Key}={x.Value}"));

            // API 傳送欄位
            // 商店代號
            outModel.MerchantID = "MS351926985";
            // 串接程式版本
            outModel.Version = "2.0";
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoEncrypt = EncryptAESHex(TradeInfoParam, HashKey, HashIV);
            outModel.TradeInfo = TradeInfoEncrypt;
            //交易資料 SHA256 加密
            outModel.TradeSha = EncryptSHA256($"HashKey={HashKey}&{TradeInfoEncrypt}&HashIV={HashIV}");

            return Json(outModel);
        }

        /// <summary>
        /// 加密後再轉 16 進制字串
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後的字串</returns>
        public string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var encryptValue = EncryptAES(Encoding.UTF8.GetBytes(source), cryptoKey, cryptoIV);

                if (encryptValue != null)
                {
                    result = BitConverter.ToString(encryptValue)?.Replace("-", string.Empty)?.ToLower();
                }
            }

            return result;
        }

        /// <summary>
        /// 字串加密AES
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後字串</returns>
        public byte[] EncryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }

        /// <summary>
        /// 字串加密SHA256
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }
            return result;
        }

        /// <summary>
        /// 16 進制字串解密
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後的字串</returns>
        public string DecryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                // 將 16 進制字串 轉為 byte[] 後
                byte[] sourceBytes = ToByteArray(source);

                if (sourceBytes != null)
                {
                    // 使用金鑰解密後，轉回 加密前 value
                    result = Encoding.UTF8.GetString(DecryptAES(sourceBytes, cryptoKey, cryptoIV)).Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
        /// <returns></returns>
        public byte[] ToByteArray(string source)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var outputLength = source.Length / 2;
                var output = new byte[outputLength];

                for (var i = 0; i < outputLength; i++)
                {
                    output[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
                }
                result = output;
            }

            return result;
        }

        /// <summary>
        /// 字串解密AES
        /// </summary>
        /// <param name="source">解密前字串</param>
        /// <param name="cryptoKey">解密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後字串</returns>
        public static byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                // 智付通無法直接用PaddingMode.PKCS7，會跳"填補無效，而且無法移除。"
                // 所以改為PaddingMode.None並搭配RemovePKCS7Padding
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] data = decryptor.TransformFinalBlock(source, 0, source.Length);
                    int iLength = data[data.Length - 1];
                    var output = new byte[data.Length - iLength];
                    Buffer.BlockCopy(data, 0, output, 0, output.Length);
                    return output;
                }
            }
        }
    }
}
