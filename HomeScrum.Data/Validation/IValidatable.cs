using System;
using System.Collections.Generic;

namespace HomeScrum.Data.Validation
{
   internal interface IValidatable
   {
      bool IsValidFor( TransactionType transactionType );
      IDictionary<String, String> GetErrorMessages();
   }
}
