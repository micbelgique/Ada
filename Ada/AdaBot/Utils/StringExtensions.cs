using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AdaBot.Bot.Utils
{
    public static class StringExtensions
    {
        private static readonly Random _spintaxRnd = new Random();

        public static string Spintax(this string str, Random seed = null)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (seed == null)
            {
                seed = _spintaxRnd;
            }

            string pattern = @"\[[^\[\]]*\]";
            Match m = Regex.Match(str, pattern);
            while (m.Success)
            {
                string seg = str.Substring(m.Index + 1, m.Length - 2);
                string[] choices = seg.Split('|');
                str = str.Substring(0, m.Index) + choices[seed.Next(choices.Length)] + str.Substring(m.Index + m.Length);
                m = Regex.Match(str, pattern);
            }
            return str;
        }
    }
}