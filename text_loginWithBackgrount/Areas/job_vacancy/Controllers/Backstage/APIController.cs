using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Class_system_Backstage_pj.Areas.job_vacancy.Controllers
{
    [Area("job_vacancy")]
    public class APIController : Controller
    {
        private readonly studentContext _studentContext;

        public APIController(studentContext studentContext)
        {
            _studentContext = studentContext;
        }

        /// <summary>
        /// 用以顯示 ResumeList 的資料。
        /// </summary>
        public async Task<JsonResult> Resume()
        {
            var combine = await _studentContext.T工作履歷資料s
                            .Where(resume => resume.F刪除狀態 == "0")
                            .GroupJoin(
                                _studentContext.T課程學生班級s,
                                resume => resume.F學員Id,
                                relation => relation.學生id,
                                (resume, studentRelations) => new
                                {
                                    ResumeId = resume.FId,
                                    ResumeName = resume.F履歷名稱,
                                    HopeJobTitle = resume.F希望職稱,
                                    HopeSalary = resume.F希望薪水待遇,
                                    HopeLocation = resume.F希望工作地點,
                                    ResumeStatus = resume.F履歷狀態,
                                    LastUpdate = resume.F最後更新時間.HasValue ? resume.F最後更新時間.Value.ToString("yyyy/MM/dd HH:mm:ss") : null,
                                    StudentName = resume.F學員.姓名,
                                    Classes = studentRelations
                                        .Join(
                                            _studentContext.T課程班級s,
                                            studentRelation => studentRelation.班級id,
                                            classInfo => classInfo.班級id,
                                            (studentRelation, classInfo) => classInfo.班級名稱
                                        )
                                        .ToList()
                                }
                            )
                            .ToListAsync();

            var resumes = combine.Select(data => new { 
                            resumeId = data.ResumeId,
                            studentName = data.StudentName,
                            studentClass = data.Classes.Select(className => new { ClassName = className }),
                            resumeName = data.ResumeName,
                            hopeJobTitle = data.HopeJobTitle,
                            hopeSalary = data.HopeSalary,
                            hopeLocation = data.HopeLocation,
                            resumeStatus = data.ResumeStatus,
                            lastUpdate = data.LastUpdate
            }).ToList();

            return Json(combine);
        }

        /// <summary>
        /// 計算並回傳履歷總數。
        /// </summary>
        /// <returns></returns>
        public async Task<ContentResult> ResumeSum()
        {
            var count = await _studentContext.T工作履歷資料s
                .Where(data => data.F刪除狀態 == "0")
                .CountAsync();

            return Content(count.ToString());
        }


    }
}
