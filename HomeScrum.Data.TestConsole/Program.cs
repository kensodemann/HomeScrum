using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using Moq;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.TestConsole
{
   public class Program
   {
      private static Mock<ILogger> _logger;
      
      static void Main( string[] args )
      {
         _logger = new Mock<ILogger>();
      }

      static void PrintWorkItemType( WorkItemType workItemType )
      {
         Console.WriteLine( "\nWorkItemType\n" );
         Console.WriteLine( "\tId: " + workItemType.Id.ToString() );
         Console.WriteLine( "\tName: " + workItemType.Name );
         Console.WriteLine( "\tDescription: " + workItemType.Description );
         Console.WriteLine( "\tStatusCd: " + workItemType.StatusCd );
         Console.WriteLine( "\tCategory: " + workItemType.Category.ToString() );
         Console.WriteLine( "\tIsPredefined: " + workItemType.IsPredefined );
      }

      static void PrintWorkItemStatus( WorkItemStatus status )
      {
         Console.WriteLine( "\nWorkItemStatus\n" );
         Console.WriteLine( "\tId: " + status.Id.ToString() );
         Console.WriteLine( "\tName: " + status.Name );
         Console.WriteLine( "\tDescription: " + status.Description );
         Console.WriteLine( "\tStatusCd: " + status.StatusCd );
         Console.WriteLine( "\tCagtegory: " + status.Category );
         Console.WriteLine( "\tIsPredefined: " + status.IsPredefined );
      }

      static void PrintSprintStatus( SprintStatus status )
      {
         Console.WriteLine( "\nSprintStatus\n" );
         Console.WriteLine( "\tId: " + status.Id.ToString() );
         Console.WriteLine( "\tName: " + status.Name );
         Console.WriteLine( "\tDescription: " + status.Description );
         Console.WriteLine( "\tStatusCd: " + status.StatusCd );
         Console.WriteLine( "\tIsOpenStatus: " + status.IsOpenStatus );
         Console.WriteLine( "\tIsPredefined: " + status.IsPredefined );
      }

      static void PrintProjectStatus( ProjectStatus status )
      {
         Console.WriteLine( "\nProjectStatus\n" );
         Console.WriteLine( "\tId: " + status.Id.ToString() );
         Console.WriteLine( "\tName: " + status.Name );
         Console.WriteLine( "\tDescription: " + status.Description );
         Console.WriteLine( "\tStatusCd: " + status.StatusCd );
         Console.WriteLine( "\tIsActive: " + status.IsActive );
         Console.WriteLine( "\tIsPredefined: " + status.IsPredefined );
      }

      static void PrintAcceptanceCriteriaStatus( AcceptanceCriterionStatus status )
      {
         Console.WriteLine( "\nAcceptanceCriteriaStatus\n" );
         Console.WriteLine( "\tId: " + status.Id.ToString() );
         Console.WriteLine( "\tName: " + status.Name );
         Console.WriteLine( "\tDescription: " + status.Description );
         Console.WriteLine( "\tStatusCd: " + status.StatusCd );
         Console.WriteLine( "\tIsAccepted: " + status.IsAccepted );
         Console.WriteLine( "\tIsPredefined: " + status.IsPredefined );
      }

      static void PrintProject( Project project )
      {
         Console.WriteLine( "\nProject\n" );
         Console.WriteLine( "\tId: " + project.Id.ToString() );
         Console.WriteLine( "\tName: " + project.Name );
         Console.WriteLine( "\tDescription: " + project.Description );
      }
   }
}
