using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using text_loginWithBackgrount.Areas.quiz.Services;

namespace text_loginWithBackgrount.Areas.quiz.Hubs
{
    public class QuizHub : Hub
    {
        private readonly QuizService _quizService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuizHub(QuizService quizService, IHttpContextAccessor httpContextAccessor)
        {
            _quizService = quizService;
            _httpContextAccessor = httpContextAccessor;
        }



        public override async Task OnConnectedAsync()
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, "aaaa");
            //_ = Task.Run(() => _quizService.StartQuiz(Context.ConnectionId));

            await Console.Out.WriteLineAsync(Context.ConnectionId + " => START");
            
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? ex)
        {

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "aaaa");
            await base.OnDisconnectedAsync(ex);
        }

   

        

    }
}
