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
    }
}
