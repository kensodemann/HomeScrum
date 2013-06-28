using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.SqlServer;
using HomeScrum.Data.Domain;
using Moq;
using HomeScrum.Services;

namespace HomeScrum.Data.TestConsole
{
   public class Program
   {
      private static Mock<ILogger> _logger;
      
      static void Main( string[] args )
      {
         _logger = new Mock<ILogger>();

         //PrintAllWorkItemTypes();
         //PrintAllWorkItemStatuses();
         //PrintAllSprintStatuses();
         //PrintAllProjectStatuses();
         //PrintAllAcceptanceCriteriaStatuses();

         PrintAllProjects();

         //var repository = new SecurityRepository();
         //Console.WriteLine( repository.IsValidLogin( "admin", "admin" ) ? "Yes" : "No" );
         //repository.ChangePassword( "admin", "admin", "something" );
         //Console.WriteLine( repository.IsValidLogin( "admin", "admin" ) ? "Yes" : "No" );
         //repository.ChangePassword( "admin", "something", "admin" );
         //Console.WriteLine( repository.IsValidLogin( "admin", "admin" ) ? "Yes" : "No" );
      }

      private static void PrintAllWorkItemTypes()
      {
         var repository = new SimpleSortedRepository<WorkItemType>(_logger.Object);
         var workItemTypes = repository.GetAll();

         Console.WriteLine( "Work Item Types: " + workItemTypes.Count().ToString() );

         foreach (var workItemType in workItemTypes)
         {
            PrintWorkItemType( workItemType );
         }
      }

      private static void PrintAllWorkItemStatuses()
      {
         var repository = new SimpleSortedRepository<WorkItemStatus>( _logger.Object );
         var statuses = repository.GetAll();

         Console.WriteLine( "Work Item Statuses: " + statuses.Count().ToString() );

         foreach (var status in statuses)
         {
            PrintWorkItemStatus( status );
         }
      }

      private static void PrintAllSprintStatuses()
      {
         var repository = new SimpleSortedRepository<SprintStatus>( _logger.Object );
         var statuses = repository.GetAll();

         Console.WriteLine( "Sprint Statuses: " + statuses.Count().ToString() );

         foreach (var status in statuses)
         {
            PrintSprintStatus( status );
         }
      }

      private static void PrintAllProjectStatuses()
      {
         var repository = new SimpleSortedRepository<ProjectStatus>( _logger.Object );
         var statuses = repository.GetAll();

         Console.WriteLine( "Project Statuses: " + statuses.Count().ToString() );

         foreach (var status in statuses)
         {
            PrintProjectStatus( status );
         }
      }

      private static void PrintAllAcceptanceCriteriaStatuses()
      {
         var repository = new SimpleSortedRepository<AcceptanceCriteriaStatus>( _logger.Object );
         var statuses = repository.GetAll();

         Console.WriteLine( "Acceptance Criteria Statuses: " + statuses.Count().ToString() );

         foreach (var status in statuses)
         {
            PrintAcceptanceCriteriaStatus( status );
         }
      }

      private static void PrintAllProjects()
      {
         //var repository = new Repository<Project, Guid>();
         var repository = new ProjectRepository( _logger.Object );
         var projects = repository.GetAll();

         Console.WriteLine( "Projects: " + projects.Count().ToString() );

         foreach (var project in projects)
         {
            PrintProject( project );
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

      static void PrintWorkItemStatus( WorkItemStatus status )
      {
         Console.WriteLine( "\nWorkItemStatus\n" );
         Console.WriteLine( "\tId: " + status.Id.ToString() );
         Console.WriteLine( "\tName: " + status.Name );
         Console.WriteLine( "\tDescription: " + status.Description );
         Console.WriteLine( "\tStatusCd: " + status.StatusCd );
         Console.WriteLine( "\tIsOpenStatus: " + status.IsOpenStatus );
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

      static void PrintAcceptanceCriteriaStatus( AcceptanceCriteriaStatus status )
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
