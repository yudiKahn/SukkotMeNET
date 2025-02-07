namespace IEsrog.Configuration
{
    public class ApplicationConfiguration
    {
        //email:
        public string EmailApiKey { get; set; } = null!;

        //database:
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string DBUsersCollectionName { get; set; } = string.Empty;
        public string DBProductsCollectionName { get; set; } = string.Empty;
        public string DBItemsCollectionName { get; set; } = string.Empty;
        public string DBCartsCollectionName { get; set; } = string.Empty;
        public string DBOrdersCollectionName { get; set; } = string.Empty;

        //jwt:
        public string JwtSecret { get; set; } = null!;

        public string SesAccess { get; set; } = null!;
        public string SesSecret { get; set; } = null!;
    }
}
