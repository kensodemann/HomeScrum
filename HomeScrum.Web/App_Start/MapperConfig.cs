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
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<ProjectStatusEditorViewModel, ProjectStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<SprintStatusEditorViewModel, SprintStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<WorkItemStatusEditorViewModel, WorkItemStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );
         Mapper.CreateMap<WorkItemTypeEditorViewModel, WorkItemType>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.AllowUse ) );

         Mapper.CreateMap<ProjectEditorViewModel, Project>()
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.MapFrom( src => src.LastModifiedUserId ) )
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<DomainModelResolver<ProjectStatus>>().FromMember( src => src.StatusId ) )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<WorkItemEditorViewModel, WorkItem>()
            .ForMember( dest => dest.Status, opt => opt.ResolveUsing<DomainModelResolver<WorkItemStatus>>().FromMember( src => src.StatusId ) )
            .ForMember( dest => dest.WorkItemType, opt => opt.ResolveUsing<DomainModelResolver<WorkItemType>>().FromMember( src => src.WorkItemTypeId ) )
            .ForMember( dest => dest.Project, opt => opt.ResolveUsing<DomainModelResolver<Project>>().FromMember( src => src.ProjectId ) )
            .ForMember( dest => dest.ParentWorkItem, opt => opt.ResolveUsing<DomainModelResolver<WorkItem>>().FromMember( src => src.ParentWorkItemId ) )
            .ForMember( dest => dest.LastModifiedUserRid, opt => opt.Ignore() )
            .ForMember( dest => dest.CreatedByUser, opt => opt.ResolveUsing<DomainModelResolver<User>>().FromMember( src => src.CreatedByUserId ) )
            .ForMember( dest => dest.AssignedToUser, opt => opt.ResolveUsing<DomainModelResolver<User>>().FromMember( src => src.AssignedToUserId ) )
            .ForMember( dest => dest.AcceptanceCriteria, opt => opt.Ignore() )
            .ForMember( dest => dest.Tasks, opt => opt.Ignore() )
            .ConstructUsingServiceLocator();

         Mapper.CreateMap<CreateUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.IsActive ) );
         Mapper.CreateMap<EditUserViewModel, User>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<BooleanToStatusCdResolver>().FromMember( src => src.IsActive ) );
      }


      private static void MapDomainsToEditorViewModels()
      {
         Mapper.CreateMap<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<ProjectStatus, ProjectStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<SprintStatus, SprintStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemType, WorkItemTypeEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );

         Mapper.CreateMap<Project, ProjectEditorViewModel>()
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.LastModifiedUserId, opt => opt.MapFrom( src => src.LastModifiedUserRid ) );

         Mapper.CreateMap<WorkItem, WorkItemEditorViewModel>()
            .ForMember( dest => dest.Statuses, opt => opt.Ignore() )
            .ForMember( dest => dest.WorkItemTypes, opt => opt.Ignore() )
            .ForMember( dest => dest.Projects, opt => opt.Ignore() )
            .ForMember( dest => dest.AssignedToUsers, opt => opt.Ignore() )
            .ForMember( dest => dest.ProductBacklogItems, opt => opt.Ignore() );

         Mapper.CreateMap<User, CreateUserViewModel>()
             .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
             .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
             .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<User, EditUserViewModel>()
            .ForMember( dest => dest.NewPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.ConfirmPassword, opt => opt.Ignore() )
            .ForMember( dest => dest.IsActive, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
      }


      private static void MapDomainsToViewModels()
      {
         Mapper.CreateMap<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<ProjectStatus, ProjectStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<SprintStatus, SprintStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemStatus, WorkItemStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );
         Mapper.CreateMap<WorkItemType, WorkItemTypeViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<StatusCdToBooleanResolver>().FromMember( src => src.StatusCd ) );

         Mapper.CreateMap<AcceptanceCriterion, AcceptanceCriterionViewModel>()
            .ForMember( dest => dest.IsAccepted, opt => opt.MapFrom( src => src.Status.IsAccepted ) );
         Mapper.CreateMap<Project, ProjectViewModel>();
         Mapper.CreateMap<WorkItem, WorkItemViewModel>()
            .ForMember( dest => dest.IsComplete, opt => opt.MapFrom( src => !(src.Status.IsOpenStatus) ) )
            .ForMember( dest => dest.AssignedToUserName, opt => opt.MapFrom( src => src.AssignedToUser.UserName ) )
            .ForMember( dest => dest.CreatedByUserName, opt => opt.MapFrom( src => src.CreatedByUser.UserName ) );
         Mapper.CreateMap<WorkItem, WorkItemIndexViewModel>()
            .ForMember( dest => dest.IsComplete, opt => opt.MapFrom( src => !(src.Status.IsOpenStatus) ) );

         Mapper.CreateMap<User, UserViewModel>()
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
            using (var session = _sessionFactory.OpenSession())
            {
               ModelT model = session.Get<ModelT>( sourceId );
               return model;
            }
         }
      }
      #endregion
   }
}