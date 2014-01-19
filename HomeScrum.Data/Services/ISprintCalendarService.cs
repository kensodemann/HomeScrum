using HomeScrum.Data.Domain;

namespace HomeScrum.Data.Services
{
   public interface ISprintCalendarService
   {
      void Update( Sprint sprint );
      void Reset( Sprint sprint );
   }
}
