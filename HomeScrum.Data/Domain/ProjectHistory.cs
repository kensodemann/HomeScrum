using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class ProjectHistory : DomainObjectBase
   {
      public ProjectHistory()
         : base() { }

      public ProjectHistory( ProjectHistory model )
         : base( model )
      {
         this.LastModifiedUserRid = model.LastModifiedUserRid;
         this.ProjectRid = model.ProjectRid;
         this.HistoryTimestamp = model.HistoryTimestamp;
         this.SequenceNumber = model.SequenceNumber;

         this.ProjectStatus = new ProjectStatus( model.ProjectStatus );
      }

      public virtual Guid LastModifiedUserRid { get; set; }

      public virtual ProjectStatus ProjectStatus { get; set; }

      public virtual Guid ProjectRid { get; set; }

      public virtual DateTime HistoryTimestamp { get; set; }

      public virtual int SequenceNumber { get; set; }
   }
}
