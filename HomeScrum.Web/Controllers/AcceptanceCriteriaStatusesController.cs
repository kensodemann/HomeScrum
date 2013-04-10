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
   public class AcceptanceCriteriaStatusesController : ReadWriteController<AcceptanceCriteriaStatus>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriteriaStatus> repository, IValidator<AcceptanceCriteriaStatus> validator )
         : base( repository, validator ) { }

      //
      // GET: /AcceptanceCriteriaStatuses/
      public override ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<AcceptanceCriteriaStatus>, IEnumerable<AcceptanceCriteriaStatusViewModel>>( items ) );
      }
   }
}
