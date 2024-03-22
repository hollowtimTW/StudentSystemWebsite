using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Security.Claims;
using text_loginWithBackgrount.Areas.quiz.Models;


namespace text_loginWithBackgrount.Areas.quiz.Hubs
{
    public class QuizHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly studentContext _context;

        private readonly IMongoCollection<QuestionOrder> _questionOrderCollection;
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<StudentAnswer> _studentAnswerCollection;


        public QuizHub(IHttpContextAccessor httpContextAccessor, studentContext context, IOptions<MongoDbSettings> mongoDbSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _questionOrderCollection = mongoDatabase.GetCollection<QuestionOrder>(
                mongoDbSettings.Value.QuestionOrderCollection);

            _questionCollection = mongoDatabase.GetCollection<Question>(
                mongoDbSettings.Value.QuestionCollection);

            _studentAnswerCollection = mongoDatabase.GetCollection<StudentAnswer>(
                mongoDbSettings.Value.StudentAnswerCollection);
        }


        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? ex)
        {

            await Clients.Client(Context.ConnectionId).SendAsync("SaveAnswers");
            await base.OnDisconnectedAsync(ex);
        }

   

        

    }
}
