using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Class_system_Backstage_pj.Areas.video_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("video_management")]
    public class T影片VideoController : Controller
    {
        private readonly ILogger<T影片VideoController> _logger;
        private readonly studentContext _studentContext;
        public T影片VideoController(ILogger<T影片VideoController> logger, studentContext studentContext)
        {
            _logger = logger;
            _studentContext = studentContext;
        }
        

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult video()
        { return View(); }
        public IActionResult test() 
        { return View(); }
       


        [HttpPost]
        public IActionResult DeleteVideo(int id)
        {
            try
            {
                // 根據 id 查找要刪除的資料
                var video = _studentContext.T影片Videos.Find(id);

                if (video == null)
                {
                    return Json(new { success = false, message = "找不到指定的資料" });
                }

                // 執行刪除操作
                var dependentData = _studentContext.T影片OrderDetails.Where(x => x.FVideoId == id);
                _studentContext.T影片OrderDetails.RemoveRange(dependentData);
                _studentContext.SaveChanges();

                // 再刪除主要的資料
                _studentContext.T影片Videos.Remove(video);
                _studentContext.SaveChanges();

                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                // 處理例外情況
                return Json(new { success = false, message = "刪除失敗：" + ex.Message });
            }
        }
        public JsonResult DetailsJson()
        {
            return Json(_studentContext.T影片Videos);
        }

        [HttpPost]
        public JsonResult Create(T影片Video newVideo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 將新影片資料添加到資料庫
                    _studentContext.T影片Videos.Add(newVideo);
                    _studentContext.SaveChanges();

                    // 返回成功的 JSON 回應
                    return Json(new { success = true, message = "影片新增成功" });
                }

                // 如果模型驗證失敗，返回失敗的 JSON 回應
                return Json(new { success = false, message = "請檢查輸入的資料" });
            }
            catch (Exception ex)
            {
                // 返回例外錯誤的 JSON 回應
                return Json(new { success = false, message = $"新增影片時發生錯誤: {ex.Message}" });
            }
        }

        public JsonResult UpdateJson()
        {
            
            return Json(_studentContext.T影片Videos);
        }


        public JsonResult Indexjson()
        {
            return Json(_studentContext.T影片Videos);
        }
        
    }
    
}
