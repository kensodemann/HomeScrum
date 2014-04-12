using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;


namespace HomeScrum.Web.Controllers.Admin
{
   public class WorkItemStatusesController : SystemDataObjectController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      [Inject]
      public WorkItemStatusesController( IPropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      protected override IQueryable<WorkItemStatusViewModel> SelectViewModels( IQueryable<WorkItemStatus> query )
      {
         return query.Select( x => new WorkItemStatusViewModel()
         {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            AllowUse = (x.StatusCd == 'A'),
            IsPredefined = x.IsPredefined,
            Category = EnumHelper.GetDescription( x.Category )
         } );
      }

      public override System.Web.Mvc.ActionResult Create( string callingController = null, string callingAction = null, string callingId = null, string parentWorkItemId = null )
      {
         ViewBag.EditorTitle = "New Work Item Status";
         return base.Create( callingController, callingAction, callingId, parentWorkItemId );
      }

      public override System.Web.Mvc.ActionResult Edit( System.Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         ViewBag.EditorTitle = "Work Item Status";
         return base.Edit( id, callingController, callingAction, callingId );
      }
   }
}
