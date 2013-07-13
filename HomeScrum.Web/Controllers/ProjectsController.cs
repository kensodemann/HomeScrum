using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Principal;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IRepository<Project> repository, IRepository<ProjectStatus> projectStatusRepository, IUserRepository userRepository,
         IValidator<Project> validator, IPropertyNameTranslator<Project, ProjectEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( repository, validator, translator, logger, sessionFactory )
      {
         _projectStatusRepository = projectStatusRepository;
         _userRepository = userRepository;
      }

      // Temporarily here like this, will need to refactor to project into a list of domain objects
      public override System.Web.Mvc.ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<Project>, IEnumerable<ProjectViewModel>>( items ) );
      }

      private readonly IRepository<ProjectStatus> _projectStatusRepository;
      private readonly IUserRepository _userRepository;

      protected override void PopulateSelectLists( ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( viewModel );
         viewModel.Statuses = _projectStatusRepository.GetAll().ToSelectList( viewModel.StatusId );
      }


      protected override void AddItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.AddItem( model, user );
      }

      protected override void UpdateItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.UpdateItem( model, user );
      }
   }
}