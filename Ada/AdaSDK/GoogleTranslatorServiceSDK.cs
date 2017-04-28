using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            webClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");

            var result = await webClient.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();
            content = content.Substring(content.IndexOf("onmouseout=") + 47);
            content = content.Substring(0, content.IndexOf("</span>"));
            return content;
        }
    }
}