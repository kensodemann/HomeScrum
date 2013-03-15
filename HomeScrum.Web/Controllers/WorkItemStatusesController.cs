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
   public class WorkItemStatusesController : DataObjectBaseController<WorkItemStatus>
   {
      [Inject]
      public WorkItemStatusesController( IDataObjectRepository<WorkItemStatus> repository, IValidator<WorkItemStatus> validator )
         : base( repository, validator ) { }
   }
}
