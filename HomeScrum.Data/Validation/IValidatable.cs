using System;
using System.Collections.Generic;

namespace HomeScrum.Data.Validation
{
   public interface IValidatable
   {
      bool IsValidFor( TransactionType transactionType );
      IDictionary<String, String> GetErrorMessages();
   }
}
