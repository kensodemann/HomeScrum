using HomeScrum.Common.Utility;
using HomeScrum.Spa.Models.Base;

namespace HomeScrum.Spa.Models
{
   public class WorkItemType : HomeScrumObject
   {
      public HomeScrum.Data.Domain.WorkItemTypeCategory Category { get; set; }
      public string CategoryName { get { return EnumHelper.GetDescription( this.Category ); } }
   }
}