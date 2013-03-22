using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class WorkItemStatusValidator : SystemDataObjectValidator<WorkItemStatus>
   {
      public WorkItemStatusValidator( IRepository<WorkItemStatus, Guid> repository )
         : base( repository ) { }

      protected override string ObjectName { get { return "Work Item Status"; } }
   }
}
