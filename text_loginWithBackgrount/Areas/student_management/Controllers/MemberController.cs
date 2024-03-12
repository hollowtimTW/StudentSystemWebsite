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
                TempData["ErrorMessage"] = "註冊失敗，請檢查您的輸入。";
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

        [HttpGet]
        public IActionResult GetStudentEditPartialView(int id)
        {

            var student = _studentContext.T會員學生s.Find(id);
            MemberEditViewModel memberEditViewModel = new MemberEditViewModel
            {
                學生id = student.學生id,
                姓名 = student.姓名,
                性別 = student.性別,
                身分證字號 = student.身分證字號,
                信箱 = student.信箱,
                手機 = student.手機,
                地址 = student.地址,
                生日 = student.生日,
                學校 = student.學校,
                科系 = student.科系,
                學位 = student.學位,
                畢肄 = student.畢肄,
                鎖定 = student.鎖定

            };
            //只給_StudentPartialView的話
            //只會去這邊找/Areas/student_management/Views/Member/_StudentPartialView
            return PartialView("~/Areas/student_management/Views/PartialView/_StudentPartialView.cshtml", memberEditViewModel);
        }

        // PUT: api/Member/5
        [HttpPost]
        public IActionResult EditStudent(MemberEditViewModel editMember)
        {
            if (!ModelState.IsValid)
            {
                TempData["memberMessage"] = "修改失敗，請檢查項目數值";
                return RedirectToAction(nameof(GetAllStudent));
            }


            var user = _studentContext.T會員學生s.Find(editMember.學生id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {

                try
                {
                    byte[]? _filebyte = null;
                    if (editMember.圖片 != null && editMember.圖片.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            // 將圖片數據讀取到內存流中
                            editMember.圖片.CopyTo(memoryStream);
                            // 將內存流中的數據轉換為 byte 陣列
                            _filebyte = memoryStream.ToArray();
                        }
                    }

                    user.姓名 = editMember.姓名;
                    user.性別 = editMember.性別;
                    user.身分證字號 = editMember.身分證字號;
                    user.信箱 = editMember.信箱;
                    user.手機 = editMember.手機;
                    user.地址 = editMember.地址;
                    user.圖片 = _filebyte;
                    user.生日 = editMember.生日;
                    user.修改日期 = DateTime.Now;
                    //existingMember.狀態 = updatedMember.狀態;
                    user.鎖定 = editMember.鎖定;
                    user.學校 = editMember.學校;
                    user.科系 = editMember.科系;
                    user.學位 = editMember.學位;
                    user.畢肄 = editMember.畢肄;

                    _studentContext.Update(user);
                    _studentContext.SaveChanges();
                    TempData["memberMessage"] = "修改成功";
                    return RedirectToAction(nameof(GetAllStudent));

                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

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
