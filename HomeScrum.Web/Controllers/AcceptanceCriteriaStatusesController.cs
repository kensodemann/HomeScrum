using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : DomainObjectController<AcceptanceCriteriaStatus>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriteriaStatus> repository, IValidator<AcceptanceCriteriaStatus> validator )
         : base( repository, validator ) { }
   }
}
