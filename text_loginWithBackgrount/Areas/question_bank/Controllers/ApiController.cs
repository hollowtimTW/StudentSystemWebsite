using Class_system_Backstage_pj.Areas.question_bank.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.question_bank.ViewModels;

namespace Class_system_Backstage_pj.Areas.question_bank.Controllers
{
    [Route("Api/Chat/[action]")]
    public class ApiController : Controller
	{

		private readonly studentContext _context;
		public ApiController(studentContext context)
		{
			_context = context;
		}


        /// <summary>
        /// 班級學生名單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id?}")]
        public IActionResult GetAllStudents(int id)
		{

            var result = _context.T課程學生班級s
                .Where(p => p.學生id == id)
                .Join(_context.T課程學生班級s, p => p.班級id, q => q.班級id, (p, q) => new CStudent
                {
                    sId = q.學生id,
                    sName = q.學生.姓名,
                    sClassId = q.班級id,
                    sClassName = q.班級.班級名稱
                });

            return Json(result);
		}


		/// <summary>
		/// 班級學生名單(for teacher)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id?}")]
		public IActionResult GetAllStd(int id)
        {
            

            var result = _context.T課程學生班級s
				.Where(p => p.班級id == id)
				.Select(q => new CStudent
				{
					sId = q.學生id,
					sName = q.學生.姓名,
					sClassId = q.班級id,
					sClassName = q.班級.班級名稱
				});

			return Json(result);
		}





		/// <summary>
		/// 班級教師名單
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id?}")]
        public IActionResult GetAllTeachers(int id)
        {

            var classId = _context.T課程學生班級s
                .Where(p => p.學生id == id)
                .Select(p=>p.班級id)
                .FirstOrDefault();

            if (classId == default(int))
            {
                return BadRequest("查無教師");
            }

            var result = _context.T課程班級科目s
                .Where(p => p.班級id == classId)
                .Select(p => new CTeacher
                {
                    tId = p.老師id,
                    tName = p.老師.姓名,
                    tClassId = p.班級id,
                    tClassName = p.班級.班級名稱,
                    tSubId = p.科目id,
                    tSubName =p.科目.科目名稱,
                }).Distinct();

            return Json(result);
        }

    }
}
