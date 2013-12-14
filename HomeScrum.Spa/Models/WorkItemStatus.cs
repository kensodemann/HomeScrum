using HomeScrum.Common.Utility;
using HomeScrum.Spa.Models.Base;

namespace HomeScrum.Spa.Models
{
   public class WorkItemStatus : HomeScrumObject
   {
      public HomeScrum.Data.Domain.WorkItemStatusCategory Category { get; set; }
      public string CategoryName { get { return EnumHelper.GetDescription( this.Category ); } }
   }
}