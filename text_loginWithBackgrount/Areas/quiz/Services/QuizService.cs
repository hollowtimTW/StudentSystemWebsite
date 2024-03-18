using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;
using text_loginWithBackgrount.Areas.quiz.Hubs;

namespace text_loginWithBackgrount.Areas.quiz.Services
{
    public class QuizService
    {
        private readonly IHubContext<QuizHub> _hubContext;
        public QuizService(IHubContext<QuizHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task StartQuiz(string ConnectionId)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds <= 30)
            {
                // 取得目前時間
                DateTime now = DateTime.Now;
                string nowString = now.ToString();

                // 傳送目前時間
                await _hubContext.Clients.Group("aaaa").SendAsync("SetTime", nowString);
                await Console.Out.WriteLineAsync(ConnectionId + " => " +nowString);
                // 等待 5 秒
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            await _hubContext.Clients.Group("aaaa").SendAsync("OverTime");
            await Console.Out.WriteLineAsync(ConnectionId + " => End");
        }
    }
}
