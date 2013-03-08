using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class SystemDataObjectViewModel<T> : DataObjectBaseViewModel<T> where T : DataObjectBase, new() 
   {
      public SystemDataObjectViewModel()
         : base() { }

      public SystemDataObjectViewModel( T model )
         : base( model ) { }
   }
}