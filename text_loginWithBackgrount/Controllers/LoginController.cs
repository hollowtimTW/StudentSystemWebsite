using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using text_loginWithBackgrount.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using text_loginWithBackgrount.Data.LoginPost;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;
using text_loginWithBackgrount.ViewModels;
using text_loginWithBackgrount.Data.Encryptor;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace text_loginWithBackgrount.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly studentContext _dbStudentSystemContext;
        private readonly IConfiguration _configuration;
        public LoginController(studentContext dbStudentSystemContext, IConfiguration configuration)
        {
            _dbStudentSystemContext = dbStudentSystemContext;
            _configuration = configuration;
        }

        /// <summary>
        /// 店家登入頁
        /// </summary>
        /// <returns></returns>
        public IActionResult StoreIndex()
        {
            return View();
        }

        /// <summary>
        /// 學生登入頁
        /// </summary>
        /// <returns></returns>
        public IActionResult StudentIndex()
        {
            return View();
        }

        /// <summary>
        /// 學生註冊頁View
        /// </summary>
        /// <returns></returns>
        public IActionResult StudentRegister()
        {
            return View();
        }


        /// <summary>
        /// 學生註冊頁
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StudentRegister(MemberViewModel memberRegisterinfo)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.ErrorMessage = "註冊失敗，請檢查您的輸入。";
                //return View(memberRegisterinfo);
                return BadRequest(new { errorMessage = "註冊失敗，請檢查您的輸入。" });
            }

            if (memberRegisterinfo.信箱 == null || memberRegisterinfo.信箱 == "")
            {
                return BadRequest(new { errorMessage = "請填入正確信箱。" });
            }

            if (CheckEmailAvailability(memberRegisterinfo.信箱))
            {
                return BadRequest(new { errorMessage = "信箱重複。" });
            }

            //先不處理圖片
            //byte[]? _filebyte = null;
            //if (memberRegisterinfo.圖片 != null && memberRegisterinfo.圖片.Length > 0)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        // 將圖片數據讀取到內存流中
            //        memberRegisterinfo.圖片.CopyTo(memoryStream);
            //        // 將內存流中的數據轉換為 byte 陣列
            //        _filebyte = memoryStream.ToArray();
            //    }
            //}

            IRegistrationEncryptor encryptor = EncryptorFactory.CreateEncryptor();
            (string _hashPassword, string _salt) = encryptor.EncryptPassword(memberRegisterinfo.密碼);

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
                圖片 = null,
                密碼 = _hashPassword,
                Salt = _salt,
                註冊日期 = DateTime.Now,
                修改日期 = DateTime.Now,
            };

            _dbStudentSystemContext.T會員學生s.Add(memberEntity);
            await _dbStudentSystemContext.SaveChangesAsync();

            return Ok(new { successMessage = "註冊成功！" });
        }


        /// <summary>
        /// 學生確認重複email
        /// </summary>
        /// <returns></returns>        
        public bool CheckEmailAvailability(string email)
        {
            var user = _dbStudentSystemContext.T會員學生s.FirstOrDefault(a => a.信箱 == email);
            return (user != null);
        }


        /// <summary>
        /// 執行授權學生登入，回傳頁寫在StudentIndex view的js中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult StudentIndex(LoginPost value)
        {
            var user = (from a in _dbStudentSystemContext.T會員學生s
                        where a.信箱 == value.Account && a.密碼 == value.Password
                        select a).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                var claims = new List<Claim>
                {
                   new Claim(ClaimTypes.Name, user.姓名),
                   new Claim("FullName", user.姓名),
                   new Claim("StudentId", user.學生id.ToString()),
                   new Claim(ClaimTypes.Role,"student")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Ok();
            }
        }


        /// <summary>
        /// 執行授權店家登入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult StoreIndex(LoginPost value)
        {
            var user = (from a in _dbStudentSystemContext.T訂餐店家資料表s
                        where a.電子信箱 == value.Account && a.密碼 == value.Password
                        select a).FirstOrDefault();
            if (user == null)
            {
                TempData["key"] = "alert('帳號密碼錯誤')";
                return View();
            }
            else
            {
                var claims = new List<Claim>
                {
                   new Claim(ClaimTypes.Name, user.店家名稱),
                   new Claim("FullName", user.店家名稱),
                   new Claim("teacherID", user.店家id.ToString()),
                   new Claim(ClaimTypes.Role,"store")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "StoreBackground");
                //return Ok();
            }
        }
        /// <summary>
        /// 老師重置密碼頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult resetPassword()
        {
            return View();
        }

        /// <summary>
        /// 老師登入頁
        /// </summary>
        /// <returns></returns>
        public IActionResult TeacherIndex()
        {
            return View();
        }


        /// <summary>
        /// 老師註冊頁
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TeacherRegister(MemberViewModel memberRegisterinfo)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(new { errorMessage = "註冊失敗，請檢查您的輸入。" });
                TempData["key"] = "alert('註冊失敗，請檢查您的輸入。')";
                return View("TeacherIndex");
            }

            if (memberRegisterinfo.信箱 == null || memberRegisterinfo.信箱 == "")
            {
                //return BadRequest(new { errorMessage = "請填入正確信箱。" });
                TempData["key"] = "alert('請填入正確信箱。')";
                return View("TeacherIndex");
            }

            if (CheckEmailAvailability(memberRegisterinfo.信箱))
            {
                //return BadRequest(new { errorMessage = "信箱重複。" });
                TempData["key"] = "alert('信箱重複。')";
                return View("TeacherIndex");
            }

            //先不處理圖片
            //byte[]? _filebyte = null;
            //if (memberRegisterinfo.圖片 != null && memberRegisterinfo.圖片.Length > 0)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        // 將圖片數據讀取到內存流中
            //        memberRegisterinfo.圖片.CopyTo(memoryStream);
            //        // 將內存流中的數據轉換為 byte 陣列
            //        _filebyte = memoryStream.ToArray();
            //    }
            //}

            IRegistrationEncryptor encryptor = EncryptorFactory.CreateEncryptor();
            (string _hashPassword, string _salt) = encryptor.EncryptPassword(memberRegisterinfo.密碼);

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
                圖片 = null,
                密碼 = _hashPassword,
                Salt = _salt,
                註冊日期 = DateTime.Now,
                修改日期 = DateTime.Now,
            };

            _dbStudentSystemContext.T會員學生s.Add(memberEntity);
            await _dbStudentSystemContext.SaveChangesAsync();

            //return Ok(new { successMessage = "註冊成功！" });
            TempData["key"] = "alert('註冊成功！')";
            return View("TeacherIndex");
        }


        /// <summary>
        /// 老師確認重複email
        /// </summary>
        /// <returns>有信箱是true</returns>        
        public bool TeacherCheckEmailAvailability(string email)
        {
            var user = _dbStudentSystemContext.T會員老師s.FirstOrDefault(a => a.信箱 == email);
            return (user != null);
        }

        /// <summary>
        /// 老師登入授權帳號驗證
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TeacherIndex(LoginPost value)
        {
            //var user = (from a in _dbStudentSystemContext.T會員老師s
            //            where a.信箱 == value.Account && a.密碼 == value.Password
            //            select a).SingleOrDefault();
            var user = _dbStudentSystemContext.T會員老師s.SingleOrDefault(a => a.信箱 == value.Account);

            if (user == null || !VerifyPassword(user.密碼, user.Salt, value.Password))
            {
                TempData["key"] = "alert('帳號密碼錯誤')";
                return View();
            }
            else
            {
                var claims = new List<Claim>
                {
                   new Claim(ClaimTypes.Name, user.姓名),
                   new Claim("FullName", user.姓名),
                   new Claim("teacherID", user.老師id.ToString()),
                   new Claim(ClaimTypes.Role,"teacher")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "SystemBackground");
            }
        }

        /// <summary>
        /// 密碼驗證
        /// </summary>
        /// <returns>正確是回傳 true。</returns>
        private bool VerifyPassword(string dbPassword, string dbSalt, string inputPassword)
        {
            IRegistrationEncryptor encryptor = EncryptorFactory.CreateEncryptor();
            string hashedPassword = encryptor.HashPassword(inputPassword, dbSalt);
            return dbPassword == hashedPassword;
        }

        public IActionResult StudentForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StudentForgetPassword(string email)
        {

            var user = _dbStudentSystemContext.T會員學生s.SingleOrDefault(a => a.信箱 == email);
            if (user == null)
            {
                return BadRequest(new { errorMessage = "沒有這個信箱。" });
            }
            else
            {
                //HttpContext.Session.SetString("teacherID", (user.學生id).ToString());
                //設定使用者資訊
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.信箱.ToString()),
                    new Claim(ClaimTypes.Role,"teacher"),

                };

                //取出appsettings.json裡的KEY處理
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

                //設定jwt相關資訊
                var jwt = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                //產生JWT Token
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                //產生網址
                string applicationUrl = "https://"+HttpContext.Request.Host.Value;
                applicationUrl += "/Login/StudentResetPassword?token=" + token;

                //var jwtToken = JwtTokenParser.ParseJwtToken(token);
                //var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                //var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                //var expirationTime = jwtToken.ValidTo;

                try
                {
                    if (false)
                    {
                        var mail = new MailMessage();
                        mail.From = new MailAddress("rizeno3260@gmail.com");
                        mail.Subject = "學生平台認證信";
                        mail.Body = "<html>\r\n<body>\r\n<p>這是你的忘記密碼連結：</p>\r\n<a href='" +
                            applicationUrl
                            + "'>點擊使用TOKEN前往重設密碼頁</a>\r\n</body>\r\n</html>\r\n";
                        mail.To.Add(user.信箱.ToString());
                        mail.IsBodyHtml = true;
                        //--------獲得smtp認證
                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                        smtpClient.Port = 587;
                        smtpClient.UseDefaultCredentials = false; //是用來指示是否使用預設的網路憑證
                        smtpClient.Credentials = new NetworkCredential("rizeno3260@gmail.com", "tpsd gliw vmno fpdv");
                        smtpClient.EnableSsl = true; /// 如果你的 SMTP 伺服器支援 SSL，可以啟用它
                        smtpClient.Send(mail);
                    }
                    return Ok(new { successMessage = "已發送驗證信，請至信箱收取！", linkContent = applicationUrl });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { errorMessage = "發送驗證信失敗！" });
                }


            }
        }




        public IActionResult StudentResetPassword()
        {
            return View();
        }


        /// <summary>
        /// 帳號登出
        /// </summary>
        /// <returns></returns>
        public IActionResult logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Template");
        }

        /// <summary>
        /// 執行忘記密碼的JWT認證
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public string teacherForgretPassword(LoginPost value)
        {
            var user = (from a in _dbStudentSystemContext.T會員老師s
                        where a.信箱 == value.Account
                        select a).SingleOrDefault();
            if (user == null)
            {
                return "錯誤訊息";
            }
            else
            {
                HttpContext.Session.SetString("teacherID", (user.老師id).ToString());
                //設定使用者資訊
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.信箱.ToString()),

                };

                //取出appsettings.json裡的KEY處理
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

                //設定jwt相關資訊
                var jwt = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                //產生JWT Token
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                //回傳JWT Token給認證通過的使用者
                return token;
            }
        }
        /// <summary>
        /// 執行確認授權還沒到期，完成修改密碼
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult reseatTeacherPassword(LoginPost value)
        {
            string? verify = HttpContext.Session.GetString("teacherID");
            if (!string.IsNullOrEmpty(verify))
            {
                int id = Convert.ToInt32(verify);
                var storeUser = _dbStudentSystemContext.T會員老師s.FirstOrDefault(a => a.老師id == id);
                if (storeUser != null)
                {
                    storeUser.密碼 = value.Password;
                    _dbStudentSystemContext.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

    }
}
