using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using text_loginWithBackgrount.Data.Encryptor;
using text_loginWithBackgrount.ViewModels;

namespace text_loginWithBackgrount.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")]
    public class StudentController : Controller
    {
        private readonly studentContext _dbStudentSystemContext;

        public StudentController(studentContext dbStudentSystemContext)
        {
            _dbStudentSystemContext = dbStudentSystemContext;
        }

        public IActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                // 獲取當前登入用戶的識別資訊
                var studentId = User.FindFirst("StudentId")?.Value;
                var user = _dbStudentSystemContext.T會員學生s.SingleOrDefault(a => a.學生id.ToString() == studentId);

                if (user != null)
                {
                    //取出需要的部分
                    MemberViewModel model = new MemberViewModel
                    {
                        姓名 = user.姓名,
                        性別 = user.性別,
                        身分證字號 = user.身分證字號,
                        信箱 = user.信箱,
                        手機 = user.手機,
                        地址 = user.地址,
                        生日 = user.生日,
                        學校 = user.學校,
                        科系 = user.科系,
                        學位 = user.學位,
                        畢肄 = user.畢肄,
                        修改日期 = user.修改日期
                    };

                    return View(model);
                }

                return View();
            }
            return View();
        }


        [HttpPost]
        public IActionResult StudentEdit(MemberEditViewModel MemberEditInfo)
        {

            var studentId = User.FindFirst("StudentId")?.Value;
            var user = _dbStudentSystemContext.T會員學生s.SingleOrDefault(a => a.學生id.ToString() == studentId);


            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
    .SelectMany(v => v.Errors)
    .Select(e => e.ErrorMessage)
    .ToList();
                return BadRequest(new { errorMessage = "修改失敗，請檢查您的輸入。" });
            }

            if (MemberEditInfo.信箱 == null || MemberEditInfo.信箱 == "")
            {
                return BadRequest(new { errorMessage = "請填入正確信箱。" });
            }

            //如果信箱不等於現在的信箱，就要去找自己以外的重複
            if (CheckEmailAvailability(MemberEditInfo.信箱, user.信箱))
            {
                return BadRequest(new { errorMessage = "信箱重複。" });
            }



            byte[]? _filebyte = null;
            if (MemberEditInfo.圖片 != null && MemberEditInfo.圖片.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // 將圖片數據讀取到內存流中
                    MemberEditInfo.圖片.CopyTo(memoryStream);
                    // 將內存流中的數據轉換為 byte 陣列
                    _filebyte = memoryStream.ToArray();
                }
            }

            //這邊不改密碼
            //IRegistrationEncryptor encryptor = EncryptorFactory.CreateEncryptor();
            //(string _hashPassword, string _salt) = encryptor.EncryptPassword(MemberEditInfo.密碼);

            if (user == null)
            {
                return BadRequest(new { errorMessage = "查無此人。" });
            }
            else
            {

                user.姓名 = MemberEditInfo.姓名;
                user.性別 = MemberEditInfo.性別;

                user.身分證字號 = MemberEditInfo.身分證字號;
                user.信箱 = MemberEditInfo.信箱;
                user.手機 = MemberEditInfo.手機;
                user.地址 = MemberEditInfo.地址;
                user.圖片 = _filebyte;
                user.生日 = MemberEditInfo.生日;
                user.學校 = MemberEditInfo.學校;
                user.科系 = MemberEditInfo.科系;
                user.學位 = MemberEditInfo.學位;
                user.畢肄 = MemberEditInfo.畢肄;
                user.修改日期 = DateTime.Now;

                _dbStudentSystemContext.SaveChanges();
            }

            return Ok();
            //return Ok(new { successMessage = "修改成功！" });
        }

        /// <summary>
        /// 學生確認重複email
        /// </summary>
        /// <returns></returns>        
        public bool CheckEmailAvailability(string email, string originalEmail = null)
        {
            if (originalEmail == null)
            {
                var user = _dbStudentSystemContext.T會員學生s.FirstOrDefault(a => a.信箱 == email);
                return (user != null);
            }
            else
            {
                var user = _dbStudentSystemContext.T會員學生s.FirstOrDefault(a => a.信箱 == email && a.信箱 != originalEmail);
                return (user != null);
            }
        }

    }
}
