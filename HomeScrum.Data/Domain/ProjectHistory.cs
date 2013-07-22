using System;

namespace HomeScrum.Data.Domain
{
   public class ProjectHistory : DomainObjectBase
   {
      public ProjectHistory()
         : base( null ) { }

      public virtual Guid LastModifiedUserRid { get; set; }

      public virtual ProjectStatus ProjectStatus { get; set; }

      public virtual Guid ProjectRid { get; set; }

      public virtual DateTime HistoryTimestamp { get; set; }

      public virtual int SequenceNumber { get; set; }
   }
}
