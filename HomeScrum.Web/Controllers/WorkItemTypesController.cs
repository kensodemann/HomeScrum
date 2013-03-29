using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemTypesController : DomainObjectController<WorkItemType, WorkItemTypeEditorViewModel>
   {
      [Inject]
      public WorkItemTypesController( IRepository<WorkItemType, Guid> repository, IValidator<WorkItemType> validator )
         : base( repository, validator ) { }
   }
}
