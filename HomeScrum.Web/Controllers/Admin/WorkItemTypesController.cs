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
   public class WorkItemTypesController : SystemDataObjectController<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>
   {
      [Inject]
      public WorkItemTypesController( IPropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      protected override IQueryable<WorkItemTypeViewModel> SelectViewModels( IQueryable<WorkItemType> query )
      {
         return query.Select( x => new WorkItemTypeViewModel()
         {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            AllowUse = (x.StatusCd == 'A'),
            IsPredefined = x.IsPredefined,
            Category = EnumHelper.GetDescription( x.Category )
         } );
      }
   }
}
