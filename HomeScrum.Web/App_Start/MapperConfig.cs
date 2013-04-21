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
         MapDomainsToEditorViewModels();
         MapDomainsToViewModels();

         MapEditorViewModelsToDomains();
      }


      private static void MapEditorViewModelsToDomains()
      {
         Mapper.CreateMap<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
         Mapper.CreateMap<ProjectStatusEditorViewModel, ProjectStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
         Mapper.CreateMap<SprintStatusEditorViewModel, SprintStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
         Mapper.CreateMap<WorkItemStatusEditorViewModel, WorkItemStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
         Mapper.CreateMap<WorkItemTypeEditorViewModel, WorkItemType>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );

         Mapper.CreateMap<ProjectEditorViewModel, Project>()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.MapFrom( src => src.LastModifiedUserId ) )
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<ProjectStatusResolver>() )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<WorkItemEditorViewModel, WorkItem>()
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<WorkItemStatusResolver>() )
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.WorkItemType, opt => opt.Ignore() )
            .ForMember( dest => dest.Project, opt => opt.Ignore() )
            .ForMember( dest => dest.ParentWorkItem, opt => opt.Ignore() )
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.Ignore() )
            .ForMember( dest => dest.CreatedByUser, opt => opt.Ignore() )
            .ForMember( dest => dest.AssignedToUser, opt => opt.Ignore() )
            .ForMember( dest => dest.AcceptanceCriteria, opt => opt.Ignore() );

         Mapper.CreateMap<CreateUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<UserStatusResolver>() );
         Mapper.CreateMap<EditUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<UserStatusResolver>() );
      }


      private static void MapDomainsToEditorViewModels()
      {
         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<ProjectStatus, ProjectStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<SprintStatus, SprintStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<WorkItemType, WorkItemTypeEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<Project, ProjectEditorViewModel>()
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.LastModifiedUserId, opt => opt.MapFrom( src => src.LastModifiedUserRid ) );

         Mapper.CreateMap<WorkItem, WorkItemEditorViewModel>()
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() );

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
         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<ProjectStatus, ProjectStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<SprintStatus, SprintStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );
         Mapper.CreateMap<WorkItemType, WorkItemTypeViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<AcceptanceCriteria, AcceptanceCriteriaViewModel>()
            .ForMember( dest => dest.IsAccepted, opt => opt.MapFrom( src => src.Status.IsAccepted ) );
         Mapper.CreateMap<Project, ProjectViewModel>();
         Mapper.CreateMap<WorkItem, WorkItemViewModel>()
            .ForMember( dest => dest.AssignedToUserName, opt => opt.MapFrom( src => src.AssignedToUser.UserName ) )
            .ForMember( dest => dest.CreatedByUserName, opt => opt.MapFrom( src => src.CreatedByUser.UserName ) );

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

      public class StatusCodeResolver : ValueResolver<SystemDomainObjectViewModel, char>
      {
         protected override char ResolveCore( SystemDomainObjectViewModel source )
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

      public class WorkItemStatusResolver : ValueResolver<WorkItemEditorViewModel, WorkItemStatus>
      {
         [Inject]
         public WorkItemStatusResolver( IRepository<WorkItemStatus> repository )
         {
            _respository = repository;
         }
         private readonly IRepository<WorkItemStatus> _respository;


         protected override WorkItemStatus ResolveCore( WorkItemEditorViewModel source )
         {
            return _respository.Get( source.StatusId );
         }
      }
      #endregion
   }
}