using System.ComponentModel;

namespace HomeScrum.Data.Domain
{
   public enum WorkItemStatusCategory
   {
      Unstarted,

      [Description( "In Process" )]
      InProcess,

      Complete
   }
}
