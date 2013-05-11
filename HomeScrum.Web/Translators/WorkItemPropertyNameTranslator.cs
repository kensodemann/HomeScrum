using HomeScrum.Web.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class WorkItemEditorViewModelPropertyNameTranslator : PropertyNameTranslator<WorkItemEditorViewModel>
   {
      public WorkItemEditorViewModelPropertyNameTranslator() : base()
      {
         this.AddTranslation( "CreatedByUser", "CreatedByUserId" );
         this.AddTranslation( "LastModifiedUserRid", "LastModifiedUserId" );
      }
   }
}