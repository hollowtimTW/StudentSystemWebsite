using Class_system_Backstage_pj.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using text_loginWithBackgrount.Areas.job_vacancy.DTO;
using text_loginWithBackgrount.Areas.job_vacancy.ViewModels;
using text_loginWithBackgrount.Controllers;

namespace text_loginWithBackgrount.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    /// <summary>
    /// 訪客能看到的基本分頁
    /// </summary>
    public class jobController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly studentContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public jobController(ILogger<HomeController> logger, studentContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;

            // 設置 QuestPDF 的授權類型為社區版
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: job_vacancy/job/CompanyDetails/5
        [Route("/job_vacancy/job/{Action=Index}/{companyID}")]
        public async Task<IActionResult> CompanyDetails(int companyID)
        {

            try
            {
                var thisCompany = await _context.T工作公司資料s.FindAsync(companyID);

                if (thisCompany == null)
                {
                    return NotFound("找不到此公司資料");
                }

                var viewModel = new CompanyDetailViewModel
                {
                    companyID = thisCompany.FId,
                    taxID = thisCompany.F統一編號,
                    CompanyName = thisCompany.F公司名稱,
                    Principal = !string.IsNullOrEmpty(thisCompany.F負責人) ? thisCompany.F負責人 : "暫不提供",
                    CompanyPhone = !string.IsNullOrEmpty(thisCompany.F公司電話) ? thisCompany.F公司電話 : "暫不提供",
                    CompanyAddress = !string.IsNullOrEmpty(thisCompany.F公司地址) ? thisCompany.F公司地址 : "暫不提供",
                    CompanyProfile = !string.IsNullOrEmpty(thisCompany.F公司簡介) ? thisCompany.F公司簡介.Replace("\\n", "\n") : "暫不提供",
                    ContactPerson = !string.IsNullOrEmpty(thisCompany.F聯絡人) ? thisCompany.F聯絡人 : "請洽平台系統管理員",
                    ContactPhone = !string.IsNullOrEmpty(thisCompany.F聯絡人電話) ? thisCompany.F聯絡人電話 : "請洽平台系統管理員",
                    ContactEmail = !string.IsNullOrEmpty(thisCompany.F聯絡人Email) ? thisCompany.F聯絡人Email : "請洽平台系統管理員"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }

        // GET: job_vacancy/job/JobDetails/5
        [Route("/job_vacancy/job/{Action=Index}/{jobID}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "student")] //限定要登入學員帳號
        public async Task<IActionResult> JobDetails(int jobID)
        {
            //獲取登入學生的ID
            var user = HttpContext.User.Claims.ToList();
            var loginID = Convert.ToInt32(user.Where(a => a.Type == "StudentId").First().Value);

            ViewData["ApplyStudentID"] = loginID;

            try
            {
                var thisJob = await _context.T工作職缺資料s.FindAsync(jobID);

                if (thisJob == null)
                {
                    return NotFound("找不到此職缺資料");
                }

                var thisCompany = await _context.T工作公司資料s.FirstOrDefaultAsync(company => company.FId == thisJob.F公司Id);

                if (thisCompany == null)
                {
                    return NotFound("找不到此職缺的相應公司資料，請聯絡平台系統管理員");
                }

                var viewModel = new JobDetailViewModel
                {
                    JobID = jobID,
                    JobTitle = thisJob.F職務名稱,
                    UpdateTime = thisJob.F最後更新時間.HasValue ? thisJob.F最後更新時間.Value.ToString("yyyy/MM/dd HH:mm:ss") + "更新" : string.Empty,
                    JobContent = !string.IsNullOrEmpty(thisJob.F工作內容) ? thisJob.F工作內容.Replace("\\n", "\n") : "暫不提供",
                    Salary = !string.IsNullOrEmpty(thisJob.F薪水待遇) ? thisJob.F薪水待遇 : "暫不提供",
                    JobType = !string.IsNullOrEmpty(thisJob.F工作性質) ? thisJob.F工作性質 : "暫不提供",
                    JobLocation = !string.IsNullOrEmpty(thisJob.F工作地點) ? thisJob.F工作地點 : "暫不提供",
                    JobTime = !string.IsNullOrEmpty(thisJob.F工作時段) ? thisJob.F工作時段 : "暫不提供",
                    ShiftRequirement = !string.IsNullOrEmpty(thisJob.F輪班需求) ? (thisJob.F輪班需求 == "Y" ? "需要輪班" : "不需輪班") : "暫不提供",
                    RequiredPeople = !string.IsNullOrEmpty(thisJob.F需求人數) ? thisJob.F需求人數 : "暫不提供",
                    AcademicRequirement = !string.IsNullOrEmpty(thisJob.F學歷要求) ? thisJob.F學歷要求.Replace("\\n", "\n") : "不拘",
                    LanguageCondition = !string.IsNullOrEmpty(thisJob.F語文條件) ? thisJob.F語文條件.Replace("\\n", "\n") : "不拘",
                    WorkAbility = !string.IsNullOrEmpty(thisJob.F工作技能) ? thisJob.F工作技能.Replace("\\n", "\n") : "無特別要求",
                    OtherCondition = !string.IsNullOrEmpty(thisJob.F其他條件) ? thisJob.F其他條件.Replace("\\n", "\n") : "無特別要求",

                    CompanyID = thisCompany.FId,
                    CompanyName = thisCompany.F公司名稱,
                    ContactPerson = !string.IsNullOrEmpty(thisCompany.F聯絡人) ? thisCompany.F聯絡人 : "請洽平台系統管理員",
                    ContactPhone = !string.IsNullOrEmpty(thisCompany.F聯絡人電話) ? thisCompany.F聯絡人電話 : "請洽平台系統管理員",
                    ContactEmail = !string.IsNullOrEmpty(thisCompany.F聯絡人Email) ? thisCompany.F聯絡人Email : "請洽平台系統管理員"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }

        }

        /// <summary>
        /// 應徵1：返回應徵信的視圖（含履歷清單）
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="jobTitle"></param>
        /// <returns></returns>
        // GET: job_vacancy/job/GetResumeTitles
        [Route("/job_vacancy/job/{Action=Index}")]
        public async Task<IActionResult> GetResumeTitles([FromQuery] int studentID, [FromQuery] int jobID)
        {

            var studentName = await _context.T會員學生s
                              .Where(s => s.學生id == studentID)
                              .Select(s => s.姓名)
                              .FirstOrDefaultAsync();

            var resumeData = await _context.T工作履歷資料s
                            .Where(r => r.F學員Id == studentID && r.F刪除狀態 == "0")
                            .OrderByDescending(r => r.F最後更新時間)
                            .Select(r => new { r.FId, r.F履歷名稱 })
                            .ToListAsync();

            var jobTitle = await _context.T工作職缺資料s
                          .Where(j => j.FId == jobID)
                          .Select(j => j.F職務名稱)
                          .FirstOrDefaultAsync();
            
            var resumeTitles = resumeData.Select(r => r.F履歷名稱).ToList();
            var resumeIDs = resumeData.Select(r => r.FId).ToList();

            var viewModel = new ApplyViewModel
            {
                StudentID = studentID,
                JobID = jobID,
                JobTitle = jobTitle,
                ResumeIDs = resumeIDs,
                ResumeTitles = resumeTitles,
                ApplyLetter = $"您好，我是{studentName}，希望能獲得面試的機會。"
            };

            return PartialView("_ApplyPartial", viewModel);
        }

        /// <summary>
        /// 應徵2：送出應徵信
        /// </summary>
        /// <param name="viewModel"></param>
        // POST: job_vacancy/job/ApplyLetter
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/job_vacancy/job/{Action=Index}")]
        public async Task<IActionResult> ApplyLetter(
            [Bind("StudentID, JobID, ResumeID, ApplyLetter")] ApplyViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var thisResume = await _context.T工作履歷資料s.FindAsync(viewModel.ResumeID);
                    if (thisResume == null)
                    {
                        return Json(new { success = false, message = "找不到指定的履歷" });
                    }

                    var theStudent = await _context.T會員學生s.FindAsync(viewModel.StudentID);
                    if (theStudent == null)
                    {
                        return Json(new { success = false, message = "找不到指定的學生" });
                    }

                    var theJob = await _context.T工作職缺資料s.FindAsync(viewModel.JobID);
                    if (theJob == null)
                    {
                        return Json(new { success = false, message = "找不到指定的職缺" });
                    }

                    var theCompany = await _context.T工作公司資料s.Where(data => data.FId == theJob.F公司Id).FirstOrDefaultAsync();
                    if (theCompany == null)
                    {
                        return Json(new { success = false, message = "找不到指定的公司" });
                    }

                    List<T工作工作經驗> thisResumeWorkExp = null;
                    if (thisResume.F有無工作經驗 == "Y")
                    {
                        thisResumeWorkExp = await (
                            from workExp in _context.T工作工作經驗s
                            join resumeWorkExp in _context.T工作履歷表工作經驗s
                                on workExp.FId equals resumeWorkExp.F工作經驗Id
                            where resumeWorkExp.F履歷Id == thisResume.FId
                            select workExp
                        ).ToListAsync();
                    }

                    var resumeData = new ResumeDataDTO
                    {
                        Resume = thisResume,
                        Student = theStudent,
                        WorkExperience = thisResumeWorkExp
                    };

                    // 使用 QuestPDF 生成 PDF 文件
                    var pdfData = GeneratePdf(resumeData);

                    // 將 PDF 文件保存到暫時資料夾
                    //var pdfFilePath = SavePdf(pdfData);

                    // 將 PDF 文件作為附件寄出
                    string recipientName = theCompany.F公司名稱;
                    string recipientEmail = theCompany.F聯絡人Email;
                    string subject = $"【Rasengan 人才培訓服務系統應徵信】{theStudent.姓名} - {theJob.F職務名稱}";
                    string letterBody = $"尊敬的 {recipientName} 負責人，您好，\r\n\r\n" +
                                        $"本封郵件由 Rasengan 人才培訓服務系統自動發送，特此通知貴公司收到一份新的應徵信。\r\n\r\n" +
                                        $"應徵內容如下：\r\n{viewModel.ApplyLetter}\r\n\r\n" +
                                        $"如果對求職者有興趣，請直接與他/她聯絡，謝謝。\r\n\r\n";

                    bool sendSuccess = SendEmail(recipientName, recipientEmail, subject, letterBody, pdfData);
                    if (!sendSuccess)
                        return Json(new { success = false, message = "應徵信寄送失敗，請稍後再試或洽系統管理者。" });

                    var applyRecord = new T工作應徵工作紀錄
                    {
                        F學員Id = viewModel.StudentID,
                        F職缺Id = viewModel.JobID,
                        F應徵信內容 = viewModel.ApplyLetter,
                        F應徵時間 = DateTime.Now,
                        F刪除狀態 = "0"
                    };

                    _context.T工作應徵工作紀錄s.Add(applyRecord);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "應徵成功" });

                }
                else
                {
                    string errorMessage = string.Join("；", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "發生異常：" + ex.Message });
            }
        }

        private byte[] GeneratePdf(ResumeDataDTO resumeData)
        {
            var document = Document.Create(container =>
            {

                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x
                            .FontSize(12)
                            .FontFamily("源石黑體"));

                    // 獲取LOGO圖片的相對路徑
                    string logoFilePath = Path.Combine("images", "logo.jpg");
                    // 組合圖片的完整路徑
                    string logoFullPath = Path.Combine(_hostingEnvironment.WebRootPath, logoFilePath);

                    //頁首
                    page.Header()
                        .Background("#008374")
                        .Width(100)
                        .Height(35)
                        .AlignMiddle()
                        .PaddingLeft(25)
                        .Image(logoFullPath);

                    //頁腳
                    page.Footer()
                        .Background("#008374")
                        .Height(35);

                    //內頁主體
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .PaddingHorizontal(2, Unit.Centimetre)
                        .Column(x =>
                        {
                            // 設定行距
                            x.Spacing(5);

                            // ==== 基本資料區 =====
                            x.Item().Row(row =>
                            {
                                if (resumeData?.Student?.圖片 != null)
                                {
                                    row.RelativeItem(3)
                                        .Width(1, Unit.Inch)
                                        .AlignMiddle()
                                        .Image(resumeData.Student.圖片)
                                        .WithCompressionQuality(ImageCompressionQuality.Low)
                                        .WithRasterDpi(72);
                                }
                                else
                                {
                                    row.RelativeItem(3)
                                        .Width(1, Unit.Inch)
                                        .Border(1)
                                        .BorderColor("#008374")
                                        .Background(Colors.Grey.Lighten3)
                                        .AlignBottom()
                                        .AlignCenter()
                                        .Height(35)
                                        .Image(logoFullPath);
                                }
                                row.RelativeItem(9)
                                        .Column(column =>
                                        {
                                            column.Spacing(5);
                                            column.Item().Text($"{resumeData?.Student?.姓名}").LetterSpacing(0.05f).FontSize(20).Bold();

                                            var birthday = resumeData?.Student?.生日;
                                            if (birthday != null)
                                            {
                                                var today = DateTime.Today;
                                                var age = today.Year - birthday.Value.Year;
                                                if (birthday > today.AddYears(-age)) age--;

                                                column.Item().Text(text =>
                                                {
                                                    text.Span("個人資訊　").SemiBold().LetterSpacing(0.05f);
                                                    text.Span($"{age}歲，{resumeData?.Student?.性別 ?? ""}").LetterSpacing(0.05f);
                                                });
                                            }
                                            else
                                            {
                                                column.Item().Text(text =>
                                                {
                                                    text.Span("個人資訊　").SemiBold().LetterSpacing(0.05f);
                                                    text.Span(resumeData?.Student?.性別 ?? "").LetterSpacing(0.05f);
                                                });
                                            }

                                            column.Item().Text(text =>
                                            {
                                                text.Span("連絡電話　").SemiBold().LetterSpacing(0.05f);
                                                text.Span($"{resumeData?.Student?.手機 ?? ""}").LetterSpacing(0.05f);
                                            });

                                            column.Item().Text(text =>
                                            {
                                                text.Span("聯絡信箱　").SemiBold().LetterSpacing(0.05f);
                                                text.Span($"{resumeData?.Student?.信箱 ?? ""}").LetterSpacing(0.05f);
                                            });

                                            column.Item().Text(text =>
                                            {
                                                text.Span("希望職稱　").SemiBold().LetterSpacing(0.05f);
                                                text.Span($"{resumeData?.Resume?.F希望職稱 ?? ""}").LetterSpacing(0.05f);
                                            });
                                        });
                            });

                            // ==== 學歷區 =====
                            x.Item().Height(20);
                            x.Item().Text("學歷").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");
                            x.Item().Text($"{resumeData?.Student?.學校 ?? ""}　{resumeData?.Student?.科系 ?? ""}").SemiBold().LetterSpacing(0.05f);
                            x.Item().Text($"{resumeData?.Student?.學位 ?? ""}　{resumeData?.Student?.畢肄 ?? ""}").LetterSpacing(0.05f);

                            // ==== 專長技能區 =====
                            x.Item().Height(30);
                            x.Item().Text("專長技能").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");
                            x.Item().Text(resumeData?.Resume?.F專長技能 ?? "").LetterSpacing(0.05f).LineHeight(1.5f);

                            // ==== 語文能力區 =====
                            x.Item().Height(30);
                            x.Item().Text("語文能力").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");
                            x.Item().Text(resumeData?.Resume?.F語文能力 ?? "").LetterSpacing(0.05f).LineHeight(1.5f);

                            // ==== 工作經驗區 =====
                            x.Item().Height(30);
                            x.Item().Text("工作經驗").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");
                            if (resumeData?.WorkExperience != null && resumeData?.WorkExperience.Count > 0)
                            {
                                foreach (var data in resumeData.WorkExperience)
                                {
                                    x.Item().ShowEntire().Layers(layers =>
                                    {
                                        layers
                                            .Layer()
                                            .Height(64)
                                            .Width(62)
                                            .Background(Colors.Grey.Lighten3);

                                        layers
                                            .PrimaryLayer()
                                            .Padding(25)
                                            .Column(column =>
                                            {
                                                column.Item().Column(c =>
                                                {
                                                    c.Spacing(5);
                                                    c.Item().Text($"{data.F職務名稱 ?? ""}").FontSize(14).Bold().LetterSpacing(0.05f);
                                                    c.Item().Text($"{data.F公司名稱 ?? ""}　{data.F起始年月 ?? ""} ~ {data.F結束年月 ?? ""}").FontSize(10).LetterSpacing(0.05f);
                                                    c.Item().PaddingTop(5).Text($"{data.F工作內容 ?? ""}").LetterSpacing(0.05f).LineHeight(1.5f);
                                                });
                                            });
                                    });
                                }
                            }
                            else
                            {
                                x.Item().Text("無");
                            }

                            // ==== 求職條件區 =====
                            x.Item().Height(30);
                            x.Item().Text("求職條件").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");

                            x.Item().Text(text =>
                            {
                                text.Span("工作性質　").SemiBold().LetterSpacing(0.05f);
                                text.Span(resumeData?.Resume?.F工作性質 ?? "").LetterSpacing(0.05f);
                            });
                            x.Item().Text(text =>
                            {
                                text.Span("工作時段　").SemiBold().LetterSpacing(0.05f);
                                text.Span(resumeData?.Resume?.F工作時段 ?? "").LetterSpacing(0.05f);
                            });
                            x.Item().Text(text =>
                            {
                                text.Span("配合輪班　").SemiBold().LetterSpacing(0.05f);

                                if (resumeData?.Resume?.F配合輪班 == "Y")
                                {
                                    text.Span("可配合輪班").LetterSpacing(0.05f);
                                }
                                else
                                {
                                    text.Span("不便配合輪班").LetterSpacing(0.05f);
                                }
                            });
                            x.Item().Text(text =>
                            {
                                text.Span("希望薪水待遇　").SemiBold().LetterSpacing(0.05f);
                                text.Span(resumeData?.Resume?.F希望薪水待遇 ?? "").LetterSpacing(0.05f);
                            });
                            x.Item().Text(text =>
                            {
                                text.Span("希望工作地點　").SemiBold().LetterSpacing(0.05f);
                                text.Span(resumeData?.Resume?.F希望工作地點 ?? "").LetterSpacing(0.05f);
                            });

                            // ==== 自傳區 =====
                            x.Item().Height(30);
                            x.Item().Text("自傳").FontSize(14).Bold().LetterSpacing(0.05f);
                            x.Item().PaddingBottom(5).LineHorizontal(2).LineColor("#008374");
                            x.Item().Text(resumeData?.Resume?.F自傳 ?? "").LetterSpacing(0.05f).LineHeight(1.5f);
                        });
                });
            });

            // 將 PDF 內容保存到記憶體中
            MemoryStream memoryStream = new MemoryStream();
            document.GeneratePdf(memoryStream);
            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }

        private string SavePdf(byte[] pdfData)
        {
            throw new NotImplementedException();
        }

        public bool SendEmail(string recipientEmail, string recipientName, string subject, string body, byte[] pdfData)
        {

            string senderName = "Rasengen人才培訓服務系統";
            string senderEmail = "saiunkoku2008@gmail.com";
            //Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            string senderPassword = "ubad armd qien kvvo";        //安全性考量
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(recipientName, senderEmail));
            message.Subject = subject;

            // 創建一個 MimePart 來表示 PDF 附件
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(pdfData)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = subject + ".pdf"
            };

            // 測試用：用桌面的 txt 檔作為附件
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string txtFilePath = Path.Combine(desktopPath, "text.txt");
            //byte[] txtData = System.IO.File.ReadAllBytes(txtFilePath);
            //var attachment = new MimePart("text", "plain")
            //{
            //    Content = new MimeContent(new MemoryStream(txtData)),
            //    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            //    ContentTransferEncoding = ContentEncoding.Base64,
            //    FileName = "text.txt"
            //};



            // 創建郵件主體
            var multipart = new Multipart("mixed");
            multipart.Add(new TextPart("plain") { Text = body }); // 添加郵件的文本內容
            multipart.Add(attachment); // 添加PDF附件

            // 將郵件主體設置為郵件的內容
            message.Body = multipart;

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Timeout = 60000; // 設置連接超時時間為 60 秒
                    client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    client.Authenticate(senderEmail, senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true; // 寄信成功
            }
            catch
            {
                return false; // 寄信失敗
            }

        }

        public IActionResult ResumePreview()
        {
            return View();
        }
    }
}
