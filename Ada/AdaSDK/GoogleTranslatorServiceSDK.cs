using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdaSDK
{
    public class GoogleTranslatorServiceSDK
    {
        public GoogleTranslatorServiceSDK()
        {

        }

        public async Task<string> TranslateText(string input, string languagePair)
        {
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);

            HttpClient webClient = new HttpClient();

            //webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            var result = await webClient.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();
            content = content.Substring(content.IndexOf("onmouseout=") + 47);
            content = content.Substring(0, content.IndexOf("</span>"));
            return content;
        }
    }
}