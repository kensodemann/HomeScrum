using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class AcceptanceCriteriaStatusValidator : SystemDataObjectValidator<AcceptanceCriterionStatus>
   {
      public AcceptanceCriteriaStatusValidator( IRepository<AcceptanceCriterionStatus> repository )
         : base( repository ) { }


      protected override string ObjectName { get { return "Acceptance Criteria Status"; } }
   }
}
