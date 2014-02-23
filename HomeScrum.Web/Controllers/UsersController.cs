using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.Admin;
using NHibernate.Linq;
using Ninject;
using NHibernate;
using System.Collections.Generic;
using HomeScrum.Web.Models.Base;
using Ninject.Extensions.Logging;
using HomeScrum.Web.Controllers.Base;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class UsersController : HomeScrumControllerBase
   {
      [Inject]
      public UsersController( ILogger logger, ISecurityService securityService, ISessionFactory sessionFactory )
      {
         _logger = logger;
         _securityService = securityService;
         _sessionFactory = sessionFactory;
      }

      private readonly ISecurityService _securityService;
      private readonly ISessionFactory _sessionFactory;
      private readonly ILogger _logger;

      private ILogger Log { get { return _logger; } }

      //
      // GET: /Users/
      public ActionResult Index()
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var users = session.Query<User>()
               .Select( x => new UserViewModel()
                             {
                                Id = x.Id,
                                FirstName = x.FirstName,
                                MiddleName = x.MiddleName,
                                LastName = x.LastName,
                                IsActive = (x.StatusCd == 'A'),
                                UserName = x.UserName
                             } ).ToList();

            transaction.Commit();
            return View( users );
         }
      }


      //
      // GET: /Users/Create
      public ActionResult Create( string callingAction = null, string callingId = null )
      {
         var viewModel = new CreateUserViewModel()
         {
            Mode = EditMode.Create
         };

         UpdateNavigationStack( viewModel, null, callingAction, callingId );

         return View( viewModel );
      }

      //
      // POST: /Users/Create
      [HttpPost]
      public virtual ActionResult Create( CreateUserViewModel viewModel )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               viewModel.Mode = EditMode.Create;
               if (ModelState.IsValid)
               {
                  var model = Mapper.Map<User>( viewModel );
                  model.SetPassword( viewModel.NewPassword );

                  if (model.IsValidFor( Data.TransactionType.Insert ))
                  {
                     session.Save( model );
                     transaction.Commit();
                     return RedirectToAction( "Edit",
                        new
                        {
                           id = model.Id.ToString(),
                           callingController = viewModel.CallingController,
                           callingAction = viewModel.CallingAction,
                           callingId = viewModel.CallingId != Guid.Empty ? viewModel.CallingId.ToString() : null
                        } );
                  }
                  else
                  {
                     foreach (var message in model.GetErrorMessages())
                     {
                        ModelState.AddModelError( message.Key, message.Value );
                     }
                  }
               }
               transaction.Commit();
            }
            catch (Exception e)
            {
               Log.Error( e, "Create POST Error" );
               transaction.Rollback();
            }

            return View( viewModel );
         }
      }


      //
      // GET: /Users/Edit/Guid
      public ActionResult Details( Guid id, string callingAction = null, string callingId = null )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var model = session.Get<User>( id );
            if (model != null)
            {
               var viewModel = Mapper.Map<UserViewModel>( model );
               transaction.Commit();
               UpdateNavigationStack( viewModel, null, callingAction, callingId );
               return View( viewModel );
            }
         }

         return HttpNotFound();
      }


      //
      // GET: /Users/Edit/Guid
      public ActionResult Edit( Guid id, string callingAction = null, string callingId = null )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var model = session.Get<User>( id );
            if (model != null)
            {
               var viewModel = Mapper.Map<EditUserViewModel>( model );
               viewModel.Mode = EditMode.Edit;
               UpdateNavigationStack( viewModel, null, callingAction, callingId );
               transaction.Commit();
               return View( viewModel );
            }
         }

         return HttpNotFound();
      }

      //
      // POST: /Users/Edit/5
      [HttpPost]
      public ActionResult Edit( EditUserViewModel viewModel )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               viewModel.Mode = EditMode.Edit;
               if (ModelState.IsValid)
               {
                  var model = Mapper.Map<User>( viewModel );
                  if (model.IsValidFor( Data.TransactionType.Update ))
                  {
                     session.Update( model );
                     transaction.Commit();
                     return RedirectToAction( "Edit",
                        new
                        {
                           id = viewModel.Id.ToString(),
                           callingController = viewModel.CallingController,
                           callingAction = viewModel.CallingAction,
                           callingId = viewModel.CallingId != Guid.Empty ? viewModel.CallingId.ToString() : null
                        } );
                  }
                  else
                  {
                     foreach (var message in model.GetErrorMessages())
                     {
                        ModelState.AddModelError( message.Key, message.Value );
                     }
                  }
               }
               transaction.Commit();
            }
            catch (Exception e)
            {
               Log.Error( e, "Edit POST Error" );
               transaction.Rollback();
            }


            return View( viewModel );
         }
      }
   }
}
