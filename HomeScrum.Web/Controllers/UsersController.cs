using AutoMapper;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
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
            _userRepository.Add( model );
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
