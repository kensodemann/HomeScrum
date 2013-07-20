using System;
using System.Collections.Generic;

namespace HomeScrum.Data.Validators
{
   internal interface IValidatable
   {
      bool IsValidFor( TransactionType transactionType );
      IDictionary<String, String> GetErrorMessages();
   }
}
