using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemStatusEditorViewModel : SystemDomainObjectViewModel, IEditorViewModel
   {
      //[Display( Name = "WorkItemStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      //public virtual bool IsOpenStatus { get; set; }

      public virtual WorkItemStatusCategory Category { get; set; }

      public EditMode Mode { get; set; }
   }
}