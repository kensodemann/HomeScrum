using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   class WorkItemTypeValidator : SystemDataObjectValidator<WorkItemType>
   {
      public WorkItemTypeValidator( IDataObjectRepository<WorkItemType> repository )
         : base( repository ) { }
   }
}
