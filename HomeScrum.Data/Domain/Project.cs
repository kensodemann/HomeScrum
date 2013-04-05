using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class Project : DataObjectBase
   {
      public Project()
         : base() { }

      public Project( Project model )
         : base( model )
      {
         this.LastModifiedUserRid = model.LastModifiedUserRid;

         if (model.ProjectStatus != null)
         {
            this.ProjectStatus = new ProjectStatus( model.ProjectStatus );
         }
      }

      [Required]
      public virtual ProjectStatus ProjectStatus { get; set; }

      public virtual Guid LastModifiedUserRid { get; set; }
   }
}
