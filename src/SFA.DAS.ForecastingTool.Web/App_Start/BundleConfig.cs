using System.Web.Optimization;

namespace SFA.DAS.ForecastingTool.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/assets/javascripts/govuk-template.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/stylesheets/govuk-template.css",
                      "~/assets/stylesheets/fonts.css",
                      "~/Content/Site.css"
                      ));
        }
    }
}