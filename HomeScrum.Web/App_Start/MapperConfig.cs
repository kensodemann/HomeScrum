using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web
{
   public class MapperConfig
   {
      public static void RegisterMappings()
      {
         MapDomainObjectsToViewModels();
         MapDomainObjectsToEditorViewModels();

         MapViewModelsToDomainObjects();
         MapEditorViewModelsToDomainObjects();
      }


      private static void MapEditorViewModelsToDomainObjects()
      {
         Mapper.CreateMap<DomainObjectEditorViewModel, DomainObjectBase>()
            .Include<SystemDomainObjectEditorViewModel, SystemDomainObject>();
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
      }

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

      private static void MapDomainObjectsToEditorViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectEditorViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectEditorViewModel>();
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
      }

      private static void MapDomainObjectsToViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectViewModel>();
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

      public class EditorStatusCodeResolver : ValueResolver<SystemDomainObjectEditorViewModel, char>
      {
         protected override char ResolveCore( SystemDomainObjectEditorViewModel source )
         {
            return source.AllowUse ? 'A' : 'I';
         }
      }
      #endregion
   }
}