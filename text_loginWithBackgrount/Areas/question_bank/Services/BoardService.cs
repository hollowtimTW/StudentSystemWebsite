using Microsoft.Extensions.Options;
using MongoDB.Driver;
using text_loginWithBackgrount.Areas.question_bank.Mongodb;
using text_loginWithBackgrount.Areas.question_bank.ViewModels;


namespace text_loginWithBackgrount.Services
{
	public class BoardService
	{
		private readonly IMongoCollection<Board> _boardCollection;

		public BoardService(
		IOptions<MongoDbCollectionSettings> boardCollectionSettings)
		{
			var mongoClient = new MongoClient(
				boardCollectionSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				boardCollectionSettings.Value.DatabaseName);

			_boardCollection = mongoDatabase.GetCollection<Board>(
				boardCollectionSettings.Value.CollectionName);
		}

		public async Task<List<Board>> GetAsync() =>
		   await _boardCollection.Find(_ => true).ToListAsync();

		public async Task<Board?> GetAsync(string id) =>
			await _boardCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

		public async Task CreateAsync(Board newBoard) =>
			await _boardCollection.InsertOneAsync(newBoard);

		public async Task UpdateAsync(string id, Board updatedBoard) =>
			await _boardCollection.ReplaceOneAsync(x => x.Id == id, updatedBoard);

		public async Task RemoveAsync(string id) =>
			await _boardCollection.DeleteOneAsync(x => x.Id == id);
	}
}
