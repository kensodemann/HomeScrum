using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class SystemDataObjectViewModel<T> : DataObjectBaseViewModel<T> where T : SystemDataObject, new() 
   {
      public SystemDataObjectViewModel() //TODO: Set IsPredefined to N for newly created stuff.
         : base() { }

      public SystemDataObjectViewModel( T model )
         : base( model ) { }

      public bool IsActive
      {
         get { return Model.StatusCd == 'A'; }
         set { Model.StatusCd = value ? 'A' : 'I'; }
      }
   }
}