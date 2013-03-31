using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SprintStatus : SystemDataObject
   {
      public SprintStatus()
         : base() { }

      public SprintStatus( SprintStatus model )
         : base( model )
      {
         this.IsOpenStatus = model.IsOpenStatus;
      }

      [Display( Name = "SprintStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsOpenStatus { get; set; }
   }
}
