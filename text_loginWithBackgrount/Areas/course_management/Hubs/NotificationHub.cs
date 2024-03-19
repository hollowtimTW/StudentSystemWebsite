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

namespace text_loginWithBackgrount.Areas.course_management.Hubs
{
    [Area("course_management")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "teacher")]
    public class NotificationHub : Hub
    {
        private readonly studentContext _context;
        private static readonly ConcurrentDictionary<string, string> ConnectionTeacherMap = new ConcurrentDictionary<string, string>();

        public NotificationHub(studentContext context)
        {

            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            string userId = Context.User?.FindFirst("teacherID")?.Value;
            string connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                Context.Items["UserId"] = userId;
            }
            ConnectionTeacherMap.TryAdd(connectionId, userId);

            Console.WriteLine($"User connected: ConnectionId = {connectionId}, UserId = {userId}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            ConnectionTeacherMap.TryRemove(connectionId, out _);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<string> GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task SendNotification(string teacheId)
        {
            string message = "您有一則新通知!";
            // 將通知發送給有上線的接收者

            string connectionId = ConnectionTeacherMap.FirstOrDefault(pair => pair.Value == teacheId).Key;

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }



        }


    }
}
