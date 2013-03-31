﻿using System;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class UsersController : HomeScrumController
   {
      [Inject]
      public UsersController( IRepository<User, Guid> userRepository, ISecurityRepository securityRepository, IValidator<User> validator )
      {
         _userRepository = userRepository;
         _securityRepository = securityRepository;
         _validator = validator;
      }

      private readonly IRepository<User, Guid> _userRepository;
      private readonly ISecurityRepository _securityRepository;
      private readonly IValidator<User> _validator;

      //
      // GET: /AcceptanceCriteriaStatuses/
      public virtual ActionResult Index()
      {
         return View( _userRepository.GetAll() );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Details/5
      public virtual ActionResult Details( Guid id )
      {
         var model = _userRepository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Create
      [HttpPost]
      public virtual ActionResult Create( CreateUserViewModel viewModel )
      {
         Validate( viewModel, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            _userRepository.Add( new User( viewModel ) );
            _securityRepository.ChangePassword( viewModel.UserName, "bogus", viewModel.NewPassword );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public virtual ActionResult Edit( Guid id )
      {
         var model = _userRepository.Get( id );
         if (model != null)
         {
            return View( new EditUserViewModel( model ) );
         }

         return HttpNotFound();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( EditUserViewModel viewModel )
      {
         Validate( viewModel, TransactionType.Update );

         if (ModelState.IsValid)
         {
            User model = new User( viewModel );
            _userRepository.Update( model );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            return View( viewModel );
         }
      }


      private void Validate( User model, TransactionType transactionType )
      {
         if (!_validator.ModelIsValid( model, transactionType ))
         {
            foreach (var message in _validator.Messages)
            {
               ModelState.AddModelError( message.Key, message.Value );
            }
         }
      }
   }
}
