using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.SqlServer;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.TestConsole
{
   public class Program
   {
      static void PrintWorkItemType( WorkItemType workItemType )
      {
         Console.WriteLine( "\nWorkItemType\n" );
         Console.WriteLine( "\tId: " + workItemType.Id.ToString() );
         Console.WriteLine( "\tName: " + workItemType.Name );
         Console.WriteLine( "\tDescription: " + workItemType.Description );
         Console.WriteLine( "\tStatusCd: " + workItemType.StatusCd );
         Console.WriteLine( "\tIsTask: " + workItemType.IsTask );
         Console.WriteLine( "\tIsPredefined: " + workItemType.IsPredefined );
      }


      static void Main( string[] args )
      {
         var repository = new DataObjectRepository<WorkItemType>();
         var workItemTypes = repository.GetAll();

         Console.WriteLine( "Work Item Types: " + workItemTypes.Count().ToString() );

         foreach (var workItemType in workItemTypes)
         {
            PrintWorkItemType( workItemType );
         }
      }
   }
}
