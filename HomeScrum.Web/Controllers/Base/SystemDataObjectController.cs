using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public class SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> : ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : SystemDomainObject
      where ViewModelT : SystemDomainObjectViewModel
      where EditorViewModelT : SystemDomainObjectViewModel, new()
   {
      public SystemDataObjectController( IValidator<ModelT> validator, IPropertyNameTranslator<ModelT, EditorViewModelT> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( validator, translator, logger, sessionFactory ) { }


      //
      // POST: /ModelTs/UpdateSortOrders
      [HttpPost]
      public ActionResult UpdateSortOrders( IEnumerable<string> itemIds )
      {
         using (var session = SessionFactory.OpenSession())
         {
            using (var transaction = session.BeginTransaction())
            {
               var idIndex = 0;
               foreach (var id in itemIds)
               {
                  var item = session.Get<ModelT>( new Guid( id ) );
                  idIndex++;
                  if (item != null && item.SortSequence != idIndex)
                  {
                     item.SortSequence = idIndex;
                     session.Update( item );
                  }
               }
               transaction.Commit();
            }
         }

         return new EmptyResult();
      }

      //
      // GET: /ModelT/
      public override ActionResult Index()
      {
         Log.Debug( "Index()" );

         using (var session = SessionFactory.OpenSession())
         {
            SystemDomainObjectViewModel viewModel = null;
            var items = session
               .QueryOver<ModelT>()
               .Select( Projections.ProjectionList()
                  .Add( Projections.Property<ModelT>( x => x.Id ).WithAlias( () => viewModel.Id ) )
                  .Add( Projections.Property<ModelT>( x => x.Name ).WithAlias( () => viewModel.Name ) )
                  .Add( Projections.Property<ModelT>( x => x.Description ).WithAlias( () => viewModel.Description ) )
                  .Add( Projections.Property<ModelT>( x => x.StatusCd ).WithAlias( () => viewModel.StatusCd ) )
                  .Add( Projections.Property<ModelT>( x => x.IsPredefined ).WithAlias( () => viewModel.IsPredefined ) ) )
               .OrderBy( x => x.SortSequence ).Asc
               .TransformUsing( Transformers.AliasToBean<SystemDomainObjectViewModel>() )
               .List<SystemDomainObjectViewModel>();
            return View( items );
         }

      }
   }
}