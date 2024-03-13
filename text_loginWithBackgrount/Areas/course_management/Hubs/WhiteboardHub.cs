using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Class_system_Backstage_pj.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace text_loginWithBackgrount.Areas.course_management.Hubs
{
    [Area("course_management")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher,student")]
    public class WhiteboardHub : Hub
    {
        private readonly studentContext _context;
        private readonly IDistributedCache _cache;
        private readonly ConcurrentDictionary<string, string> subjectGroups = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> ConnectionTeacherMap = new ConcurrentDictionary<string, string>();

        public WhiteboardHub(studentContext context, IDistributedCache cache)
        {

            _context = context;
            _cache = cache;

        }

        public override Task OnConnectedAsync()
        {
            string userId = Context.User?.FindFirst("teacherID")?.Value;
            string connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                userId = "老師";
            }
            else
            {
                //每個學生的姓名
                userId = Context.User?.FindFirst("FullName")?.Value;
            }


            ConnectionTeacherMap.TryAdd(connectionId, userId);
            Clients.Caller.SendAsync("ReceiveWhiteBoardConnect", connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            ConnectionTeacherMap.TryRemove(connectionId, out _);

            return base.OnDisconnectedAsync(exception);
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
            await Clients.Caller.SendAsync("groupJoinState", "請通知學生重新載入");
        }

        public async Task checkGroup(string classCourseId)
        {
            string groupName = await _cache.GetStringAsync(classCourseId);

            if (!string.IsNullOrEmpty(groupName))
            {
                // 加入找到的群組[讓學生連結加入群組]
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                string teacherConnectId = groupName.Split(":")[1];

                //確認老師端的開啟狀態
                await Clients.Client(teacherConnectId).SendAsync("checkOpenValue");

            }

        }

        public async Task OpenValueResponse(string isOpen, string classCourseId)
        {
            if (isOpen == "開啟")
            {
                string groupName = await _cache.GetStringAsync(classCourseId);
                var callerConnectionId = Context.ConnectionId;
                await Clients.GroupExcept(groupName, callerConnectionId).SendAsync("groupJoinState", "開啟");
            }
            else
            {
                string groupName = await _cache.GetStringAsync(classCourseId);
                var callerConnectionId = Context.ConnectionId;
                await Clients.GroupExcept(groupName, callerConnectionId).SendAsync("groupJoinState", "未開啟");
            }
        }
        public async Task SendPath(string jsonData, string classCourseId)
        {
            var groupName = await _cache.GetStringAsync(classCourseId);
            if (groupName != null)
            {

                await Clients.Group(groupName).SendAsync("ReceivePath", jsonData);
            }

        }

        public async Task ClearCanvas(string classCourseId)
        {
            var groupName = await _cache.GetStringAsync(classCourseId);
            if (groupName != null)
            {

                await Clients.Group(groupName).SendAsync("ReceiveClearCanvas");
            }

        }



    }
}
