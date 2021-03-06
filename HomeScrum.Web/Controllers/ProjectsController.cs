﻿using HomeScrum.Data.Domain;
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
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      //
      // GET: /Projects/Creae
      public override ActionResult Create( string callingController = null, string callingAction = null, string callingId = null, string parentWorkItemId = null )
      {
         ViewBag.EditorTitle = "New Project";
         return base.Create( callingController, callingAction, callingId, parentWorkItemId );
      }

      //
      // GET: /Projects/Edit/Guid
      public override ActionResult Edit( Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         ViewBag.EditorTitle = "Project";
         return base.Edit( id, callingController, callingAction, callingId );
      }

      protected override void PopulateSelectLists( ISession session, ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( session, viewModel );
         viewModel.Statuses = ActiveProjectStatuses( session, viewModel.StatusId );
      }

      private IEnumerable<SelectListItem> ActiveProjectStatuses( ISession session, Guid selectedId )
      {
         return session.Query<ProjectStatus>()
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .OrderBy( x => x.SortSequence )
            .SelectSelectListItems( selectedId )
            .ToList();
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