using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;

namespace text_loginWithBackgrount.Areas.class_discuss.Controllers
{
    public class WebApiController : Controller
    {
        private readonly studentContext _DBContext;

        public WebApiController(studentContext myDBContext)
        {
            _DBContext = myDBContext;
        }
        public IActionResult Subject()
        {
            var result = _DBContext.T討論看板s.
               Select(a => new
               {
                   a.看板id,
                   a.名稱
               });
            return Json(result);
        }
        public IActionResult artcard(int id)
        {
            var result = from item in _DBContext.T討論文章s
                         join a in _DBContext.T討論子版s on item.看板id equals a.看板id
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         where item.看板id == id
                         select new
                         {
                             s文章id = item.文章id,
                             s分類 = a.名稱,
                             s標題 = item.標題,
                             s內容 = item.內容,
                             s作者 = b.姓名,
                             s日期 = item.時間
                         };
            return Json(result);
        }

        [HttpPost("/saveData")]
        public IActionResult SaveData([FromBody] string model)
        {
            // 将数据保存到数据库
            _DBContext.T討論文章s.Add(new T討論文章
            {
                內容 = model,
                時間 = DateTime.Now
            });
            _DBContext.SaveChanges();

            return Ok();
        }
    }
}
