using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class WorkItemPropertyNameTranslator : PropertyNameTranslator<WorkItem, WorkItemEditorViewModel>
   {
      public WorkItemPropertyNameTranslator() : base()
      {
         this.AddTranslation( "LastModifiedUserRid", "LastModifiedUserId" );
      }
   }
}