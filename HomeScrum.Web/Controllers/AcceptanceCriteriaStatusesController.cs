using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using Ninject;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : SystemDataObjectController<AcceptanceCriteriaStatus, Guid>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriteriaStatus, Guid> repository, IValidator<AcceptanceCriteriaStatus> validator )
         : base( repository, validator ) { }
   }
}
