using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Admin;

namespace HomeScrum.Web.Translators
{
   public class ProjectPropertyNameTranslator:PropertyNameTranslator<Project, ProjectEditorViewModel>
   {
      public ProjectPropertyNameTranslator()
         : base()
      {
         this.AddTranslation( "LastModifiedUserRid", "LastModifiedUserId" );
      }
   }
}