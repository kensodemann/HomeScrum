using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web.Controllers.Base
{
   public class HomeScrumControllerBase : Controller
   {
      /// <summary>
      /// This method is intended to be called from the GET actions.  All NavigationStack maintenence is done from the GET
      /// because the user will not go through a POST, which is where a pop() would logically ding, in the case where they
      /// nav back or where they use the Cancel link.
      /// </summary>
      /// <param name="viewModel">Receives the new top calling information</param>
      /// <param name="callingController">Calling controller, null in case of cancel</param>
      /// <param name="callingAction">Calling action, null in case of cancel</param>
      /// <param name="callingId">Calling Id, null in case of cancel</param>
      protected void UpdateNavigationStack( ViewModelBase viewModel, string callingController, string callingAction, string callingId )
      {
         if (callingController != null || callingAction != null)
         {
            PushNavigationData( callingController, callingAction, callingId );
         }
         else
         {
            PopNavigationData();
         }
         PeekNavigationData( viewModel );
      }


      protected void ClearNavigationStack()
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            stack.Clear();
            Session["NavigationStack"] = stack;
         }
      }


      private void PushNavigationData( string callingController, string callingAction, string callingId )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack == null)
         {
            stack = new Stack<NavigationData>();
         }

         if (stack.Count != 0)
         {
            var top = stack.Peek();
            if (top.Controller == callingController && top.Action == callingAction && top.Id == callingId)
            {
               return;
            }
         }

         stack.Push( new NavigationData() { Controller = callingController, Action = callingAction, Id = callingId } );
         Session["NavigationStack"] = stack;
      }


      private void PopNavigationData()
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            stack.Pop();
            Session["NavigationStack"] = stack;
         }
      }


      protected void PeekNavigationData( ViewModelBase viewModel )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            var data = stack.Peek();
            Guid parsedId;
            Guid.TryParse( data.Id, out parsedId );
            viewModel.CallingController = data.Controller;
            viewModel.CallingAction = data.Action;
            viewModel.CallingId = parsedId;
         }
         else
         {
            viewModel.CallingController = null;
            viewModel.CallingAction = null;
            viewModel.CallingId = Guid.Empty;
         }
      }


      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}