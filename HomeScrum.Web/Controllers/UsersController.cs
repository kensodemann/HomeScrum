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

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class UsersController : Controller
   {
      [Inject]
      public UsersController( ISecurityRepository securityRepository )
      {
         _securityRepository = securityRepository;
      }

      private readonly ISecurityRepository _securityRepository;

      //
      // GET: /Users/
      public ActionResult Index()
      {
         using (var session = NHibernateHelper.OpenSession())
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

            return View( users );
         }
      }

      //
      // GET: /Users/Details/Guid
      public ActionResult Details( Guid id )
      {
         using (var session = NHibernateHelper.OpenSession())
         {
            var model = session.Get<User>( id );

            if (model == null)
            {
               return HttpNotFound();
            }
            return View( Mapper.Map<UserViewModel>( model ) );
         }
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
         if (ModelState.IsValid)
         {
            var model = Mapper.Map<User>( viewModel );
            try
            {
               using (var session = NHibernateHelper.OpenSession())
               {
                  using (var transaction = session.BeginTransaction())
                  {
                     session.Save( model );
                     transaction.Commit();
                  }
               }

               _securityRepository.ChangePassword( viewModel.UserName, "bogus", viewModel.NewPassword );
               return RedirectToAction( () => this.Index() );
            }
            catch (InvalidOperationException)
            {
               foreach (var message in model.GetErrorMessages())
               {
                  ModelState.AddModelError( message.Key, message.Value );
               }
            }
         }

         return View();
      }

      //
      // GET: /Users/Edit/Guid
      public ActionResult Edit( Guid id )
      {
         using (var session = NHibernateHelper.OpenSession())
         {
            var model = session.Get<User>( id );
            if (model != null)
            {
               return View( Mapper.Map<EditUserViewModel>( model ) );
            }
         }

         return HttpNotFound();
      }

      //
      // POST: /Users/Edit/5
      [HttpPost]
      public ActionResult Edit( EditUserViewModel viewModel )
      {
         if (ModelState.IsValid)
         {
            var model = Mapper.Map<User>( viewModel );
            try
            {
               using (var session = NHibernateHelper.OpenSession())
               {
                  using (var transaction = session.BeginTransaction())
                  {
                     session.Update( model );
                     transaction.Commit();
                  }
               }
               return RedirectToAction( () => this.Index() );
            }
            catch (InvalidOperationException)
            {
               foreach (var message in model.GetErrorMessages())
               {
                  ModelState.AddModelError( message.Key, message.Value );
               }
            }
         }

         return View( viewModel );
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}
