using System.Web.Optimization;

namespace SFA.DAS.ForecastingTool.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/scripts/govuk-template.js"));

            bundles.Add(new StyleBundle("~/Content/stylesheets/bundled-css").Include(
                "~/Content/stylesheets/govuk-template.css",
                "~/Content/stylesheets/fonts.css",
                "~/Content/stylesheets/forecasting-main.css"));
        }
    }
}