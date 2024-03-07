namespace text_loginWithBackgrount.Areas.question_bank.Mongodb
{
    public class MongoDbCollectionSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CollectionName { get; set; } = null!;
    }
}
