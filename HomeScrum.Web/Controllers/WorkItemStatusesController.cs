using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemStatusesController : DomainObjectController<WorkItemStatus, WorkItemStatusEditorViewModel>
   {
      [Inject]
      public WorkItemStatusesController( IRepository<WorkItemStatus, Guid> repository, IValidator<WorkItemStatus> validator )
         : base( repository, validator ) { }
   }
}
