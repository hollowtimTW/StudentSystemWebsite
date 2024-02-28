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
        /// 老師登入頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
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
                //return RedirectToAction("Index", "StoreBackground");
                return Ok();
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
        /// 老師登入授權帳號驗證
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(LoginPost value)
        {
            var user = (from a in _dbStudentSystemContext.T會員老師s
                        where a.信箱 == value.Account && a.密碼 == value.Password
                        select a).SingleOrDefault();
            if (user == null)
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
