using System.Web.Optimization;

namespace MartineOBotWebApp
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundle/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundle/script").Include(
                "~/Scripts/jquery-2.2.2.js",
                "~/Scripts/bootstrap.js"
            ));

            bundles.Add(new ScriptBundle("~/bundle/unavailibility").Include(
                "~/Scripts/Unavailibility.js"
            ));

            bundles.Add(new ScriptBundle("~/bundle/moment").Include(
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/customutctodate.js"
            ));
        }
    }
}
