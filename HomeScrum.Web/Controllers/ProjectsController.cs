using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IRepository<ProjectStatus> projectStatusRepository, IValidator<Project> validator,
         IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( validator, translator, logger, sessionFactory )
      {
         _projectStatusRepository = projectStatusRepository;
      }

      private readonly IRepository<ProjectStatus> _projectStatusRepository;
      
      protected override void PopulateSelectLists( ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( viewModel );
         viewModel.Statuses = _projectStatusRepository.GetAll().ToSelectList( viewModel.StatusId );
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