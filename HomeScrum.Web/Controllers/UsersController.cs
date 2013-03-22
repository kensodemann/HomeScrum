using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.Controllers
{
   public class UsersController : Controller
   {
      [Inject]
      public UsersController( IRepository<User, String> userRepository, ISecurityRepository securityRepository, IValidator<User> validator )
      {
         _userRepository = userRepository;
         _securityRepository = securityRepository;
         _validator = validator;
      }

      private readonly IRepository<User, String> _userRepository;
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
      public virtual ActionResult Details( string id )
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
      public virtual ActionResult Create( UserEditorViewModel viewModel )
      {
         Validate( viewModel.User );

         if (ModelState.IsValid)
         {
            _userRepository.Add( viewModel.User );
            return RedirectToAction( "Index" );
         }

         return View();
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public virtual ActionResult Edit( string id )
      {
         var model = _userRepository.Get( id );
         if (model != null)
         {
            return View( new UserEditorViewModel( model ) );
         }

         return HttpNotFound();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( UserEditorViewModel viewModel )
      {
         Validate( viewModel.User );

         if (ModelState.IsValid)
         {
            _userRepository.Update( viewModel.User );

            return RedirectToAction( "Index" );
         }
         else
         {
            return View();
         }
      }


      private void Validate( User model )
      {
         if (!_validator.ModelIsValid( model ))
         {
            foreach (var message in _validator.Messages)
            {
               ModelState.AddModelError( message.Key, message.Value );
            }
         }
      }
   }
}
