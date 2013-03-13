using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   class AcceptanceCriteriaStatusValidator : SystemDataObjectValidator<AcceptanceCriteriaStatus>
   {
      public AcceptanceCriteriaStatusValidator( IDataObjectRepository<AcceptanceCriteriaStatus> repository )
         : base( repository ) { }
   }
}
