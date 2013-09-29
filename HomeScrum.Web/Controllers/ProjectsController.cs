using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectEditorViewModel>
   {
      public ProjectsController( IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      protected override void PopulateSelectLists( ISession session, ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( session, viewModel );
         viewModel.Statuses = ActiveProjectStatuses( session, viewModel.StatusId );
      }

      private IEnumerable<SelectListItem> ActiveProjectStatuses( ISession session, Guid selectedId )
      {
         var queryModel = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ProjectStatus>()
         {
            SelectedId = selectedId
         };

         return queryModel.GetQuery( session )
            .SelectSelectListItems( selectedId );
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
         return session.Query<User>()
            .Single( x => x.UserName == p.Identity.Name ).Id;
      }
   }
}