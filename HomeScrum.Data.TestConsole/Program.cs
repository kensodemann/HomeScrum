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
      static void Main( string[] args )
      {
         PrintAllWorkItemTypes();
         PrintAllWorkItemStatuses();
         PrintAllSprintStatuses();
         PrintAllProjectStatuses();
      }

      private static void PrintAllWorkItemTypes()
      {
         var repository = new DataObjectRepository<WorkItemType>();
         var workItemTypes = repository.GetAll();

         Console.WriteLine( "Work Item Types: " + workItemTypes.Count().ToString() );

         foreach (var workItemType in workItemTypes)
         {
            PrintWorkItemType( workItemType );
         }
      }

      private static void PrintAllWorkItemStatuses()
      {
         var repository = new DataObjectRepository<WorkItemStatus>();
         var workItemStatuses = repository.GetAll();

         Console.WriteLine( "Work Item Statuses: " + workItemStatuses.Count().ToString() );

         foreach (var workItemStatus in workItemStatuses)
         {
            PrintWorkItemStatus( workItemStatus );
         }
      }

      private static void PrintAllSprintStatuses()
      {
         var repository = new DataObjectRepository<SprintStatus>();
         var sprintStatuses = repository.GetAll();

         Console.WriteLine( "Sprint Statuses: " + sprintStatuses.Count().ToString() );

         foreach (var sprintStatus in sprintStatuses)
         {
            PrintSprintStatus( sprintStatus );
         }
      }

      private static void PrintAllProjectStatuses()
      {
         var repository = new DataObjectRepository<ProjectStatus>();
         var projectStatuses = repository.GetAll();

         Console.WriteLine( "Project Statuses: " + projectStatuses.Count().ToString() );

         foreach (var projectStatus in projectStatuses)
         {
            PrintProjectStatus( projectStatus );
         }
      }

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

      static void PrintWorkItemStatus( WorkItemStatus workItemStatus )
      {
         Console.WriteLine( "\nWorkItemStatus\n" );
         Console.WriteLine( "\tId: " + workItemStatus.Id.ToString() );
         Console.WriteLine( "\tName: " + workItemStatus.Name );
         Console.WriteLine( "\tDescription: " + workItemStatus.Description );
         Console.WriteLine( "\tStatusCd: " + workItemStatus.StatusCd );
         Console.WriteLine( "\tIsOpenStatus: " + workItemStatus.IsOpenStatus );
         Console.WriteLine( "\tIsPredefined: " + workItemStatus.IsPredefined );
      }

      static void PrintSprintStatus( SprintStatus sprintStatus )
      {
         Console.WriteLine( "\nSprintStatus\n" );
         Console.WriteLine( "\tId: " + sprintStatus.Id.ToString() );
         Console.WriteLine( "\tName: " + sprintStatus.Name );
         Console.WriteLine( "\tDescription: " + sprintStatus.Description );
         Console.WriteLine( "\tStatusCd: " + sprintStatus.StatusCd );
         Console.WriteLine( "\tIsOpenStatus: " + sprintStatus.IsOpenStatus );
         Console.WriteLine( "\tIsPredefined: " + sprintStatus.IsPredefined );
      }

      static void PrintProjectStatus( ProjectStatus projectStatus )
      {
         Console.WriteLine( "\nProjectStatus\n" );
         Console.WriteLine( "\tId: " + projectStatus.Id.ToString() );
         Console.WriteLine( "\tName: " + projectStatus.Name );
         Console.WriteLine( "\tDescription: " + projectStatus.Description );
         Console.WriteLine( "\tStatusCd: " + projectStatus.StatusCd );
         Console.WriteLine( "\tIsActive: " + projectStatus.IsActive );
         Console.WriteLine( "\tIsPredefined: " + projectStatus.IsPredefined );
      }
   }
}
