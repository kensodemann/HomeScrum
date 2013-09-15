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
      [Description( "Backlog Item - Product Features and Requirements" )]
      BacklogItem,

      [Description( "Task - Work required to accomplish features" )]
      Task,

      [Description( "Issue - A Problem or Bug" )]
      Issue
   }
}
