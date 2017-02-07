namespace MartineobotIOTMvvm
{
    public static class AppConfig
    {
#if DEBUG
        public static readonly string WebUri = "";      // AdaWebApp URL for Test
#else
        public static readonly string WebUri = "";      // AdaWebApp URL fro Prod
#endif
        public static readonly string UserName = "";    // Use to get the API token
        public static readonly string Password = "";
    }
}