namespace AdaW10
{
    public static class AppConfig
    {
#if DEBUG
        public static readonly string WebUri = "http://adawebapp.azurewebsites.net";      // AdaWebApp URL for Test
#else
        public static readonly string WebUri = "";      // AdaWebApp URL fro Prod
#endif
        public static readonly string UserName = "test";    // Use to get the API token
        public static readonly string Password = "test";
    }
}