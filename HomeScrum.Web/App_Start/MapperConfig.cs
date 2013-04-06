﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web
{
   public class MapperConfig
   {
      public static void RegisterMappings()
      {
         //
         // Domain to View Model mapping
         Mapper.CreateMap<DomainObjectBase, DomainObjectViewModel>()
            .Include<SystemDomainObject, SystemDomainObjectViewModel>();
         Mapper.CreateMap<SystemDomainObject, SystemDomainObjectViewModel>()
            .ForMember( dest => dest.AllowUse, opt => opt.ResolveUsing<AllowUseResolver>() );

         //
         // View Model to Domain mapping
         Mapper.CreateMap<DomainObjectViewModel, DomainObjectBase>()
            .Include<SystemDomainObjectViewModel, SystemDomainObject>(); ;
         Mapper.CreateMap<SystemDomainObjectViewModel, SystemDomainObject>()
            .ForMember( dest => dest.StatusCd, opt => opt.ResolveUsing<StatusCodeResolver>() );
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
      #endregion
   }
}