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
   public class ProjectStatusesController : SystemDataObjectController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      [Inject]
      public ProjectStatusesController( IPropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger , sessionFactory) { }

      protected override IQueryable<ProjectStatusViewModel> SelectViewModels( IQueryable<ProjectStatus> query )
      {
         return query.Select( x => new ProjectStatusViewModel()
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
         ViewBag.EditorTitle = "New Project Status";
         return base.Create( callingController, callingAction, callingId, parentWorkItemId );
      }

      public override System.Web.Mvc.ActionResult Edit( System.Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         ViewBag.EditorTitle = "Project Status";
         return base.Edit( id, callingController, callingAction, callingId );
      }
   }
}
