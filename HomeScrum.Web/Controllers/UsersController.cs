using System;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using HomeScrum.Common.Utility;
using System.Linq.Expressions;
using AutoMapper;
using System.Collections.Generic;

namespace HomeScrum.Web.Controllers
{
   public class UsersController : Controller
   {
      [Inject]
      public UsersController( IRepository<User> userRepository, ISecurityRepository securityRepository, IValidator<User> validator )
      {
         _userRepository = userRepository;
         _securityRepository = securityRepository;
         _validator = validator;
      }

      private readonly IRepository<User> _userRepository;
      private readonly ISecurityRepository _securityRepository;
      private readonly IValidator<User> _validator;

      //
      // GET: /Users/
      public ActionResult Index()
      {
         var items = _userRepository.GetAll();
         return View( Mapper.Map<ICollection<User>, IEnumerable<UserViewModel>>( items ) );
      }

      //
      // GET: /Users/Details/Guid
      public ActionResult Details( Guid id )
      {
         var model = _userRepository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( Mapper.Map<UserViewModel>( model ) );
      }

      //
      // GET: /Users/Create
      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /Users/Create
      [HttpPost]
      public virtual ActionResult Create( CreateUserViewModel viewModel )
      {
         var model = Mapper.Map<User>( viewModel );
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            _userRepository.Add( new User( model ) );
            _securityRepository.ChangePassword( viewModel.UserName, "bogus", viewModel.NewPassword );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /Users/Edit/Guid
      public ActionResult Edit( Guid id )
      {
         var model = _userRepository.Get( id );
         if (model != null)
         {
            return View( Mapper.Map<EditUserViewModel>( model ) );
         }

         return HttpNotFound();
      }

      //
      // POST: /Users/Edit/5
      [HttpPost]
      public ActionResult Edit( EditUserViewModel viewModel )
      {
         var model = Mapper.Map<User>( viewModel );
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
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

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}
