using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemTypesController : DomainObjectController<WorkItemType>
   {
      [Inject]
      public WorkItemTypesController( IRepository<WorkItemType> repository, IValidator<WorkItemType> validator )
         : base( repository, validator ) { }
   }
}
