using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      protected override void PopulateSelectLists( ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( viewModel );
         viewModel.Statuses = ActiveProjectStatuses( viewModel.StatusId );
      }

      private IEnumerable<SelectListItem> ActiveProjectStatuses( Guid selectedId )
      {
         var queryModel = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ProjectStatus>()
         {
            SelectedId = selectedId
         };

         var session = SessionFactory.GetCurrentSession();

         return queryModel.GetQuery( session )
            .Select( Projections.ProjectionList()
               .Add( Projections.Cast( NHibernateUtil.String, Projections.Property( "Id" ) ), "Value" )
               .Add( Projections.Property( "Name" ), "Text" )
               .Add( Projections.Conditional( Restrictions.Eq( "Id", selectedId ), Projections.Constant( true ), Projections.Constant( false ) ), "Selected" ) )
            .TransformUsing( Transformers.AliasToBean<SelectListItem>() )
            .List<SelectListItem>();
      }


      protected override void Save( ISession session, Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( session, user );
         base.Save( session, model, user );
      }

      protected override void Update( ISession session, Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( session, user );
         base.Update( session, model, user );
      }

      private Guid GetUserId( ISession session, IPrincipal p )
      {
         var user = session
            .CreateCriteria( typeof( User ) )
            .Add( Expression.Eq( "UserName", p.Identity.Name ) )
            .UniqueResult() as User;
         return user == null ? default( Guid ) : user.Id;
      }
   }
}