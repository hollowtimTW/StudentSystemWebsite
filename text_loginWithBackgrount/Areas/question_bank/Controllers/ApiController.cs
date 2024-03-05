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




	}
}
