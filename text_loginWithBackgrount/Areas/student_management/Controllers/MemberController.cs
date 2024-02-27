using Class_system_Backstage_pj.Areas.student_management.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace Class_system_Backstage_pj.Areas.student_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    [Area("student_management")]
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> _logger;
        private readonly studentContext _studentContext;

        public MemberController(ILogger<MemberController> logger, studentContext studentContext)
        {
            _logger = logger;
            _studentContext = studentContext;
        }


        public IActionResult Index()
        {
            return View();
        }




        // GET: api/Member
        [HttpGet]
        public IActionResult GetAllStudent()
        {
            //return Ok(members);
            var members = _studentContext.T會員學生s.ToList();
            return View(members);
        }

        public IActionResult GetAllStudentJson()
        {
            var members = _studentContext.T會員學生s;
            return Json(members);

        }


        public IActionResult CheckStudentEmail(T會員學生 user)
        {
            bool isDuplicate = _studentContext.T會員學生s.Any(p => p.信箱 == user.信箱);
            return Content($"{isDuplicate}", "text/plain", Encoding.UTF8);
        }


        // GET: api/Member/5
        //[HttpGet("{id}")]
        //public IActionResult GetStudentById(int id)
        //{
        //    var member = _studentContext.T會員學生s.Find(id);
        //    if (member == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(member);
        //}

        [HttpGet]
        public IActionResult GetStudentEditPartialView(int id)
        {
            
            var member = _studentContext.T會員學生s.Find(id);
            //只給_StudentPartialView的話
            //只會去這邊找/Areas/student_management/Views/Member/_StudentPartialView
            return PartialView("~/Areas/student_management/Views/PartialView/_StudentPartialView.cshtml", member);
        }


        public IActionResult RegisterStudent()
        {
            ViewBag.SuccessMessage = "";
            ViewBag.ErrorMessage = "";
            return View();
        }

        // POST: api/Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterStudent(MemberRegisterViewModel memberRegisterinfo)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "註冊失敗，請檢查您的輸入。";
                return View(memberRegisterinfo);
            }

            byte[]? _filebyte = null;

            if (memberRegisterinfo.圖片 != null && memberRegisterinfo.圖片.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // 將圖片數據讀取到內存流中
                    memberRegisterinfo.圖片.CopyTo(memoryStream);

                    // 將內存流中的數據轉換為 byte 陣列
                    _filebyte = memoryStream.ToArray();
                }
            }

            string _salt = GenerateSalt();
            string _hashPassword = HashPassword(memberRegisterinfo.密碼, _salt);

            var memberEntity = new T會員學生
            {
                姓名 = memberRegisterinfo.姓名,
                性別 = memberRegisterinfo.性別,
                身分證字號 = memberRegisterinfo.身分證字號,
                信箱 = memberRegisterinfo.信箱,
                手機 = memberRegisterinfo.手機,
                地址 = memberRegisterinfo.地址,
                生日 = memberRegisterinfo.生日,
                學校 = memberRegisterinfo.學校,
                科系 = memberRegisterinfo.科系,
                學位 = memberRegisterinfo.學位,
                畢肄 = memberRegisterinfo.畢肄,
                圖片 = _filebyte,
                密碼 = _hashPassword,
                Salt = _salt,
                註冊日期 = DateTime.Now,
                修改日期 = DateTime.Now,
            };



            try
            {
                _studentContext.T會員學生s.Add(memberEntity);
                _studentContext.SaveChanges();
                TempData["SuccessMessage"] = memberRegisterinfo.姓名 + " 註冊成功。可進行下一位註冊。";
                return RedirectToAction("RegisterStudent");

                //ModelState.Clear(); //清空值
                //return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"註冊失敗：{ex.Message}";
                return View(memberRegisterinfo);
            }
        }

        //創造鹽 (亂碼)
        private string GenerateSalt()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return Convert.ToBase64String(salt);//轉成base64字串
            }
        }

        //創造Hash
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())// SHA-256 雜湊
            {
                byte[] combined = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // PUT: api/Member/5
        [HttpPost]
        public IActionResult UpdateStudent(int id, T會員學生 updatedMember)
        {

            var existingMember = _studentContext.T會員學生s.Find(id);
            if (existingMember == null)
            {
                return NotFound();
            }
            else {
                existingMember.姓名 = updatedMember.姓名;
                existingMember.性別 = updatedMember.性別;
                existingMember.身分證字號 = updatedMember.身分證字號;
                existingMember.信箱 = updatedMember.信箱;
                existingMember.手機 = updatedMember.手機;
                existingMember.地址 = updatedMember.地址;
                existingMember.圖片 = updatedMember.圖片;
                existingMember.生日 = updatedMember.生日;
                existingMember.修改日期 = DateTime.Now;
                //existingMember.狀態 = updatedMember.狀態;
                //existingMember.鎖定 = updatedMember.鎖定;
                existingMember.學校 = updatedMember.學校;
                existingMember.科系 = updatedMember.科系;
                existingMember.學位 = updatedMember.學位;
                existingMember.畢肄 = updatedMember.畢肄;
            }


            try
            {
                _studentContext.Update(existingMember);
                _studentContext.SaveChanges();
            }
            catch (Exception ex) 
            {

            }

            return Ok();
        }


        // DELETE: api/Member/5 先封印這方法
        //[HttpDelete("{id}")]
        //public IActionResult DeleteMember(int id)
        //{
        //    var member = _studentContext.T會員學生s.Find(id);
        //    if (member == null)
        //    {
        //        return NotFound();
        //    }

        //    _studentContext.T會員學生s.Remove(member);
        //    _studentContext.SaveChanges();

        //    return NoContent();
        //}



    }
}
