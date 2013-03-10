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
      public virtual Guid Id { get; set; }

      [Display( Name = "Name", Prompt = "NamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Name { get; set; }

      [Display( Name = "Description", Prompt = "DescriptionPrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Description { get; set; }
   }
}
