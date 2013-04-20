using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.WorkItems;
using Ninject;

namespace HomeScrum.Web
{
   public class MapperConfig
   {
      public static void RegisterMappings()
      {
         MapDomainsToViewModels();
         MapDomainsToEditorViewModels();

         MapEditorViewModelsToDomains();
      }


      private static void MapEditorViewModelsToDomains()
      {
         Mapper.CreateMap<SystemDomainObjectEditorViewModel, SystemDomainObject>()
            .Include<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>()
            .Include<ProjectStatusEditorViewModel, ProjectStatus>()
            .Include<SprintStatusEditorViewModel, SprintStatus>()
            .Include<WorkItemStatusEditorViewModel, WorkItemStatus>()
            .Include<WorkItemTypeEditorViewModel, WorkItemType>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
         Mapper.CreateMap<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>();
         Mapper.CreateMap<ProjectStatusEditorViewModel, ProjectStatus>();
         Mapper.CreateMap<SprintStatusEditorViewModel, SprintStatus>();
         Mapper.CreateMap<WorkItemStatusEditorViewModel, WorkItemStatus>();
         Mapper.CreateMap<WorkItemTypeEditorViewModel, WorkItemType>();

         Mapper.CreateMap<ProjectEditorViewModel, Project>()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.MapFrom( src => src.LastModifiedUserId ) )
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<ProjectStatusResolver>() )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<CreateUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<UserStatusResolver>() );
         Mapper.CreateMap<EditUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<UserStatusResolver>() );
      }


      private static void MapDomainsToEditorViewModels()
      {
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
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<IsActiveUserResolver>() );
         Mapper.CreateMap<User, EditUserViewModel>()
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<IsActiveUserResolver>() );
      }

      private static void MapDomainsToViewModels()
      {
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
         Mapper.CreateMap<WorkItem, WorkItemViewModel>()
            .ForMember( dest => dest.AssignedToUserName, opt => opt.MapFrom( src => src.AssignedToUser.UserName ) )
            .ForMember( dest => dest.CreatedByUserName, opt => opt.MapFrom( src => src.CreatedByUser.UserName ) );

         Mapper.CreateMap<User, DisplayViewModel>()
            .Include<User, UserViewModel>();
         Mapper.CreateMap<User, UserViewModel>()
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<IsActiveUserResolver>() );
      }


      #region Resolvers
      public class AllowUseResolver : ValueResolver<SystemDomainObject, bool>
      {
         protected override bool ResolveCore( SystemDomainObject source )
         {
            return source.StatusCd == 'A';
         }
      }

      public class StatusCodeResolver : ValueResolver<SystemDomainObjectEditorViewModel, char>
      {
         protected override char ResolveCore( SystemDomainObjectEditorViewModel source )
         {
            return source.AllowUse ? 'A' : 'I';
         }
      }

      public class IsActiveUserResolver : ValueResolver<User, bool>
      {
         protected override bool ResolveCore( User source )
         {
            return source.StatusCd == 'A';
         }
      }

      public class UserStatusResolver : ValueResolver<UserEditorViewModel, char>
      {
         protected override char ResolveCore( UserEditorViewModel source )
         {
            return source.IsActive ? 'A' : 'I';
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
            return _projectStatusRepository.Get( source.StatusId );
         }
      }
      #endregion
   }
}