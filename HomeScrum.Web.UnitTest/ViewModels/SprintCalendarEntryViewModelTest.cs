using System;
using HomeScrum.Web.Models.Sprints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class SprintCalendarEntryViewModelTest
   {
      [TestMethod]
      public void ToPlotPoint_FormatsDatePoints()
      {
         var date = new DateTime(2014,1,5,12,11,45);
         var entry = new SprintCalendarEntryViewModel()
         {
            HistoryDate = date,
            PointsRemaining = 42
         };

         Assert.AreEqual( "['2014-01-05', 42]", entry.ToPlotPoint() );
      }
   }
}
