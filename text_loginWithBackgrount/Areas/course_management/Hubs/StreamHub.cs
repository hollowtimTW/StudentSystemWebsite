using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace text_loginWithBackgrount.Areas.course_management.Hubs
{
    [Area("course_management")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher,student")]
    public class StreamHub : Hub
    {
        private readonly studentContext _context;
        private readonly IDistributedCache _cache;
        private readonly ConcurrentDictionary<string, string> subjectGroups = new ConcurrentDictionary<string, string>();

        public StreamHub(studentContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// 這個方法是老師用於先創建自己的直播連線。
        /// </summary>
        /// <param name="id">班級科目id。</param>
        /// <returns>再_cashe中記錄傳進來的班級科目id的授課老師的連線id為何。</returns>
        public async Task createGroup(string id)
        {
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                string ConnectionId = Context.ConnectionId;
                string groupName = id + ":" + ConnectionId + ":" + DateNow;
                //連結字串 - 群組名稱[讓老師連結加入群組]
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                //連結字串 - 群組名稱[讓老師連結加入群組]存入cashe(不論是否有原本的群組，當老師重整就會新創一個連線，學生端要重整才能連到新的群組)
                await _cache.SetStringAsync(id, groupName);

                var message = "群組已經創建，請通知學生們重新載入";
                var obj = new
                {
                    groupName = groupName,
                    message
                };
                string json = JsonConvert.SerializeObject(obj);

                await Clients.Caller.SendAsync("groupJoinState", json);
            }
                catch (Exception ex)
                {
                Console.WriteLine("An error occurred: " + ex.Message);
                var message = "群組創建發生問題，請重新重新載入";
                var obj = new
                {
                    groupName = "",
                    message
                };
                string json = JsonConvert.SerializeObject(obj);
                await Clients.Caller.SendAsync("groupJoinState", json);
            }

            }

        /// <summary>
        /// 這個方法是學生在載入直播畫面後用於確認目前這個班級科目id是否已經有授課老師的直播連線。
        /// </summary>
        /// <param name="classCourseId">班級科目id。</param>
        /// <param name="peerId">peer.js幫學生創建的peerid。</param>
        /// <returns>再_cashe中查詢傳進來的班級科目id的授課老師的連線id為何，如果有就把學生的peerid傳給老師的連線。</returns>
        public async Task checkGroup(string classCourseId, string peerId)
          {
            try
            {
                string groupName = await _cache.GetStringAsync(classCourseId);

                if (!string.IsNullOrEmpty(groupName))
                {
                    // 加入找到的群組[讓學生連結加入群組]
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                    // 執行相應操作，例如發送成功加入群組的通知
                    var message = "已經連結到群組";
                    var obj = new
                    {
                        groupName = groupName,
                        message
                    };
                    string json = JsonConvert.SerializeObject(obj);
                    await Clients.Caller.SendAsync("groupJoinState", json);

                    //發送老師端創建這個學生的offer
                    string teacherConnectId = groupName.Split(":")[1];
                    await Clients.Client(teacherConnectId).SendAsync("ReceiveStudentConnectId", peerId);

                }
                else
                {
                    // 群組不存在，執行相應操作，例如發送群組不存在的提示
                    var message = "群組尚未創建，請耐心等待老師創建";
                    var obj = new
                    {
                        groupName = "",
                        message
                    };
                    string json = JsonConvert.SerializeObject(obj);

                    await Clients.Caller.SendAsync("groupJoinState", json);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);

                var message = "連線發生問題，請聯絡站方";
                var obj = new
                {
                    groupName = "",
                    message
                };
                string json = JsonConvert.SerializeObject(obj);

                await Clients.Caller.SendAsync("groupJoinState", json);
            }
        }

        public async Task SendPeerId(string groupName, string peerId)
        {
            string teacherConnectId = groupName.Split(":")[1];
            await Clients.Client(teacherConnectId).SendAsync("ReceiveStudentConnectId", peerId);
        }

        /// <summary>
        /// 這個方法是學生比老師開啟音訊流還早開啟畫面時時，嘗試在老師開啟影訊流時通知學生把自己的peerId傳給老師端。，但目前尚未將流程寫完
        /// </summary>
        /// <param name="groupName">老師所建立的groupName。</param>
        /// <returns>叫所有有綁定SendPeerIdToteacher的學生。</returns>
        public async Task informStudent(string groupName)
        {
            await Clients.Group(groupName).SendAsync("SendPeerIdToteacher");

        }
    }
}
