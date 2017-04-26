using System;
using System.Net;

namespace AdaSDK
{
    public class GoogleTranslatorServiceSDK
    {
        public GoogleTranslatorServiceSDK()
        {

        }

        //public string TranslateText(string input, string languagePair)
        //{
        //    string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);

        //    WebClient webClient = new WebClient();

        //    webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        //    string result = webClient.DownloadString(url);
        //    result = result.Substring(result.IndexOf("onmouseout=") + 47);
        //    result = result.Substring(0, result.IndexOf("</span>"));
        //    return result;
        //}
    }
}