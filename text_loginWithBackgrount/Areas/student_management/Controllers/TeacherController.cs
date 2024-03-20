using Class_system_Backstage_pj.Areas.student_management.ViewModels;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace text_loginWithBackgrount.Areas.student_management.Controllers
{

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher, admin")]
    [Area("student_management")]
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly studentContext _studentContext;

        public TeacherController(ILogger<TeacherController> logger, studentContext studentContext)
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
        public IActionResult GetAllTeacher()
        {
            //return Ok(members);
            var teacher = _studentContext.T會員老師s.ToList();
            return View(teacher);
        }

        public IActionResult GetAllTeacherJson()
        {
            var teacher = _studentContext.T會員老師s;
            return Json(teacher);

        }


        public IActionResult CheckTeacherEmail(T會員老師 user)
        {
            bool isDuplicate = _studentContext.T會員老師s.Any(p => p.信箱 == user.信箱);
            return Content($"{isDuplicate}", "text/plain", Encoding.UTF8);
        }


        // GET: api/Member/5
        //[HttpGet("{id}")]
        //public IActionResult GetStudentById(int id)
        //{
        //    var member = _studentContext.T會員老師s.Find(id);
        //    if (member == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(member);
        //}




        public IActionResult RegisterTeacher()
        {
            ViewBag.SuccessMessage = "";
            ViewBag.ErrorMessage = "";
            return View();
        }

        // POST: api/Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterTeacher(TeacherRegisterViewModel teacherRegisterinfo)
        {

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "註冊失敗，請檢查您的輸入。";
                return View(teacherRegisterinfo);
            }

            byte[]? _filebyte = null;

            if (teacherRegisterinfo.圖片 != null && teacherRegisterinfo.圖片.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // 將圖片數據讀取到內存流中
                    teacherRegisterinfo.圖片.CopyTo(memoryStream);

                    // 將內存流中的數據轉換為 byte 陣列
                    _filebyte = memoryStream.ToArray();
                }
            }

            string _salt = GenerateSalt();
            string _hashPassword = HashPassword(teacherRegisterinfo.密碼, _salt);

            var teacherEntity = new T會員老師
            {
                姓名 = teacherRegisterinfo.姓名,
                性別 = teacherRegisterinfo.性別,
                身分證字號 = teacherRegisterinfo.身分證字號,
                信箱 = teacherRegisterinfo.信箱,
                手機 = teacherRegisterinfo.手機,
                地址 = teacherRegisterinfo.地址,
                生日 = teacherRegisterinfo.生日,
                學校 = teacherRegisterinfo.學校,
                科系 = teacherRegisterinfo.科系,
                學位 = teacherRegisterinfo.學位,
                畢肄 = teacherRegisterinfo.畢肄,
                圖片 = _filebyte,
                密碼 = _hashPassword,
                Salt = _salt,
                註冊日期 = DateTime.Now,
                修改日期 = DateTime.Now,
            };



            try
            {
                _studentContext.T會員老師s.Add(teacherEntity);
                _studentContext.SaveChanges();
                TempData["SuccessMessage"] = teacherRegisterinfo.姓名 + " 註冊成功。可進行下一位註冊。";
                return RedirectToAction("RegisterTeacher");

                //ModelState.Clear(); //清空值
                //return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"註冊失敗：{ex.Message}";
                return View(teacherRegisterinfo);
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
        public IActionResult GetTeacherEditPartialView(int id)
        {

            var teacher = _studentContext.T會員老師s.Find(id);
            TeacherEditViewModel teacherEditViewModel = new TeacherEditViewModel
            {
                老師id = teacher.老師id,
                姓名 = teacher.姓名,
                性別 = teacher.性別,
                身分證字號 = teacher.身分證字號,
                信箱 = teacher.信箱,
                手機 = teacher.手機,
                地址 = teacher.地址,
                生日 = teacher.生日,
                學校 = teacher.學校,
                科系 = teacher.科系,
                學位 = teacher.學位,
                畢肄 = teacher.畢肄,
                鎖定 = teacher.鎖定

            };

            return PartialView("~/Areas/student_management/Views/PartialView/_TeacherPartialView.cshtml", teacherEditViewModel);
        }

        // PUT: api/Member/5
        [HttpPost]
        public IActionResult EditTeacher(TeacherEditViewModel editinfo)
        {
            if (!ModelState.IsValid)
            {
                TempData["memberMessage"] = "修改失敗，請檢查項目數值";
                return RedirectToAction(nameof(GetAllTeacher));
            }


            var user = _studentContext.T會員老師s.Find(editinfo.老師id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {

                try
                {
                    byte[]? _filebyte = null;
                    if (editinfo.圖片 != null && editinfo.圖片.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            // 將圖片數據讀取到內存流中
                            editinfo.圖片.CopyTo(memoryStream);
                            // 將內存流中的數據轉換為 byte 陣列
                            _filebyte = memoryStream.ToArray();
                        }
                    }

                    user.姓名 = editinfo.姓名;
                    user.性別 = editinfo.性別;
                    user.身分證字號 = editinfo.身分證字號;
                    user.信箱 = editinfo.信箱;
                    user.手機 = editinfo.手機;
                    user.地址 = editinfo.地址;
                    user.圖片 = _filebyte;
                    user.生日 = editinfo.生日;
                    user.修改日期 = DateTime.Now;
                    //existingMember.狀態 = updatedMember.狀態;
                    user.鎖定 = editinfo.鎖定;
                    user.學校 = editinfo.學校;
                    user.科系 = editinfo.科系;
                    user.學位 = editinfo.學位;
                    user.畢肄 = editinfo.畢肄;

                    _studentContext.Update(user);
                    _studentContext.SaveChanges();
                    TempData["memberMessage"] = "修改成功";
                    return RedirectToAction(nameof(GetAllTeacher));

                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

        }



    }

}
