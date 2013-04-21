using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.WorkItems;
using Ninject;
using HomeScrum.Data.Validators;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IRepository<WorkItem> repository, IValidator<WorkItem> validator )
         : base( repository, validator ) { }

      //
      // Get: /WorkItems/Create
      public override ActionResult Create()
      {
         var viewModel = new WorkItemEditorViewModel();

         return View( viewModel );
      }
   }
}