﻿using HomeScrum.Data.Domain;
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
      public ProjectsController( IValidator<Project> validator, IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( validator, translator, logger, sessionFactory ) { }

      protected override void PopulateSelectLists( ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( viewModel );
         viewModel.Statuses = ActiveProjectStatuses( viewModel.StatusId );
      }

      private IEnumerable<SelectListItem> ActiveProjectStatuses( Guid selectedId )
      {
         using (var session = SessionFactory.OpenSession())
         {
            return session
               .CreateCriteria( typeof( ProjectStatus ) )
               .Add( Restrictions.Eq( "StatusCd", 'A' ) )
               .SetProjection( Projections.ProjectionList()
                  .Add( Projections.Cast( NHibernateUtil.String, Projections.Property( "Id" ) ), "Value" )
                  .Add( Projections.Property( "Name" ), "Text" )
                  .Add( Projections.Conditional( Restrictions.Eq( "Id", selectedId ), Projections.Constant( true ), Projections.Constant( false ) ), "Selected" ) )
               .SetResultTransformer( Transformers.AliasToBean<SelectListItem>() )
               .AddOrder( Order.Asc( "SortSequence" ) )
               .List<SelectListItem>();
         }
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