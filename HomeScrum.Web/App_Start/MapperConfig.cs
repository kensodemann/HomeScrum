using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using HomeScrum.Web.Models;
using HomeScrum.Web.Models.Base;
using Ninject;

namespace HomeScrum.Web
{
   public class MapperConfig
   {
      public static void RegisterMappings()
      {
         MapDomainsToViewModels();
         MapDomainsToEditorViewModels();

         // TODO: Strongly consider removing these.  We really should have no reason to map in this direction.
         //       If we find ourselves mapping in this direction, we need to ask why.
         MapViewModelsToDomainObjects();

         MapEditorViewModelsToDomains();
      }


      private static void MapEditorViewModelsToDomains()
      {
         Mapper.CreateMap<DomainObjectEditorViewModel, DomainObjectBase>()
            .Include<SystemDomainObjectEditorViewModel, SystemDomainObject>()
            .Include<ProjectEditorViewModel, Project>();
         Mapper.CreateMap<SystemDomainObjectEditorViewModel, SystemDomainObject>()
            .Include<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>()
            .Include<ProjectStatusEditorViewModel, ProjectStatus>()
            .Include<SprintStatusEditorViewModel, SprintStatus>()
            .Include<WorkItemStatusEditorViewModel, WorkItemStatus>()
            .Include<WorkItemTypeEditorViewModel, WorkItemType>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<EditorStatusCodeResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>();
         Mapper.CreateMap<ProjectStatusEditorViewModel, ProjectStatus>();
         Mapper.CreateMap<SprintStatusEditorViewModel, SprintStatus>();
         Mapper.CreateMap<WorkItemStatusEditorViewModel, WorkItemStatus>();
         Mapper.CreateMap<WorkItemTypeEditorViewModel, WorkItemType>();

         Mapper.CreateMap<ProjectEditorViewModel, Project>()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.MapFrom( src => src.LastModifiedUserId ) )
            .ForMember( dest => dest.ProjectStatus, opt => opt.ResolveUsing<ProjectStatusResolver>() )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<UserEditorViewModel, User>()
            .Include<CreateUserViewModel, User>()
            .Include<EditUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.Ignore() );
         Mapper.CreateMap<CreateUserViewModel, User>();
         Mapper.CreateMap<EditUserViewModel, User>();
      }

      // TODO: Look at getting rid of this...
      private static void MapViewModelsToDomainObjects()
      {
         Mapper.CreateMap<DomainObjectViewModel, DomainObjectBase>()
            .Include<SystemDomainObjectViewModel, SystemDomainObject>();
         Mapper.CreateMap<SystemDomainObjectViewModel, SystemDomainObject>()
            .Include<AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatus>()
            .Include<ProjectStatusViewModel, ProjectStatus>()
            .Include<SprintStatusViewModel, SprintStatus>()
            .Include<WorkItemStatusViewModel, WorkItemStatus>()
            .Include<WorkItemTypeViewModel, WorkItemType>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatus>();
         Mapper.CreateMap<ProjectStatusViewModel, ProjectStatus>();
         Mapper.CreateMap<SprintStatusViewModel, SprintStatus>();
         Mapper.CreateMap<WorkItemStatusViewModel, WorkItemStatus>();
         Mapper.CreateMap<WorkItemTypeViewModel, WorkItemType>();
      }

      private static void MapDomainsToEditorViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectEditorViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectEditorViewModel>()
            .Include<Project, ProjectEditorViewModel>();
         Mapper.CreateMap<SystemDomainObject, SystemDomainObjectEditorViewModel>()
            .Include<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>()
            .Include<ProjectStatus, ProjectStatusEditorViewModel>()
            .Include<SprintStatus, SprintStatusEditorViewModel>()
            .Include<WorkItemStatus, WorkItemStatusEditorViewModel>()
            .Include<WorkItemType, WorkItemTypeEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>();
         Mapper.CreateMap<ProjectStatus, ProjectStatusEditorViewModel>();
         Mapper.CreateMap<SprintStatus, SprintStatusEditorViewModel>();
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusEditorViewModel>();
         Mapper.CreateMap<WorkItemType, WorkItemTypeEditorViewModel>();

         Mapper.CreateMap<Project, ProjectEditorViewModel>()
            .ForMember( dest => dest.ProjectStatuses, opt => opt.Ignore() )
            .ForMember( dest => dest.LastModifiedUserId, opt => opt.MapFrom( src => src.LastModifiedUserRid ) );
         Mapper.CreateMap<User, CreateUserViewModel>()
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() );
         Mapper.CreateMap<User, EditUserViewModel>()
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() );
      }

      private static void MapDomainsToViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectViewModel>()
            .Include<Project, ProjectViewModel>();
         Mapper.CreateMap<SystemDomainObject, SystemDomainObjectViewModel>()
            .Include<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel>()
            .Include<ProjectStatus, ProjectStatusViewModel>()
            .Include<SprintStatus, SprintStatusViewModel>()
            .Include<WorkItemStatus, WorkItemStatusViewModel>()
            .Include<WorkItemType, WorkItemTypeViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel>();
         Mapper.CreateMap<ProjectStatus, ProjectStatusViewModel>();
         Mapper.CreateMap<SprintStatus, SprintStatusViewModel>();
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusViewModel>();
         Mapper.CreateMap<WorkItemType, WorkItemTypeViewModel>();

         Mapper.CreateMap<Project, ProjectViewModel>();
         // TODO: Need a UserViewModel
      }


      #region Data Repositories
      public static IRepository<ProjectStatus> ProjectStatusRepository = new Repository<ProjectStatus>();
      #endregion


      #region Resolvers
      public class AllowUseResolver : ValueResolver<SystemDomainObject, bool>
      {
         protected override bool ResolveCore( SystemDomainObject source )
         {
            return source.StatusCd == 'A';
         }
      }

      public class StatusCodeResolver : ValueResolver<SystemDomainObjectViewModel, char>
      {
         protected override char ResolveCore( SystemDomainObjectViewModel source )
         {
            return source.AllowUse ? 'A' : 'I';
         }
      }

      public class EditorStatusCodeResolver : ValueResolver<SystemDomainObjectEditorViewModel, char>
      {
         protected override char ResolveCore( SystemDomainObjectEditorViewModel source )
         {
            return source.AllowUse ? 'A' : 'I';
         }
      }

      public class ProjectStatusResolver : ValueResolver<ProjectEditorViewModel, ProjectStatus>
      {
         [Inject]
         public ProjectStatusResolver( IRepository<ProjectStatus> repository )
         {
            _projectStatusRepository = repository;
         }
         private readonly IRepository<ProjectStatus> _projectStatusRepository;


         protected override ProjectStatus ResolveCore( ProjectEditorViewModel source )
         {
            return _projectStatusRepository.Get( source.ProjectStatusId );
         }
      }
      #endregion
   }
}