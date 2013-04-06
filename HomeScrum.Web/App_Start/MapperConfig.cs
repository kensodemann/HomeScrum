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
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<EditorStatusCodeResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatusEditorViewModel, AcceptanceCriteriaStatus>();
      }

      private static void MapViewModelsToDomainObjects()
      {
         Mapper.CreateMap<DomainObjectViewModel, DomainObjectBase>()
            .Include<SystemDomainObjectViewModel, SystemDomainObject>();
         Mapper.CreateMap<SystemDomainObjectViewModel, SystemDomainObject>()
            .Include<AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatus>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatus>();
      }

      private static void MapDomainObjectsToEditorViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectEditorViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectEditorViewModel>();
         Mapper.CreateMap<SystemDomainObject, SystemDomainObjectEditorViewModel>()
            .Include<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>();
      }

      private static void MapDomainObjectsToViewModels()
      {
         Mapper.CreateMap<DomainObjectBase, DomainObjectViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectViewModel>();
         Mapper.CreateMap<SystemDomainObject, SystemDomainObjectViewModel>()
            .Include<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         Mapper.CreateMap<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel>();
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