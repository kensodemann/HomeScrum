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
         model = new T();
      }

      private T model;
      public T Model { get { return model; } }
   }
}