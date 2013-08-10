using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItems
   {
     public static void Load(ISessionFactory sessionFactory)
      {
         CreateTestModelData();

         var session = sessionFactory.GetCurrentSession();
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItem in ModelData)
               session.Save( workItem );
            transaction.Commit();
         }
         session.Clear();
      }

      private static List<WorkItem> _workItems;
      public static WorkItem[] ModelData { get { return _workItems.ToArray(); } }

      private static List<AcceptanceCriterion> _criteria;

      private static Project homeScrum;
      private static Project sandwiches;
      private static Project preps;
      private static Project mathWar;

      private static WorkItemStatus newWorkItem;
      private static WorkItemStatus planning;
      private static WorkItemStatus assigned;
      private static WorkItemStatus complete;
      private static WorkItemStatus cancelled;

      private static WorkItemType pbi;
      private static WorkItemType customerRequest;
      private static WorkItemType bug;
      private static WorkItemType issue;
      private static WorkItemType sbi;

      private static AcceptanceCriterionStatus unverified;
      private static AcceptanceCriterionStatus accepted;
      private static AcceptanceCriterionStatus rejected;

      private static void CreateTestModelData( bool initializeIds = false )
      {
         InitializeProjects();
         InitializeWorkItemStatuses();
         InitializeWorkItemTypes();
         InitializeAcceptanceCriteriaStatuses();

         BuildBaseWorkItems();

         if (initializeIds)
         {
            InitializeIds();
            var childWorkItem = ModelData.First( x => x.WorkItemType.IsTask && x.WorkItemType.StatusCd == 'A' && x.Status.IsOpenStatus && x.Status.StatusCd == 'A' );
            var parentWorkItem = ModelData.First( x => !x.WorkItemType.IsTask && x.WorkItemType.StatusCd == 'A' && x.Status.IsOpenStatus && x.Status.StatusCd == 'A' );
            childWorkItem.ParentWorkItem = parentWorkItem;
         }
      }

      private static void BuildBaseWorkItems()
      {
         _workItems = new List<WorkItem>();

         // Backlog Items
         // 4 PBI's and 2 CR's
         //  * All with tasks
         //  * Most tasks with at lest one acceptance criterion
         //  * All tasks having statuses that are appropriate given the status of the backlog item
         //  * The following statuses for the backlog items:
         //    ** Planing (1)
         //    ** Assigned (3)
         //    ** Complete
         //    ** Cancelled
         //
         var workItem = CreateWorkItem( "Add Unit Tests", "We have been bad programmers and have not been using TDD.  Get what we have tested.", pbi, planning, homeScrum );
         var childWorkItem = CreateChildTask( workItem, "Examine Content", "Create catelog of our currently untested code.", sbi, newWorkItem );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Coverage", "All testable code is covered", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Write Tests", "Write tests to cover the code.", sbi, newWorkItem );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Coverage", "All testable code is covered", unverified );
         AddAcceptanceCriteria( childWorkItem, "Passing", "All tests pass", unverified );
         CloseCriteriaList( childWorkItem );
         
         workItem = CreateWorkItem( "Sprint Retrospective", "As a manager, I want to gather information on what went well with a sprint and what did not in order to improve the process",
            pbi, assigned, homeScrum );
         childWorkItem = CreateChildTask( workItem, "No Retrospetive", "We need to design a retrospective", issue, complete );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Before Close", "Must have before close of sprint", accepted );
         AddAcceptanceCriteria( childWorkItem, "Right", "Design allows for entry of what went right", accepted );
         AddAcceptanceCriteria( childWorkItem, "Wrong", "Design allows for entry of what went wrong", accepted );
         AddAcceptanceCriteria( childWorkItem, "Improve", "Design allows for entry of what we will improve upon the next sprint", accepted );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Add Right", "Add list of things we did right", sbi, assigned );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", accepted );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Add Wrong", "Add list of things we did wrong", sbi, assigned );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Add Improvment List", "Add a list of items we need to improve upon", sbi, assigned );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );

         workItem = CreateWorkItem( "Problem Report Printing", "As a user, I want to be able to print hard copies of problem reports", pbi, complete, preps );
         childWorkItem = CreateChildTask( workItem, "No Line Feeds", "The current ASCII printing does not contain line feeds", bug, cancelled );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Line Breaks", "Each line is on its own line", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Create Postscript", "Create a postscript file of the problem report", sbi, complete );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Sections", "Each section contains a bold header explaining what it is", accepted );
         AddAcceptanceCriteria( childWorkItem, "Printer", "The file is sent to the printer", accepted );
         AddAcceptanceCriteria( childWorkItem, "Configuration", "The problem report prints according to the user configuration", accepted );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Configuration Screen", "Create a screen to allow users to configure the report format", sbi, complete );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Header Font", "The user can set the font to use for the header", accepted );
         AddAcceptanceCriteria( childWorkItem, "Header Font Size", "The user can set the font size to use for the header", accepted );
         AddAcceptanceCriteria( childWorkItem, "Header Bold", "The user can choose if the header is bold or regular", accepted );
         AddAcceptanceCriteria( childWorkItem, "Header Default", "The header settings default to Verdana, 14 point, bold", accepted );
         AddAcceptanceCriteria( childWorkItem, "Text Font", "The user can set the font to use for the body text", accepted );
         AddAcceptanceCriteria( childWorkItem, "Text Font Size", "The user can set the font size to use for the body text", accepted );
         AddAcceptanceCriteria( childWorkItem, "Text Bold", "The user can choose if the body text is bold or regular", accepted );
         AddAcceptanceCriteria( childWorkItem, "Text Default", "The body text settings default to Verdana, 10 point, regular", accepted );
         CloseCriteriaList( childWorkItem );
         
         workItem = CreateWorkItem( "Hummus Sandwich", "As a vegan, I want a tasty, tasty sandwich without any animal product in it", pbi, assigned, sandwiches );
         childWorkItem = CreateChildTask( workItem, "All Meat", "All of your sandwiches contain dead animals, nothing for vegans to eat", bug, assigned );
         childWorkItem = CreateChildTask( workItem, "Make Hummus", "Make Hummus", sbi, assigned );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The hummus does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The hummus does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Chickpeas", "The primary ingredient is chickpeas", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The hummus tastes good", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Bread", "Make Vegan bread", sbi, complete );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The bread does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The bread does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The bread tastes good", unverified );
         AddAcceptanceCriteria( childWorkItem, "Fluffy", "The bread is properly leavened and not flat", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Assemble", "Make the sandwich", sbi, complete );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The sandwich does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The sandwich does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The sandwich tastes good", unverified );
         CloseCriteriaList( childWorkItem );
         
         workItem = CreateWorkItem( "Burndown Chart", "As a user, I want a quick and easy indication of the progress of work on a sprint", customerRequest, assigned, homeScrum );
         childWorkItem = CreateChildTask( workItem, "Burndown Store", "Create a table that is used to store the burndown", sbi, newWorkItem );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Information", "At a minimum, contains sprint, date, and remaining", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( workItem, "Generator", "Create a routine that generates the burndown", sbi, newWorkItem );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "All Active Sprints", "Routine includes all active sprints", unverified );
         AddAcceptanceCriteria( childWorkItem, "Accurate", "Routine accurately calculates the remaining value for each active sprint", unverified );
         AddAcceptanceCriteria( childWorkItem, "Store", "Routine stores the calculated value in the burndown table", unverified );
         CloseCriteriaList( childWorkItem );
         
         workItem = CreateWorkItem( "Quadratic Equations", "As a parent, I want to teach my child to sovle quadradic equations", customerRequest, cancelled, mathWar );
         childWorkItem = CreateChildTask( workItem, "Too Complex", "I think this is too complex for a game like this, and we should consider cancelling the request", issue, complete );
         
         // 2 PBI's and 3 CR's without tasks
         //    ** New (3)
         //    ** Planning (2)
         workItem = CreateWorkItem( "Add Sprints", "As a manager, I want to organize backlog and tasks into workable timeframes.",
            pbi, newWorkItem, homeScrum );
         workItem = CreateWorkItem( "Hamburger", "As a hungry person, I want something made with chopped up dead cow.",
            pbi, newWorkItem, sandwiches );
         workItem = CreateWorkItem( "LDAP Server", "As a manager, I would like to have the ability to use LDAP as a user store.",
            customerRequest, newWorkItem, preps );
         workItem = CreateWorkItem( "Division", "As a parent, I want to teach my kid how to do division.",
            customerRequest, planning, mathWar );
         workItem = CreateWorkItem( "BLT", "As a hungry customer, I would like a sandwich made with Bacon.",
            customerRequest, planning, sandwiches );


         // Stand alone tasks
         // * Issues (4)
         // * Bugs (6)
         // * SBIs (5)
         // Various statuses, all SBI's either new or planning, two of each with Acceptance Criteria (Untested)
         workItem = CreateWorkItem( "No LDAP Server", "I cannot actually work on the LDAP server related stuff until I have a test server", issue, assigned, preps );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Exists", "The LDAP Server exists", accepted );
         AddAcceptanceCriteria( workItem, "Accessible", "I can access the LDAP Server without error", rejected );
         AddAcceptanceCriteria( workItem, "Contains Data", "The LDAP server has test data I can use", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( "Modly Bread", "I cannot make a BLT if all of the bread has mold on it.", issue, assigned, sandwiches );
         workItem = CreateWorkItem( "Remainders", "No one has defined how remainders should be dealt with.", issue, cancelled, mathWar );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Playable", "The way remainders are to be handled is always playable by the user", unverified );
         AddAcceptanceCriteria( workItem, "Configurable", "The specification allows for user configuration", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( "Negative Numbers", "No one will define how they want negative numbers handled by the game", issue, newWorkItem, mathWar );

         workItem = CreateWorkItem( "No Admin", "It is possible to set all users to not be admins, which makes it impossible to then administer the system", bug, complete, preps );
         workItem = CreateWorkItem( "Bug in Soup", "Waiter, there is a bug in my soup.", bug, newWorkItem, sandwiches );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Removed", "The current bowl of soup is removed", accepted );
         AddAcceptanceCriteria( workItem, "New Soup", "A new bowl of soup is delivered to the table, not just the old soup with the bug removed", unverified );
         AddAcceptanceCriteria( workItem, "No Bug", "There is no bug in the new bowl of soup", unverified );
         AddAcceptanceCriteria( workItem, "Hot", "The new bowl of soup is hot", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( "Addition Error", "The program seems to randomly think that 2 + 2 = 8.", bug, assigned, mathWar );
         workItem = CreateWorkItem( "Cannot assign sprint", "Attempt to assign an SBI to a sprint, the sprint does not stay selected.  It is not being saved in the database.", bug, assigned, homeScrum );
         workItem = CreateWorkItem( "Hide not working", "The hiding of completed work items is having no effect.", bug, cancelled, homeScrum );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Hide", "Completed work items are hidden when hide is active", accepted );
         AddAcceptanceCriteria( workItem, "Show", "Completed work items are shown when hide is not active", accepted );
         AddAcceptanceCriteria( workItem, "No Server Hit", "The server is not hit at all during a show or a hide", accepted );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( "Bacon is Canadian", "The bacon for the BLT is Canadian.  That isn't really bacon, but more like ham.", bug, planning, sandwiches );

         workItem = CreateWorkItem( "Add Last Active Admin Edit", "When a user is made not an admin, make sure they are not the last active admin user.", sbi, newWorkItem, preps );
         workItem = CreateWorkItem( "Sprint Model", "Implement the domain model for sprints", sbi, newWorkItem, homeScrum );
         workItem = CreateWorkItem( "Add backlog to sprint", "Modify the sprint screen to allow the addition of one or more backlog items", sbi, planning, homeScrum );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "All Tasks Added", "All tasks associated with the backlog item are automatically added to the sprint", unverified );
         AddAcceptanceCriteria( workItem, "Estimate Updated", "The estimate for the completion of the sprint is updated based on the estimates on the individual tasks", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( "Add Mayo", "Spread mayo on the toast", sbi, planning, sandwiches );
         workItem = CreateWorkItem( "Add Division Configuration", "Update the configuration screen to include a configuration section.  Save the configuration to the INI", sbi, planning, mathWar );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Remainder Setting", "Are remainders entered? toggle.", unverified );
         AddAcceptanceCriteria( workItem, "Difficulty Setting", "Difficulting setting (easy, medium, difficult)", unverified );
         AddAcceptanceCriteria( workItem, "Defaults", "Remainder defaults to Yes.  Difficulty defaults to medium", unverified );
         AddAcceptanceCriteria( workItem, "Saved to INI", "Whatever elements are set are saved to the INI, even if they match the default", unverified );
         CloseCriteriaList( workItem );

         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #2",
               Description = "Description #2",
               WorkItemType = WorkItemTypes.ModelData[2],
               Status = WorkItemStatuses.ModelData[1],
               CreatedByUser = Users.ModelData[2],
               LastModifiedUserRid = Users.ModelData[0].Id,
               AssignedToUser = Users.ModelData[1],
               Project = Projects.ModelData[0]
            } );
         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #3",
               Description = "Description #3",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[2],
               CreatedByUser = Users.ModelData[1],
               LastModifiedUserRid = Users.ModelData[0].Id,
               Project = Projects.ModelData[2]
            } );
         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #4",
               Description = "Description #4",
               WorkItemType = WorkItemTypes.ModelData[1],
               Status = WorkItemStatuses.ModelData[2],
               CreatedByUser = Users.ModelData[0],
               LastModifiedUserRid = Users.ModelData[1].Id,
               AssignedToUser = Users.ModelData[2],
               Project = Projects.ModelData[1]
            } );
         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #5",
               Description = "Description #5",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[1],
               CreatedByUser = Users.ModelData[0],
               LastModifiedUserRid = Users.ModelData[1].Id,
               Project = Projects.ModelData[2]
            } );
         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #6",
               Description = "Description #6",
               WorkItemType = WorkItemTypes.ModelData[1],
               Status = WorkItemStatuses.ModelData[0],
               CreatedByUser = Users.ModelData[2],
               LastModifiedUserRid = Users.ModelData[1].Id,
               AssignedToUser = Users.ModelData[2],
               Project = Projects.ModelData[0]
            } );
         _workItems.Add(
            new WorkItem()
            {
               Name = "Work Item #7",
               Description = "PBI that is not open",
               WorkItemType = WorkItemTypes.ModelData.First( x => !x.IsTask ),
               Status = WorkItemStatuses.ModelData.First( x => !x.IsOpenStatus ),
               CreatedByUser = Users.ModelData[2],
               LastModifiedUserRid = Users.ModelData[1].Id,
               AssignedToUser = Users.ModelData[2],
               Project = Projects.ModelData[0]
            } );
      }

      private static void InitializeWorkItemTypes()
      {
         pbi = WorkItemTypes.ModelData.First( x => x.Name == "PBI" );
         customerRequest = WorkItemTypes.ModelData.First( x => x.Name == "CR" );
         bug = WorkItemTypes.ModelData.First( x => x.Name == "Bug" );
         issue = WorkItemTypes.ModelData.First( x => x.Name == "Issue" );
         sbi = WorkItemTypes.ModelData.First( x => x.Name == "SBI" );
      }

      private static void InitializeWorkItemStatuses()
      {
         newWorkItem = WorkItemStatuses.ModelData.First( x => x.Name == "New" );
         planning = WorkItemStatuses.ModelData.First( x => x.Name == "Planning" );
         assigned = WorkItemStatuses.ModelData.First( x => x.Name == "Assigned" );
         complete = WorkItemStatuses.ModelData.First( x => x.Name == "Complete" );
         cancelled = WorkItemStatuses.ModelData.First( x => x.Name == "Cancelled" );
      }

      private static void InitializeAcceptanceCriteriaStatuses()
      {
         unverified = AcceptanceCriteriaStatuses.ModelData.First( x => x.Name == "Unverified" );
         accepted = AcceptanceCriteriaStatuses.ModelData.First( x => x.Name == "Accepted" );
         rejected = AcceptanceCriteriaStatuses.ModelData.First( x => x.Name == "Rejected" );
      }

      private static void InitializeProjects()
      {
         homeScrum = Projects.ModelData.First( x => x.Name == "Home Scrum" );
         sandwiches = Projects.ModelData.First( x => x.Name == "Sandwiches" );
         preps = Projects.ModelData.First( x => x.Name == "PRepS" );
         mathWar = Projects.ModelData.First( x => x.Name == "MathWar" );
      }

      private static WorkItem CreateWorkItem( string name, string description, WorkItemType wit, WorkItemStatus status, Project project )
      {
         var workItem = new WorkItem()
         {
            Name = name,
            Description = description,
            WorkItemType = wit,
            Status = status,
            CreatedByUser = GetRandomUser(),
            LastModifiedUserRid = GetRandomUser().Id,
            Project = project
         };
         _workItems.Add( workItem );

         return workItem;
      }

      private static WorkItem CreateChildTask( WorkItem backlogItem, string name, string description, WorkItemType wit, WorkItemStatus status )
      {
         var workItem = new WorkItem()
         {
            ParentWorkItem = backlogItem,
            Name = name,
            Description = description,
            WorkItemType = wit,
            Status = status,
            Project = backlogItem.Project,
            CreatedByUser = backlogItem.CreatedByUser,
            LastModifiedUserRid = backlogItem.LastModifiedUserRid
         };

         _workItems.Add( workItem );

         return workItem;
      }

      private static void OpenCriteriaList()
      {
         _criteria = new List<AcceptanceCriterion>();
      }

      private static void CloseCriteriaList( WorkItem workItem )
      {
         workItem.AcceptanceCriteria = _criteria.ToArray();
         _criteria = null;
      }

      private static void AddAcceptanceCriteria( WorkItem workItem, string name, string description, AcceptanceCriterionStatus status )
      {
         var criterion = new AcceptanceCriterion()
         {
            WorkItem = workItem,
            Name = name,
            Description = description,
            Status = status
         };

         _criteria.Add( criterion );
      }

      private static User GetRandomUser()
      {
         var random = new Random();
         int idx = random.Next( 3 );

         return Users.ModelData[idx];
      }

      private static void InitializeIds()
      {
         foreach (var workItem in _workItems)
         {
            workItem.Id = Guid.NewGuid();
            if (workItem.AcceptanceCriteria != null)
            {
               foreach (var criterion in workItem.AcceptanceCriteria)
               {
                  criterion.Id = Guid.NewGuid();
               }
            }
         }
      }
   }
}
