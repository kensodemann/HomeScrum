using System.ComponentModel;

namespace HomeScrum.Data.Domain
{
   public enum WorkItemTypeCategory
   {
      [Description( "Backlog Item" )]
      BacklogItem,

      Task,

      [Description( "Issue, Problem, or Bug" )]
      Issue
   }
}
