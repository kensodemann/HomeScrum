using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Extensions
{
   public static class QueryProjections
   {
      public static IList<DomainObjectViewModel> SelectDomainObjectViewModels<SourceT>( this IQueryOver<SourceT, SourceT> query )
         where SourceT : DomainObjectBase
      {
         DomainObjectViewModel viewModel = null;

         return query
            .Select( Projections.ProjectionList()
               .Add( Projections.Property<SourceT>( x => x.Id ).WithAlias( () => viewModel.Id ) )
               .Add( Projections.Property<SourceT>( x => x.Name ).WithAlias( () => viewModel.Name ) )
               .Add( Projections.Property<SourceT>( x => x.Description ).WithAlias( () => viewModel.Description ) ) )
            .TransformUsing( Transformers.AliasToBean<DomainObjectViewModel>() )
            .List<DomainObjectViewModel>();
      }

      public static IList<SystemDomainObjectViewModel> SelectSystemDomainObjectViewModels<SourceT>( this IQueryOver<SourceT, SourceT> query )
         where SourceT : SystemDomainObject
      {
         SystemDomainObjectViewModel viewModel = null;

         return query
            .Select( Projections.ProjectionList()
               .Add( Projections.Property<SourceT>( x => x.Id ).WithAlias( () => viewModel.Id ) )
               .Add( Projections.Property<SourceT>( x => x.Name ).WithAlias( () => viewModel.Name ) )
               .Add( Projections.Property<SourceT>( x => x.Description ).WithAlias( () => viewModel.Description ) )
               .Add( Projections.Conditional( Restrictions.Eq( Projections.Property<SourceT>( x => x.StatusCd ), 'A' ), Projections.Constant( true ), Projections.Constant( false ) ).WithAlias( () => viewModel.AllowUse ) )
               .Add( Projections.Property<SourceT>( x => x.IsPredefined ).WithAlias( () => viewModel.IsPredefined ) ) )
            .TransformUsing( Transformers.AliasToBean<SystemDomainObjectViewModel>() )
            .List<SystemDomainObjectViewModel>();
      }
   }
}