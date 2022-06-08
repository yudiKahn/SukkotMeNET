namespace SukkotMeNET.Configuration
{
    public class ApplicationConfiguration
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpAddress { get; set; }
        public string SmtpPassword { get; set; }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string DBUsersCollectionName { get; set; }
        public string DBItemsCollectionName { get; set; }
        public string DBCartsCollectionName { get; set; }
        public string DBOrdersCollectionName { get; set; }
    }
}
