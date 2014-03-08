using System.Web;
using System.Web.Optimization;

namespace HomeScrum.Web
{
   public class BundleConfig
   {
      // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      public static void RegisterBundles( BundleCollection bundles )
      {
         // TODO: Move utilities into base and add to base scripts bundle
         bundles.Add( new ScriptBundle( "~/bundles/myScripts" )
            .Include( "~/Scripts/HomeScrum/utilities.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/workItemScripts" )
            .Include( "~/Scripts/HomeScrum/WorkItems/Details.js" )
            .Include( "~/Scripts/HomeScrum/WorkItems/Editor.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/sprintScripts" )
            .Include( "~/Scripts/HomeScrum/Sprints/Details.js" )
            .Include( "~/Scripts/HomeScrum/Sprints/Editor.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/vendor" )
            .Include( "~/Scripts/jquery-{version}.js" )
            .Include( "~/Scripts/jquery.storage.js" )
            .Include( "~/Scripts/jquery-ui-{version}.js" )
            .Include( "~/Scripts/Bootstrap.js" )
            .Include( "~/Scripts/knockout-{version}.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jqueryDataTables" )
            .Include( "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jqueryDataTablesCust" )
            .Include( "~/Scripts/dataTables.bootstrap.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jqueryval" )
            .Include( "~/Scripts/jquery.unobtrusive*",
                      "~/Scripts/jquery.validate*" ) );

         bundles.Add( new ScriptBundle( "~/jqPlot" )
            .Include( "~/Scripts/jqPlot/jquery.jqplot.js" ) );

         bundles.Add( new ScriptBundle( "~/jqPlotplugins" )
            .Include( "~/Scripts/jqPlot/plugins/jqplot.canvasTextRenderer.js" )
            .Include( "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.js" )
            .Include( "~/Scripts/jqPlot/plugins/jqplot.dateAxisRenderer.js" ) );

         // Use the development version of Modernizr to develop with and learn from. Then, when you're
         // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
         bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                     "~/Scripts/modernizr-*" ) );

         bundles.Add( new StyleBundle( "~/Content/JQueryUItheme" ).Include(
                     "~/Content/themes/redmond/jquery-ui.css" ) );

         bundles.Add( new StyleBundle( "~/Content/themes/jqueryDataTables/css" )
            .Include( "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css" ) );

         bundles.Add( new StyleBundle( "~/Content/css" )
            .Include( "~/Content/Bootstrap.css" )
            .Include( "~/Content/Bootstrap-theme.css" )
            .Include( "~/Content/dataTables.bootstrap.css" )
            .Include( "~/Content/Site.css" )
            .Include( "~/Content/Editor.css" )
            .Include( "~/Content/HeaderFooter.css" ) );

         bundles.Add( new StyleBundle( "~/Content/SignInCss" )
            .Include( "~/Content/SignIn.css" ) );

         bundles.Add( new StyleBundle( "~/jqPlotcss" )
            .Include( "~/Scripts/jqPlot/jquery.jqplot.css" ) );
      }
   }
}