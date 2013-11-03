using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectStatusEditorViewModel : SystemDomainObjectViewModel, IEditorViewModel
   {
      [Display( Name = "ProjectStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual ProjectStatusCategory Category { get; set; }

      public EditMode Mode { get; set; }
   }
}