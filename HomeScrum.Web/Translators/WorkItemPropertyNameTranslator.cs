using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class WorkItemEditorViewModelPropertyNameTranslator : PropertyNameTranslator<WorkItem, WorkItemEditorViewModel>
   {
      public WorkItemEditorViewModelPropertyNameTranslator() : base()
      {
         this.AddTranslation( "LastModifiedUserRid", "LastModifiedUserId" );
      }
   }
}