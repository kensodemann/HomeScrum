using System;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class UsersController : HomeScrumController<User>
   {
      [Inject]
      public UsersController( IRepository<User, Guid> userRepository, ISecurityRepository securityRepository, IValidator<User> validator )
         :base(userRepository)
      {
         _securityRepository = securityRepository;
         _validator = validator;
      }

      private readonly ISecurityRepository _securityRepository;
      private readonly IValidator<User> _validator;

      //
      // GET: /Users/
      public virtual ActionResult Index()
      {
         return View( Repository.GetAll() );
      }

      //
      // GET: /Users/Details/5
      public virtual ActionResult Details( Guid id )
      {
         var model = Repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /Users/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /Users/Create
      [HttpPost]
      public virtual ActionResult Create( CreateUserViewModel viewModel )
      {
         Validate( viewModel, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            Repository.Add( new User( viewModel ) );
            _securityRepository.ChangePassword( viewModel.UserName, "bogus", viewModel.NewPassword );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /Users/Edit/5
      public virtual ActionResult Edit( Guid id )
      {
         var model = Repository.Get( id );
         if (model != null)
         {
            return View( new EditUserViewModel( model ) );
         }

         return HttpNotFound();
      }

      //
      // POST: /Users/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( EditUserViewModel viewModel )
      {
         Validate( viewModel, TransactionType.Update );

         if (ModelState.IsValid)
         {
            User model = new User( viewModel );
            Repository.Update( model );

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
