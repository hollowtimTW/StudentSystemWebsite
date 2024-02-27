using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.class_discuss.Controllers
{
    public class ApiController : Controller
    {
        private readonly studentContext _DBContext;

        public ApiController(studentContext myDBContext)
        {
            _DBContext = myDBContext;
        }
        public IActionResult subject()
        { 
            var result = _DBContext.T討論看板s.
               Select(a => new
               {
                   a.看板id,
                   a.名稱,
                   封面圖 = a.封面圖
               });
            return Json(result);
        }
        public IActionResult type(int id)
        {
            var result = _DBContext.T討論子版s.
                Where(a => a.看板id == id).
               Select(a => new
               {
                   a.子版id,
                   a.名稱,
                   a.看板id
               });
            return Json(result);
        }
        public IActionResult article()
        {
            var result = _DBContext.T討論文章s.
               Select(a => new
               {
                   a.文章id,
                   作者 = a.學生id,
                   a.標題,
                   a.內容,
                   a.時間,
                   a.觀看數
               });
            return Json(result);
        }
        public IActionResult message(int id)
        {
            var result = _DBContext.T討論留言s.
                Where(a => a.文章id == id).
               Select(a => new
               {
                   a.留言id,
                   作者 = a.學生id,
                   a.內容,
                   a.時間
               });
            return Json(result);
        }
    }
}
