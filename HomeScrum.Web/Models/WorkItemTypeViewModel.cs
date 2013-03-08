using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class WorkItemTypeViewModel : SystemObjectViewModel<WorkItemType>
   {
      public WorkItemTypeViewModel()
         : base() { }
      public WorkItemTypeViewModel( WorkItemType model )
         : base( model ) { }

      public bool IsActive
      {
         get { return Model.StatusCd == 'A'; }
         set { Model.StatusCd = value ? 'A' : 'I'; }
      }

      public bool IsTask
      {
         get { return Model.IsTask == 'Y'; }
         set { Model.IsTask = value ? 'Y' : 'N'; }
      }
   }
}