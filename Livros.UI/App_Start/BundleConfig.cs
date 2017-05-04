using System.Web;
using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

namespace Livros.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {


#if DEBUG
                        bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/bower_components/angular/angular.js",
                        "~/bower_components/angular-animate/angular-animate.js",
                        "~/bower_components/angular-aria/angular-aria.js",
                        "~/bower_components/angular-messages/angular-messages.js",
                        "~/bower_components/angular-material/angular-material.js",
                        "~/bower_components/angular-material-icons/angular-material-icons.js",
                        "~/scripts/app/app.js",
                        "~/scripts/app/bookService.js",
                        "~/scripts/app/mainController.js"
                        ));
            
            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                      "~/bower_components/bootstrap/dist/css/bootstrap.css",
                      "~/bower_components/angular-material/angular-material.css",
                      "~/Content/site.css"
                      ));
        System.Web.Optimization.BundleTable.EnableOptimizations = false;
#else

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/bower_components/angular/angular.min.js",
                "~/bower_components/angular-animate/angular-animate.min.js",
                "~/bower_components/angular-aria/angular-aria.min.js",
                "~/bower_components/angular-messages/angular-messages.min.js",
                "~/bower_components/angular-material/angular-material.min.js",
                "~/bower_components/angular-material-icons/angular-material-icons.min.js",
                "~/scripts/app/app.min.js",
                "~/scripts/app/bookService.min.js",
                "~/scripts/app/mainController.min.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                "~/bower_components/bootstrap/dist/css/bootstrap.min.css",
                "~/bower_components/angular-material/angular-material.min.css",
                "~/Content/site.min.css"
            ));
            System.Web.Optimization.BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
