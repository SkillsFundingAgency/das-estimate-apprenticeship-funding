using System.Web.Optimization;

namespace SFA.DAS.ForecastingTool.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.floatThead.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/details.polyfill.js",
                "~/scripts/govuk-template.js"));

            bundles.Add(new ScriptBundle("~/bundles/training-course").Include(
                "~/Scripts/forecasting-number-extension-1.0.js",
                "~/scripts/forecasting-trainingcourse-1.0.js"));

            bundles.Add(new StyleBundle("~/Content/stylesheets/bundled-css").Include(
                "~/Content/stylesheets/govuk-template.css",
                "~/Content/stylesheets/fonts.css",
                "~/Content/stylesheets/das-controls.css",
                "~/Content/stylesheets/forecasting-main.css",
                "~/Content/stylesheets/forecasting-trainingcourse.css",
                "~/Content/stylesheets/forecasting-results.css"));
        }
    }
}