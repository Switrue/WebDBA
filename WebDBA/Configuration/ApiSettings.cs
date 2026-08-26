namespace WebDBA.Configuration
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "https://localhost:5001";
        public int TimeoutSeconds { get; set; } = 30;
        public string AcceptHeader { get; set; } = "application/json";
    }
}
