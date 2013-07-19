using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   internal interface IValidatable
   {
      bool IsValidFor( TransactionType transactionType );
   }
}
