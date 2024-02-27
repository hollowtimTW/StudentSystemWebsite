using Class_system_Backstage_pj.Areas.question_bank.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;

namespace Class_system_Backstage_pj.Areas.question_bank.Controllers
{
    [Route("{area}/{controller}/{action}/{id?}")]
    public class ApiController : Controller
	{

		private readonly studentContext _context;
		public ApiController(studentContext context)
		{
			_context = context;
		}



		public JsonResult GetExamList()
		{
			return Json(_context.T考試考試總表s.Select(x => new vExamInfo
			{
				fId = x.考試id,
				fExamName = x.考試名稱,
				fCourse = x.班級.科目.科目名稱,
				fCLass = x.班級.班級.班級名稱,
				fSTime = x.開始時間.Value.ToString("yyyy-MM-dd HH:mm:ss"),
				fETime = x.結束時間.Value.ToString("yyyy-MM-dd HH:mm:ss"),
				fPublish = x.發布者Navigation.姓名,
                fDescribe = (x.備註 != null && x.備註.Length > 10) ? x.備註.Substring(0, 10) + "..." : x.備註
            }));
		}
	}
}
