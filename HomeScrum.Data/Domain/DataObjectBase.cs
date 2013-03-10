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

      [Display( Name = "Name", Prompt = "Enter a unique name" )]
      public virtual string Name { get; set; }

      [Display( Name = "Description", Prompt = "Enter a short description" )]
      public virtual string Description { get; set; }
   }
}
