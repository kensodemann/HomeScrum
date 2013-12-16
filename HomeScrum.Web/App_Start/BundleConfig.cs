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

         bundles.Add( new ScriptBundle( "~/bundles/baseScripts" )
            .Include( "~/Scripts/HomeScrum/Base/EditorBase.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/workItemScripts" )
            .Include( "~/Scripts/HomeScrum/WorkItems/Editor.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/sprintScripts" )
            .Include( "~/Scripts/HomeScrum/Sprints/Editor.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jquery" )
            .Include( "~/Scripts/jquery-{version}.js" )
            .Include( "~/Scripts/jquery.storage.js" )
            .Include( "~/Scripts/jquery-ui-{version}.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jqueryDataTables" )
           .Include( "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js" ) );

         bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
                     "~/Scripts/jquery.unobtrusive*",
                     "~/Scripts/jquery.validate*" ) );

         // Use the development version of Modernizr to develop with and learn from. Then, when you're
         // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
         bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                     "~/Scripts/modernizr-*" ) );

         bundles.Add( new StyleBundle( "~/Content/themes/cupertino/css" ).Include(
                     "~/Content/themes/cupertino/jquery-ui.css" ) );

         bundles.Add( new StyleBundle( "~/Content/themes/jqueryDataTables/css" )
            .Include( "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css" )
            .Include( "~/Content/DataTables-1.9.4/media/css/jquery.dataTables_themeroller.css" ) );

         bundles.Add( new StyleBundle( "~/Content/css" )
            .Include( "~/Content/Site.css" )
            .Include( "~/Content/Detail.css" )
            .Include( "~/Content/Editor.css" )
            .Include( "~/Content/HeaderFooter.css" ) );
      }
   }
}