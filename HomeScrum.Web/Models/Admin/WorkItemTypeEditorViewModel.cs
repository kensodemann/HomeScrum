using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemTypeEditorViewModel : SystemDomainObjectViewModel, IEditorViewModel
   {
      //[Display( Name = "WorkItemTypeIsTask", ResourceType = typeof( DisplayStrings ) )]
      //public virtual bool IsTask { get; set; }

      public virtual WorkItemTypeCategory Category { get; set; }

      public EditMode Mode { get; set; }
   }
}