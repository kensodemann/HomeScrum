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
   public class WorkItemStatusesController : ReadWriteController<WorkItemStatus>
   {
      [Inject]
      public WorkItemStatusesController( IRepository<WorkItemStatus> repository, IValidator<WorkItemStatus> validator )
         : base( repository, validator ) { }

      //
      // GET: /WorkItemStatuses/
      public override ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<WorkItemStatus>, IEnumerable<WorkItemStatusViewModel>>( items ) );
      }
   }
}
