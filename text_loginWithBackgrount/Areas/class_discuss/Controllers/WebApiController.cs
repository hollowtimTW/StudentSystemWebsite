using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.class_discuss.DTO;
using text_loginWithBackgrount.Areas.class_discuss.ViewModels;

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
        [HttpPost]
        public IActionResult ArtCard([FromBody] CardDTO _card)
        {
            var result = _card.TypeId == 0 ?
                         (from item in _DBContext.T討論文章s
                          join a in _DBContext.T討論子版s on item.子版id equals a.子版id
                          join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                          where item.看板id == _card.SubId && item.刪除 == "0"
                          select new ArtCardViewModel
                          {
                              文章id = item.文章id,
                              子版id = item.子版id,
                              分類 = a.名稱,
                              標題 = item.標題,
                              內容 = item.內容,
                              作者 = b.姓名,
                              日期 = item.時間,
                              頭像 = b.圖片 != null ? Convert.ToBase64String(b.圖片) : null
                          }) :
                         from item in _DBContext.T討論文章s
                         join a in _DBContext.T討論子版s on item.子版id equals a.子版id
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         where item.看板id == _card.SubId && item.刪除 == "0" && item.子版id == _card.TypeId
                         select new ArtCardViewModel
                         {
                             文章id = item.文章id,
                             子版id = item.子版id,
                             分類 = a.名稱,
                             標題 = item.標題,
                             內容 = item.內容,
                             作者 = b.姓名,
                             日期 = item.時間,
                             頭像 = b.圖片 != null ? Convert.ToBase64String(b.圖片) : null
                         };

            if (!string.IsNullOrEmpty(_card.Keyword))
            {
                result = result.Where(s => s.標題.Contains(_card.Keyword) || s.作者.Contains(_card.Keyword));
            }

            if (_card.TypeId != 0)
            {
                result = result.Where(s => s.子版id == _card.TypeId);
            }

            var type = from t in _DBContext.T討論子版s
                       where t.看板id == _card.SubId
                       select t;

            int totalCount = result.Count();
            int pageSize = _card.PageSize;
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int page = _card.Page ?? 1;
            result = result.Skip((page - 1) * pageSize).Take(pageSize);

            CardPageDTO cardPage = new CardPageDTO();
            cardPage.TotalPages = totalPages;
            cardPage.Result = result.ToList();
            cardPage.TypeResult = type.ToList();
            return Json(cardPage);
        }

        public IActionResult Type(int id)
        {
            var result = _DBContext.T討論子版s.
                Where(a => a.看板id == id).
                Select(a => new
                {
                    a.子版id,
                    a.名稱
                });
            return Json(result);
        }

        public IActionResult Article(int id)
        {
            var result = from item in _DBContext.T討論文章s
                         join a in _DBContext.T討論子版s on item.子版id equals a.子版id
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         where item.文章id == id && item.刪除 == "0"
                         select new
                         {
                             文章id = item.文章id,
                             分類 = a.名稱,
                             標題 = item.標題,
                             內容 = item.內容,
                             作者 = b.姓名,
                             日期 = item.時間
                         };
            return Json(result);
        }

        public IActionResult Writer(int id)
        {
            var result = from item in _DBContext.T討論文章s
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         where item.文章id == id && item.刪除 == "0"
                         select new
                         {
                             作者 = b.姓名,
                             頭像 = b.圖片 != null ? Convert.ToBase64String(b.圖片) : null,
                             b.信箱,
                             b.學校,
                             b.科系
                         };
            return Json(result);
        }

        [HttpPost]
        public IActionResult UserArtCard([FromBody] CardDTO _card)
        {
            var result = _card.TypeId == 0 ?
                         (from item in _DBContext.T討論文章s
                          join a in _DBContext.T討論子版s on item.子版id equals a.子版id
                          join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                          join c in _DBContext.T討論看板s on item.看板id equals c.看板id
                          where item.學生id == _card.UserId && item.刪除 == "0"
                          select new ArtCardViewModel
                          {
                              文章id = item.文章id,
                              子版id = item.子版id,
                              看板id = item.看板id,
                              看板 = c.名稱,
                              分類 = a.名稱,
                              標題 = item.標題,
                              內容 = item.內容,
                              日期 = item.時間
                          }) :
                         from item in _DBContext.T討論文章s
                         join a in _DBContext.T討論子版s on item.子版id equals a.子版id
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         join c in _DBContext.T討論看板s on item.看板id equals c.看板id
                         where item.學生id == _card.UserId && item.刪除 == "0" && item.子版id == _card.TypeId
                         select new ArtCardViewModel
                         {
                             文章id = item.文章id,
                             子版id = item.子版id,
                             看板id = item.看板id,
                             看板 = c.名稱,
                             分類 = a.名稱,
                             標題 = item.標題,
                             內容 = item.內容,
                             日期 = item.時間
                         };

            if (!string.IsNullOrEmpty(_card.Keyword))
            {
                result = result.Where(s => s.標題.Contains(_card.Keyword) || s.分類.Contains(_card.Keyword) || s.看板.Contains(_card.Keyword));
            }

            int totalCount = result.Count();
            int pageSize = _card.PageSize;
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int page = _card.Page ?? 1;
            result = result.Skip((page - 1) * pageSize).Take(pageSize);

            CardPageDTO cardPage = new CardPageDTO();
            cardPage.TotalPages = totalPages;
            cardPage.Result = result.ToList();
            return Json(cardPage);
        }
        [HttpPost]
        public IActionResult Message([FromBody] CardDTO _card)
        {
            var result = from item in _DBContext.T討論留言s
                         join b in _DBContext.T會員學生s on item.學生id equals b.學生id
                         where item.文章id == _card.ArtId && item.刪除 == "0"
                         select new MessageViewModel
                         {
                             文章id = item.文章id,
                             留言id = item.留言id,
                             內容 = item.內容,
                             時間 = item.時間,
                             作者 = b.姓名,
                             作者id = item.學生id,
                             頭像 = b.圖片 != null ? Convert.ToBase64String(b.圖片) : null
                         };

            int totalCount = result.Count();
            //int pageSize = _card.PageSize;
            //int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //int page = _card.Page ?? 1;
            //result = result.Skip((page - 1) * pageSize).Take(pageSize);

            MessageDTO message = new MessageDTO();
            message.TotalMessages = totalCount;
            //message.TotalPages = totalPages;
            message.Result = result.ToList();
            return Json(message);
        }
        [HttpGet("{mesid}")]
        public ActionResult<int> mesWriter(int mesid)
        {
            T討論留言? message = _DBContext.T討論留言s.FirstOrDefault(a => a.留言id == mesid);

            return message.學生id;
        }
    }
}
