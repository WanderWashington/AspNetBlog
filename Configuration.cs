namespace AspNetBlog
{
    public static class Configuration
    {
        //Token - JWT - Json Web Token
        public static string JwtKey = "ZmVkYW3ZDg4NjNiNDhlMTk3YjkyOdkNDkyYjcwOGU=";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "curso_api_IlTevUM/z0ey3NmCV/unWG==";
        public static SmtpConfiguration Smtp = new SmtpConfiguration();

        public class SmtpConfiguration
        {
            public string Host { get; set; }

            public int Port { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }
        }
    }
}
