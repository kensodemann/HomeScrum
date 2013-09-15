using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
