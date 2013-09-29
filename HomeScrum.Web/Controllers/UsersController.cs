﻿using System;
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

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class UsersController : Controller
   {
      [Inject]
      public UsersController( ISecurityService securityService, ISessionFactory sessionFactory )
      {
         _securityService = securityService;
         _sessionFactory = sessionFactory;
      }

      private readonly ISecurityService _securityService;
      private readonly ISessionFactory _sessionFactory;

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
         var viewModel = new CreateUserViewModel();

         Guid parsedId;
         Guid.TryParse( callingId, out parsedId );
         viewModel.CallingId = parsedId;
         viewModel.CallingAction = callingAction;

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
            if (ModelState.IsValid)
            {
               var model = Mapper.Map<User>( viewModel );
               model.SetPassword( viewModel.NewPassword );

               if (model.IsValidFor( Data.TransactionType.Insert ))
               {
                  session.Save( model );
                  transaction.Commit();
                  return RedirectToAction( () => this.Index() );
               }

               foreach (var message in model.GetErrorMessages())
               {
                  ModelState.AddModelError( message.Key, message.Value );
               }
            }

            return View();
         }
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
               Guid parsedCallingId;
               Guid.TryParse( callingId, out parsedCallingId );
               viewModel.CallingId = parsedCallingId;
               viewModel.CallingAction = callingAction;
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
            if (ModelState.IsValid)
            {
               var model = Mapper.Map<User>( viewModel );
               if (model.IsValidFor( Data.TransactionType.Update ))
               {
                  session.Update( model );
                  transaction.Commit();
                  return RedirectToAction( () => this.Index() );
               }

               foreach (var message in model.GetErrorMessages())
               {
                  ModelState.AddModelError( message.Key, message.Value );
               }
            }

            return View( viewModel );
         }
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}
