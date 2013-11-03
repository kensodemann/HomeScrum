using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class SprintStatusEditorViewModel : SystemDomainObjectViewModel, IEditorViewModel
   {
      [Display( Name = "SprintStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual SprintStatusCategory Category { get; set; }

      [Display( Name = "SprintStatusAllowNewBacklogItems", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool CanAddBacklogItems { get; set; }

      [Display( Name = "SprintStatusAllowNewTaskListItems", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool CanAddTaskListItems { get; set; }

      public EditMode Mode { get; set; }
   }
}