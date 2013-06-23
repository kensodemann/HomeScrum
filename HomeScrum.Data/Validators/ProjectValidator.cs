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
   public class ProjectValidator : IValidator<Project>
   {
      private readonly IRepository<Project> _repository;

      [Inject]
      public ProjectValidator( IRepository<Project> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( Project model, TransactionType forTransaction )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
