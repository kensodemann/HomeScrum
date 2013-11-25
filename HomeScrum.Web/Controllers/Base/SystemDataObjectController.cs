using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> : ReadWriteController<ModelT, EditorViewModelT>
      where ModelT : SystemDomainObject
      where ViewModelT : SystemDomainObjectViewModel
      where EditorViewModelT : SystemDomainObjectViewModel, IEditorViewModel, new()
   {
      public SystemDataObjectController( IPropertyNameTranslator<ModelT, EditorViewModelT> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }


      //
      // POST: /ModelTs/UpdateSortOrders
      [HttpPost]
      public ActionResult UpdateSortOrders( IEnumerable<string> itemIds )
      {
         var session = SessionFactory.GetCurrentSession();
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

         return new EmptyResult();
      }

      protected abstract IQueryable<ViewModelT> SelectViewModels( IQueryable<ModelT> query );

      //
      // GET: /ModelT/
      public override ActionResult Index()
      {
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         var query = SelectViewModels( session.Query<ModelT>().OrderBy( x => x.SortSequence ) );

         return IndexView( query );
      }
   }
}