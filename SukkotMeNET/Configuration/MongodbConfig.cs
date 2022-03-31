namespace SukkotMeNET.Configuration
{
    public class MongodbConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public MongodbCollectionsConfig Collections { get; set; }
    }

    public class MongodbCollectionsConfig
    {
        public string Users { get; set; }
    }
}
