using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class SprintStatusesController : ReadWriteController<SprintStatus>
   {
      [Inject]
      public SprintStatusesController( IRepository<SprintStatus> repository, IValidator<SprintStatus> validator )
         : base( repository, validator ) { }
   }
}
