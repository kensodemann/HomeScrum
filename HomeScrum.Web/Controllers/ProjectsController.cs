﻿using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;

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
         using (var session = SessionFactory.OpenSession())
         {
            var queryModel = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ProjectStatus>()
            {
               SelectedId = selectedId
            };

            return queryModel.GetQuery( session )
               .Select( Projections.ProjectionList()
                  .Add( Projections.Cast( NHibernateUtil.String, Projections.Property( "Id" ) ), "Value" )
                  .Add( Projections.Property( "Name" ), "Text" )
                  .Add( Projections.Conditional( Restrictions.Eq( "Id", selectedId ), Projections.Constant( true ), Projections.Constant( false ) ), "Selected" ) )
               .TransformUsing( Transformers.AliasToBean<SelectListItem>() )
               .List<SelectListItem>();

            //return session
            //   .CreateCriteria( typeof( ProjectStatus ) )
            //   .Add( Restrictions.Or( Restrictions.Eq( "StatusCd", 'A' ), Restrictions.Eq( "Id", selectedId ) ) )
            //   .SetProjection( Projections.ProjectionList()
            //      .Add( Projections.Cast( NHibernateUtil.String, Projections.Property( "Id" ) ), "Value" )
            //      .Add( Projections.Property( "Name" ), "Text" )
            //      .Add( Projections.Conditional( Restrictions.Eq( "Id", selectedId ), Projections.Constant( true ), Projections.Constant( false ) ), "Selected" ) )
            //   .SetResultTransformer( Transformers.AliasToBean<SelectListItem>() )
            //   .AddOrder( Order.Asc( "SortSequence" ) )
            //   .List<SelectListItem>();
         }
      }


      // TODO: This is just temporary.  Want to move this down so we can get rid of the validators.
      [HttpPost]
      public override ActionResult Edit( ProjectEditorViewModel viewModel, IPrincipal user )
      {
         var model = Mapper.Map<Project>( viewModel );

         if (ModelState.IsValid)
         {
            try
            {
               UpdateItem( model, user );
               return RedirectToAction( () => this.Index() );
            }
            catch (InvalidOperationException)
            {
               foreach (var message in model.GetErrorMessages())
               {
                  var viewModelPropertyName = PropertyNameTranslator.TranslatedName( message.Key );
                  ModelState.AddModelError( viewModelPropertyName, message.Value );
               }
            }
         }

         PopulateSelectLists( viewModel );
         return View( viewModel );
      }


      [HttpPost]
      public override ActionResult Create( ProjectEditorViewModel viewModel, IPrincipal user )
      {
         var model = Mapper.Map<Project>( viewModel );

         if (ModelState.IsValid)
         {
            try
            {
               AddItem( model, user );
               return RedirectToAction( () => this.Index() );
            }
            catch (InvalidOperationException)
            {
               foreach (var message in model.GetErrorMessages())
               {
                  var viewModelPropertyName = PropertyNameTranslator.TranslatedName( message.Key );
                  ModelState.AddModelError( viewModelPropertyName, message.Value );
               }
            }
         }

         PopulateSelectLists( viewModel );
         return View( viewModel );
      }


      protected override void AddItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( user );
         base.AddItem( model, user );
      }

      protected override void UpdateItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( user );
         base.UpdateItem( model, user );
      }

      private Guid GetUserId( IPrincipal p )
      {
         using (var session = SessionFactory.OpenSession())
         {
            var user = session
               .CreateCriteria( typeof( User ) )
               .Add( Expression.Eq( "UserName", p.Identity.Name ) )
               .UniqueResult() as User;
            return user == null ? default( Guid ) : user.Id;
         }
      }
   }
}