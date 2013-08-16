﻿using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Web.Extensions;

namespace HomeScrum.Web.Controllers
{
    public class SprintsController : ReadWriteController<Sprint, SprintViewModel, SprintEditorViewModel>
    {
       public SprintsController( IPropertyNameTranslator<Sprint, SprintEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }


       protected override void PopulateSelectLists( ISession session, SprintEditorViewModel viewModel )
       {
          viewModel.Statuses = CreateSprintStatusSelectList( session, viewModel.StatusId );
          viewModel.Projects = CreateProjectsSelectList( session, viewModel.ProjectId );
          base.PopulateSelectLists( session, viewModel );
       }

       private IEnumerable<SelectListItem> CreateSprintStatusSelectList( ISession session, Guid selectedId )
       {
          var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<SprintStatus>() { SelectedId = selectedId };

          return query
             .GetQuery( session )
             .SelectSelectListItems<SprintStatus>( selectedId );
       }

       private IEnumerable<SelectListItem> CreateProjectsSelectList( ISession session, Guid selectedId )
       {
          return session.Query<Project>()
              .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsActive) || x.Id == selectedId )
              .OrderBy( x => x.Status.SortSequence )
              .ThenBy( x => x.Name.ToUpper() )
              .SelectSelectListItems<Project>( selectedId );
       }

       protected override void Save( ISession session, Sprint model, System.Security.Principal.IPrincipal user )
       {
          model.LastModifiedUserRid = GetUserId( session, user.Identity.Name ); 
          base.Save( session, model, user );
       }

       // TODO: Make extention to IPrincipal, replace this here and in WorkItem
       private Guid GetUserId( ISession session, string userName )
       {
          return session.Query<User>()
             .Single( x => x.UserName == userName ).Id;
       }
    }
}
