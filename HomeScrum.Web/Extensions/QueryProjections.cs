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
      public static IList<DomainObjectViewModel> SelectDomainObjectViewModels<T>( this IQueryOver<T, T> query )
         where T : DomainObjectBase
      {
         DomainObjectViewModel viewModel = null;

         return query
            .Select( Projections.ProjectionList()
               .Add( Projections.Property<T>( x => x.Id ).WithAlias( () => viewModel.Id ) )
               .Add( Projections.Property<T>( x => x.Name ).WithAlias( () => viewModel.Name ) )
               .Add( Projections.Property<T>( x => x.Description ).WithAlias( () => viewModel.Description ) ) )
            .TransformUsing( Transformers.AliasToBean<DomainObjectViewModel>() )
            .List<DomainObjectViewModel>();
      }
   }
}