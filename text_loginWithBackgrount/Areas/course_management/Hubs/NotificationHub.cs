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
using System.Collections.Generic;


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

                if (ConnectionTeacherMap.TryGetValue(userId, out string existingConnectionId))
                {
                    ConnectionTeacherMap[userId] = connectionId;
                    Console.WriteLine($"Replacing existing connection for UserId = {userId}: Old ConnectionId = {existingConnectionId}");
                }
                else
                {
                    ConnectionTeacherMap.TryAdd(userId, connectionId);
                    Console.WriteLine($"User connected: ConnectionId = {connectionId}, UserId = {userId}");
                }

            }

            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;

            var userIdToRemove = ConnectionTeacherMap.FirstOrDefault(pair => pair.Value == connectionId).Key;
            if (userIdToRemove != null)
            {
                ConnectionTeacherMap.TryRemove(userIdToRemove, out _);
                Console.WriteLine($"User disconnected: ConnectionId = {connectionId}, UserId = {userIdToRemove}");
            }
            else
            {
                Console.WriteLine($"User disconnected: ConnectionId = {connectionId}, No corresponding UserId found.");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<string> GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task SendNotification(string teacheId)
        {
            try
            {
                string message = "您有一則新通知!";
                // 將通知發送給有上線的接收者

                string connectionId = ConnectionTeacherMap.FirstOrDefault(pair => pair.Key == teacheId).Value;

                if (connectionId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
                }

            }
            catch (Exception ex)
            {
                string connectionId = ConnectionTeacherMap.FirstOrDefault(pair => pair.Key == teacheId).Value;
                Console.WriteLine($"發生錯誤: {ex.Message}");
                await Clients.Client(connectionId).SendAsync("HandleError", "發生錯誤: " + ex.Message);


            }

        }


    }
}
