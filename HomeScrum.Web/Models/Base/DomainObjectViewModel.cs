using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Base
{
   public class DomainObjectViewModel : DisplayViewModel
   {
      [Display( Name = "Name", Prompt = "NamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Name { get; set; }

      [Display( Name = "Description", Prompt = "DescriptionPrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Description { get; set; }
   }
}