using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class SystemObjectViewModel<T> : DataObjectBaseViewModel<T> where T : DataObjectBase, new() 
   {
      public SystemObjectViewModel()
         : base() { }

      public SystemObjectViewModel( T model )
         : base( model ) { }
   }
}