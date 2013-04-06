using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Base
{
   // DisplayViewModelBase and EditorViewModelBase are identicle, but they are the
   // roots of different tress.  This allows us to have two families of view models
   // for AutoMapper to map to.
   //
   // That is, we can do the following and have the mapper give us the correct type
   // of display view model based on the type for our model.
   //   var model = new ModelT();
   //   var viewModel = Mapper.Map( model, model.GetType(), typeof( DisplayViewModelBase ) ); 
   public class DisplayViewModel
   {
      public Guid Id { get; set; }
   }
}