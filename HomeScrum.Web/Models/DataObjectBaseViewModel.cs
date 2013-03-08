using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class DataObjectBaseViewModel<T> where T : DataObjectBase, new()
   {
      public DataObjectBaseViewModel()
      {
         _model = new T();
      }

      public DataObjectBaseViewModel( T model )
      {
         _model = model;
      }

      private T _model;
      public T Model { get { return _model; } }
   }
}