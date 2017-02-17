using System;
using System.IO;

namespace AdaWebApp.Helpers
{

    public static class UrlHelpers
    {
        /// <summary>
        /// Appends host name with a virtual path
        /// If virtualPath is not a virtual path, throws an ArgumenException
        /// </summary>
        public static string Content(string host, string virtualPath)
        {
            if (!virtualPath.StartsWith("~/")){
                throw new ArgumentException("VirualPath must begin by [~/]");
            }

            return Path.Combine(host, virtualPath.Substring(2)).Replace('\\', '/');
        }
    }
}
