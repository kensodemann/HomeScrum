using System;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintCalendarEntryViewModel
   {
      public DateTime HistoryDate { get; set; }
      public int PointsRemaining { get; set; }

      public string ToPlotPoint()
      {
         return String.Format( "['{0}', {1}]", this.HistoryDate.ToString( "yyyy-MM-dd" ), this.PointsRemaining );
      }
   }
}