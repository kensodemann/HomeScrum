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
   public class WorkItemStatusesController : SystemDataObjectController<WorkItemStatus>
   {
      [Inject]
      public WorkItemStatusesController( IRepository<WorkItemStatus, Guid> repository, IValidator<WorkItemStatus> validator )
         : base( repository, validator ) { }
   }
}
