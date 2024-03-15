using Azure.AI.TextAnalytics;
using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Class_system_Backstage_pj.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThirdParty.Json.LitJson;
using SkiaSharp;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers.Backstage
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("job_vacancy")]
    [Route("/job_vacancy/Vacancy/{Action=Index}")]
    public class VacancyController : Controller
    {
        private readonly studentContext _context;

        public VacancyController(studentContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ProduceJobKeyword()
        {

            var all = await _context.T工作職缺資料s
                        .Where(data => data.F刪除狀態 == "0" && data.F職缺狀態 == "公開")
                        .Include(data => data.F公司)
                        .ToListAsync();

            foreach (var item in all)
            {
                //創建匿名類型
                var jsonData = new
                {
                    公司簡介 = item.F公司.F公司簡介,
                    //職務名稱 = item.F職務名稱,
                    //薪水待遇 = item.F薪水待遇,
                    //工作性質 = item.F工作性質,
                    //工作地點 = item.F工作地點,
                    //工作時段 = item.F工作時段,
                    //輪班需求 = item.F輪班需求,
                    工作內容 = item.F工作內容,
                    //學歷要求 = item.F學歷要求,
                    語文條件 = item.F語文條件,
                    工作技能 = item.F工作技能,
                    其他條件 = item.F其他條件
                };

                // 將 JSON 物件序列化為 JSON 字串
                var jsonString = JsonConvert.SerializeObject(jsonData);

                //關鍵字擷取服務
                AzureKeyCredential credentials = new AzureKeyCredential("b8c172267f6b4d8399ca5953a265126e");
                Uri endpoint = new Uri("https://analysisrecommendation.cognitiveservices.azure.com/");

                TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);
                Response<KeyPhraseCollection> response = client.ExtractKeyPhrases(jsonString);
                KeyPhraseCollection resumeKeyPhrases = response.Value;

                //將集合結合為字串
                string resumeKeyString = string.Join(", ", resumeKeyPhrases);

                //存回資料庫
                item.F關鍵字 = resumeKeyString;
            }

            try
            {
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "處理公司關鍵字資料完成！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }


    }
}
