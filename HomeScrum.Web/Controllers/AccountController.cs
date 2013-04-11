using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using HomeScrum.Web.Models;
using HomeScrum.Web.Providers;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class AccountController : Controller
   {
      private readonly IWebSecurity WebSecurity;

      public AccountController( IWebSecurity webSecurity )
         : base()
      {
         WebSecurity = webSecurity;
      }

      //
      // GET: /Account/Login
      [AllowAnonymous]
      public ActionResult Login( string returnUrl )
      {
         ViewBag.ReturnUrl = returnUrl;
         return View();
      }


      //
      // POST: /Account/Login
      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public ActionResult Login( LoginModel model, string returnUrl )
      {
         if (ModelState.IsValid && WebSecurity.Login( model.UserName, model.Password, persistCookie: model.RememberMe ))
         {
            return RedirectToLocal( returnUrl );
         }

         // If we got this far, something failed, redisplay form
         ModelState.AddModelError( "", "The user name or password provided is incorrect." );
         return View( model );
      }


      //
      // POST: /Account/LogOff
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LogOff()
      {
         WebSecurity.Logout();

         return RedirectToAction( "Index", "Home" );
      }


      //
      // GET: /Account/Manage
      public ActionResult Manage( ManageMessageId? message )
      {
         ViewBag.StatusMessage =
             message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
             : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
             : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
             : "";
         ViewBag.ReturnUrl = Url.Action( "Manage" );
         return View();
      }


      //
      // POST: /Account/Manage
      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Manage( LocalPasswordModel model )
      {
         ViewBag.ReturnUrl = Url.Action( "Manage" );

         if (ModelState.IsValid)
         {
            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
            bool changePasswordSucceeded;
            try
            {
               changePasswordSucceeded = WebSecurity.ChangePassword( WebSecurity.CurrentUser.Identity.Name, model.OldPassword, model.NewPassword );
            }
            catch (Exception)
            {
               changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
               return RedirectToAction( "Manage", new { Message = ManageMessageId.ChangePasswordSuccess } );
            }
            else
            {
               ModelState.AddModelError( "", "The current password is incorrect or the new password is invalid." );
            }
         }

         // If we got this far, something failed, redisplay form
         return View( model );
      }


      #region Helpers
      private ActionResult RedirectToLocal( string returnUrl )
      {
         if (Url.IsLocalUrl( returnUrl ))
         {
            return Redirect( returnUrl );
         }
         else
         {
            return RedirectToAction( "Index", "Home" );
         }
      }

      public enum ManageMessageId
      {
         ChangePasswordSuccess,
         SetPasswordSuccess,
         RemoveLoginSuccess,
      }
      #endregion
   }
}
