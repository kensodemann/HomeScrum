using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class ProjectStatus : SystemDataObject
   {
      public ProjectStatus()
         : base() { }

      public ProjectStatus( ProjectStatus model )
         : base( model )
      {
         this.IsActive = model.IsActive;
      }


      [Display( Name = "ProjectStatusIsActive", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsActive { get; set; }
   }
}
