using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.WorkItems;
using Ninject;
using System;
using AutoMapper.Mappers;
using HomeScrum.Common.Utility;
using NHibernate;
using HomeScrum.Web.Models.Sprints;

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
         Mapper.CreateMap<AcceptanceCriterionStatusEditorViewModel, AcceptanceCriterionStatus>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<ProjectStatusEditorViewModel, ProjectStatus>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<SprintStatusEditorViewModel, SprintStatus>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.BacklogIsClosed, opt => opt.MapFrom( src => !src.CanAddBacklogItems ) )
            .ForMember( dest => dest.TaskListIsClosed, opt => opt.MapFrom( src => !src.CanAddTaskListItems ) )
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<WorkItemStatusEditorViewModel, WorkItemStatus>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<WorkItemTypeEditorViewModel, WorkItemType>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );

         Mapper.CreateMap<SprintEditorViewModel, Sprint>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.Ignore() )
            .ForMember( dest => dest.CreatedByUser, opt => opt.ResolveUsing<DomainModelResolver<User>>().FromMember( src => src.CreatedByUserId ) )
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<DomainModelResolver<SprintStatus>>().FromMember( src => src.StatusId ) )
            .ForMember( dest => dest.Project, opt => opt.ResolveUsing<DomainModelResolver<Project>>().FromMember( src => src.ProjectId ) );

         Mapper.CreateMap<ProjectEditorViewModel, Project>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.MapFrom( src => src.LastModifiedUserId ) )
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<DomainModelResolver<ProjectStatus>>().FromMember( src => src.StatusId ) )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<WorkItemEditorViewModel, WorkItem>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<DomainModelResolver<WorkItemStatus>>().FromMember( src => src.StatusId ) )
            .ForMember( dest => dest.WorkItemType, opt => opt.ResolveUsing<DomainModelResolver<WorkItemType>>().FromMember( src => src.WorkItemTypeId ) )
            .ForMember( dest => dest.Project, opt => opt.ResolveUsing<DomainModelResolver<Project>>().FromMember( src => src.ProjectId ) )
            .ForMember( dest => dest.ParentWorkItem, opt => opt.ResolveUsing<DomainModelResolver<WorkItem>>().FromMember( src => src.ParentWorkItemId ) )
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.Ignore() )
            .ForMember( dest => dest.CreatedByUser, opt => opt.ResolveUsing<DomainModelResolver<User>>().FromMember( src => src.CreatedByUserId ) )
            .ForMember( dest => dest.AssignedToUser, opt => opt.ResolveUsing<DomainModelResolver<User>>().FromMember( src => src.AssignedToUserId ) )
            .ForMember( dest => dest.AcceptanceCriteria, opt => opt.Ignore() )
            .ForMember( dest => dest.Sprint, opt => opt.ResolveUsing<DomainModelResolver<Sprint>>().FromMember( src => src.SprintId ) )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<CreateUserViewModel, User>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.IsActive ) );
         Mapper.CreateMap<EditUserViewModel, User>()
            .ConstructUsingServiceLocator()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.IsActive ) );
      }


      private static void MapDomainsToEditorViewModels()
      {
         Mapper.CreateMap<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<ProjectStatus, ProjectStatusEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<SprintStatus, SprintStatusEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.CanAddBacklogItems, opt => opt.MapFrom( src => !src.BacklogIsClosed ) )
            .ForMember( dest => dest.CanAddTaskListItems, opt => opt.MapFrom( src => !src.TaskListIsClosed ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemType, WorkItemTypeEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );

         Mapper.CreateMap<Project, ProjectEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.LastModifiedUserId, opt => opt.MapFrom( src => src.LastModifiedUserRid ) );

         Mapper.CreateMap<Sprint, SprintEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.Projects, opt => opt.Ignore() )
            .ForMember( dest => dest.BacklogItems, opt => opt.Ignore() )
            .ForMember( dest => dest.Tasks, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectProjectId, opt => opt.Ignore() );

         Mapper.CreateMap<WorkItem, WorkItemEditorViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectWorkItemTypeId, opt => opt.Ignore() )
            .ForMember( dest => dest.WorkItemTypes, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectProjectId, opt => opt.Ignore() )
            .ForMember( dest => dest.Projects, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectSprintId, opt => opt.Ignore() )
            .ForMember( dest => dest.Sprints, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectAssignedToUserId, opt => opt.Ignore() )
            .ForMember( dest => dest.AssignedToUsers, opt => opt.Ignore() )
            .ForMember( dest => dest.SelectParentWorkItemId, opt => opt.Ignore() )
            .ForMember( dest => dest.ProductBacklogItems, opt => opt.Ignore() )
            .ForMember( dest => dest.Tasks, opt => opt.Ignore() );

         Mapper.CreateMap<User, CreateUserViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<User, EditUserViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
      }


      private static void MapDomainsToViewModels()
      {
         Mapper.CreateMap<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Category, opt => opt.MapFrom( src => EnumHelper.GetDescription( src.Category ) ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<ProjectStatus, ProjectStatusViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            //.ForMember( dest => dest.Category, opt => opt.MapFrom( src => EnumHelper.GetDescription( src.Category ) ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<SprintStatus, SprintStatusViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            //.ForMember( dest => dest.Category, opt => opt.MapFrom( src => EnumHelper.GetDescription( src.Category ) ) )
            .ForMember( dest => dest.CanAddBacklogItems, opt => opt.MapFrom( src => !src.BacklogIsClosed ) )
            .ForMember( dest => dest.CanAddTaskListItems, opt => opt.MapFrom( src => !src.TaskListIsClosed ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Category, opt => opt.MapFrom( src => EnumHelper.GetDescription( src.Category ) ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemType, WorkItemTypeViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.Category, opt => opt.MapFrom( src => EnumHelper.GetDescription( src.Category ) ) )
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );

         Mapper.CreateMap<Sprint, SprintViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.IsComplete, opt => opt.MapFrom( src => src.Status.Category == SprintStatusCategory.Complete ) )
            .ForMember( dest => dest.CanAddBacklog, opt => opt.MapFrom( src => !src.Status.BacklogIsClosed ) )
            .ForMember( dest => dest.CanAddTasks, opt => opt.MapFrom( src => !src.Status.TaskListIsClosed ) );

         Mapper.CreateMap<AcceptanceCriterion, AcceptanceCriterionViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.IsAccepted, opt => opt.MapFrom( src => src.Status.Category == AcceptanceCriterionStatusCategory.VerificationPassed ) );

         Mapper.CreateMap<Project, ProjectViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() );

         Mapper.CreateMap<WorkItem, WorkItemViewModel>()
            .ForMember( dest => dest.Tasks, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.IsComplete, opt => opt.MapFrom( src => src.Status.Category == WorkItemStatusCategory.Complete ) )
            .ForMember( dest => dest.AssignedToUserName, opt => opt.MapFrom( src => src.AssignedToUser.UserName ) )
            .ForMember( dest => dest.CreatedByUserName, opt => opt.MapFrom( src => src.CreatedByUser.UserName ) );
         Mapper.CreateMap<WorkItem, WorkItemIndexViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.IsComplete, opt => opt.MapFrom( src => src.Status.Category == WorkItemStatusCategory.Complete ) );

         Mapper.CreateMap<User, UserViewModel>()
            .ForMember( dest => dest.CallingAction, opt => opt.Ignore() )
            .ForMember( dest => dest.CallingId, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
      }


      #region Resolvers
      public class StatusCdToBooleanResolver : ValueResolver<char, bool>
      {
         protected override bool ResolveCore( char source )
         {
            return source == 'A';
         }
      }

      public class BooleanToStatusCdResolver : ValueResolver<bool, char>
      {
         protected override char ResolveCore( bool source )
         {
            return source ? 'A' : 'I';
         }
      }

      public class DomainModelResolver<ModelT> : ValueResolver<Guid, ModelT>
      {
         [Inject]
         public DomainModelResolver( ISessionFactory sessionFactory )
         {
            _sessionFactory = sessionFactory;
         }

         private readonly ISessionFactory _sessionFactory;

         protected override ModelT ResolveCore( Guid sourceId )
         {
            var session = _sessionFactory.GetCurrentSession();

            ModelT model = session.Get<ModelT>( sourceId );
            return model;
         }
      }
      #endregion
   }
}