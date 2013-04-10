using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class SprintStatusesController : ReadWriteController<SprintStatus>
   {
      [Inject]
      public SprintStatusesController( IRepository<SprintStatus> repository, IValidator<SprintStatus> validator )
         : base( repository, validator ) { }

      //
      // GET: /SprintStatuses/
      public override ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<SprintStatus>, IEnumerable<SprintStatusViewModel>>( items ) );
      }
   }
}
