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
   public class SprintStatusesController : SystemDataObjectController<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>
   {
      [Inject]
      public SprintStatusesController( IPropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      protected override IQueryable<SprintStatusViewModel> SelectViewModels( IQueryable<SprintStatus> query )
      {
         return query.Select( x => new SprintStatusViewModel()
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
         ViewBag.EditorTitle = "New Sprint Status";
         return base.Create( callingController, callingAction, callingId, parentWorkItemId );
      }

      public override System.Web.Mvc.ActionResult Edit( System.Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         ViewBag.EditorTitle = "Sprint Status";
         return base.Edit( id, callingController, callingAction, callingId );
      }
   }
}
