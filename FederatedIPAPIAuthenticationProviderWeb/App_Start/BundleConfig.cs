﻿using System.Web;
using System.Web.Optimization;

namespace FederatedIPAPIAuthenticationProviderWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.bootstrap4.min.js",
                        "~/Scripts/DataTables/dataTables.autoFill.min.js",
                        "~/Scripts/DataTables/dataTables.buttons.min.js",
                        "~/Scripts/DataTables/dataTables.colReorder.min.js",
                        "~/Scripts/DataTables/dataTables.fixedColumns.min.js",
                        "~/Scripts/DataTables/dataTables.fixedHeader.min.js",
                        "~/Scripts/DataTables/dataTables.keyTable.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js",
                        "~/Scripts/DataTables/dataTables.rowGroup.min.js",
                        "~/Scripts/DataTables/dataTables.rowReorder.min.js",
                        "~/Scripts/DataTables/dataTables.scroller.min.js",
                        "~/Scripts/DataTables/dataTables.select.min.js",
                        "~/Scripts/DataTablesCustom/dataTables.cellEdit.js",

                        "~/Scripts/DataTables/autofill.bootstrap4.min.js",
                        "~/Scripts/DataTables/buttons.bootstrap4.min.js",
                        "~/Scripts/DataTables/responsive.bootstrap4.min.js",

                        "~/Scripts/DataTablesCustom/dataTables.customInitializer.js"
                        ));
            /*Misc js librarys not available in nuget packages ans added to project by hand */
            bundles.Add(new ScriptBundle("~/bundles/client-libraries").Include(
                        "~/Scripts/qrcode/qrcode.min.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.min.js"
                      ));

            /*Custom user built js librarys for this site only
             * often used for resusable component code or standard configurations
             */
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/site/site.ajax.config.js",
                        "~/Scripts/site/site.components.js",
                        "~/Scripts/site/site.init.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.min.css"));
        }
    }
}