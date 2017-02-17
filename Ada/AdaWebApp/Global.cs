using System.Configuration;

namespace AdaWebApp
{
    public static class Global
    {
        // Host of web application configured in azure app settings
        public static readonly string Host = ConfigurationManager.AppSettings["Host"];

        // Oxford key for face api
        public static readonly string OxfordFaceApiKey = ConfigurationManager.AppSettings["OxfordFaceApiKey"];

        // Oxford key for emotion api
        public static readonly string OxfordEmotionApiKey = ConfigurationManager.AppSettings["OxfordEmotionApiKey"];

        // Oxford person group id with contains all kown persons
        public static readonly string OxfordPersonGroupId = ConfigurationManager.AppSettings["OxfordPersonGroupId"];

        // virtual path to persons database
        public static readonly string PersonsDatabaseDirectory = $"~/PersonsFiles/{OxfordPersonGroupId}/";

        // temporary uploads folder
        public static readonly string TemporaryUploadsFolder = "~/Uploads/";
    }
}