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


        public async Task createGroup(string id)
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

        public async Task checkGroup(string classCourseId, string peerId)
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

        public async Task SendPeerId(string groupName, string peerId)
        {
            string teacherConnectId = groupName.Split(":")[1];
            await Clients.Client(teacherConnectId).SendAsync("ReceiveStudentConnectId", peerId);
        }

        public async Task informStudent(string groupName)
        {
            string teacherConnectId = groupName.Split(":")[1];
            await Clients.AllExcept(teacherConnectId).SendAsync("SendPeerIdToteacher");
        }
    }
}
