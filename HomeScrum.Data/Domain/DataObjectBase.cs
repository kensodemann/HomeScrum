using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class DataObjectBase
   {
      public DataObjectBase() { }

      public DataObjectBase( DataObjectBase model )
      {
         this.Id = model.Id;
         this.Name = model.Name;
         this.Description = model.Description;
      }

      public virtual Guid Id { get; set; }

      [Display( Name = "Name", Prompt = "NamePrompt", ResourceType = typeof( DisplayStrings ) )]
      [Required( ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string Name { get; set; }

      [Display( Name = "Description", Prompt = "DescriptionPrompt", ResourceType = typeof( DisplayStrings ) )]
      [UIHint( "MultilineText" )]
      public virtual string Description { get; set; }
   }
}
