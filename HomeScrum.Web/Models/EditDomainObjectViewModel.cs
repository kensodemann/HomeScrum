using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class EditDomainObjectViewModel<T> : IViewModel<T>
   {
      public T DomainModel { get; set; }
   }
}