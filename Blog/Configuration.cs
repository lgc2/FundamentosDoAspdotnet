namespace Blog
{
    public static class Configuration
    {
        // Token - JWT (Json Web Token)
        public static string JwtKey = "narssirvQEqfPeiHl0CcaA==";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "curso_api_FDBGdeLsAkW7r9MpbmUPiQ==";
        public static SmtpConfiguration Smtp = new();

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
