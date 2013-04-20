using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.WorkItems;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadOnlyController<WorkItem, WorkItemViewModel>
   {
      [Inject]
      public WorkItemsController( IRepository<WorkItem> repository )
         : base( repository ) { }
   }
}