using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using Ninject;

namespace HomeScrum.Data.Validators
{
   public class WorkItemValidator : IValidator<WorkItem>
   {
      private readonly IWorkItemRepository _repository;

      [Inject]
      public WorkItemValidator( IWorkItemRepository repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( WorkItem model, TransactionType forTransaction )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
