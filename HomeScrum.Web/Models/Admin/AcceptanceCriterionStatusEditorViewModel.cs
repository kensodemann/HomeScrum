using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class AcceptanceCriterionStatusEditorViewModel : SystemDomainObjectViewModel, IEditorViewModel
   {
      [Display( Name = "AcceptanceCriterionStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual AcceptanceCriterionStatusCategory Category { get; set; }

      public EditMode Mode { get; set; }
   }
}