using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class ValidationListener : NHibernate.Event.IPreUpdateEventListener, NHibernate.Event.IPreInsertEventListener
   {
      public bool OnPreUpdate( NHibernate.Event.PreUpdateEvent @event )
      {
         ValidateForTransactionType( @event.Entity, TransactionType.Update );
         return false;
      }

      public bool OnPreInsert( NHibernate.Event.PreInsertEvent @event )
      {
         ValidateForTransactionType( @event.Entity, TransactionType.Insert );
         return false;
      }


      private static void ValidateForTransactionType( object entity, TransactionType transactionType )
      {
         var model = entity as IValidatable;
         if (model != null)
         {
            if (!model.IsValidFor( transactionType ))
            {
               throw new InvalidOperationException( "The model is not valid for update" );
            }
         }
      }
   }
}
