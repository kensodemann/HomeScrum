using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Data.UnitTest.Services
{
   [TestClass]
   public class SprintCalendarServiceTest
   {
      [TestMethod]
      public void Update_DoesNothingIfNoStartDate()
      {

      }

      [TestMethod]
      public void Update_DoesNothingIfNoEndDate()
      {

      }

      [TestMethod]
      public void Update_AddsDaysUpToToday_IfNotThere()
      {
      }

      [TestMethod]
      public void Update_UpdatesTodayIfEntryExists()
      {
      }

      [TestMethod]
      public void Reset_DoesNothingIfNoStartDate()
      {

      }

      [TestMethod]
      public void Reset_DoesNothingIfNoEndDate()
      {

      }

      [TestMethod]
      public void Reset_CreatesEntryForStartDate_IfCurrentDateBeforeStartDate()
      {

      }

      [TestMethod]
      public void Reset_RebuildsCalendarToDate_IfCurrentDateBetweenStartAndEndDate()
      {
      
      }

      [TestMethod]
      public void Reset_RebuildsFullCalendar_IfCurrentDatePastEndDate()
      {

      }
   }
}
