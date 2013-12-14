using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Common.TestData
{
   public class WorkItems
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<WorkItem>())
         {
            using (var transaction = session.BeginTransaction())
            {
               CreateTestModelData( session );
               transaction.Commit();
            }
            session.Clear();
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory )
      {
         Users.Load( sessionFactory );
         WorkItemStatuses.Load( sessionFactory );
         WorkItemTypes.Load( sessionFactory );
         Projects.Load( sessionFactory );
         AcceptanceCriteriaStatuses.Load( sessionFactory );
         Sprints.Load( sessionFactory );
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

      private static void CreateTestModelData( ISession session, bool initializeIds = false )
      {
         InitializeProjects();
         InitializeWorkItemStatuses();
         InitializeWorkItemTypes();
         InitializeAcceptanceCriteriaStatuses();

         BuildBaseWorkItems( session );

         if (initializeIds)
         {
            InitializeIds();
            var childWorkItem = ModelData.First( x => x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' && x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' );
            var parentWorkItem = ModelData.First( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' && x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' );
            childWorkItem.ParentWorkItemRid = parentWorkItem.Id;
         }
      }

      private static void BuildBaseWorkItems( ISession session )
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
         var workItem = CreateWorkItem( session, "Add Unit Tests", "We have been bad programmers and have not been using TDD.  Get what we have tested.", pbi, planning, homeScrum );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 0 );
         var childWorkItem = CreateChildTask( session, workItem, "Examine Content", "Create catelog of our currently untested code.", sbi, newWorkItem, 4, 4 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Coverage", "All testable code is covered", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Write Tests", "Write tests to cover the code.", sbi, newWorkItem, 6, 6 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Coverage", "All testable code is covered", unverified );
         AddAcceptanceCriteria( childWorkItem, "Passing", "All tests pass", unverified );
         CloseCriteriaList( childWorkItem );

         workItem = CreateWorkItem( session, "Sprint Retrospective", "As a manager, I want to gather information on what went well with a sprint and what did not in order to improve the process",
            pbi, assigned, homeScrum );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 1 );
         childWorkItem = CreateChildTask( session, workItem, "No Retrospetive", "We need to design a retrospective", issue, complete, 4, 0 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 1 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Before Close", "Must have before close of sprint", accepted );
         AddAcceptanceCriteria( childWorkItem, "Right", "Design allows for entry of what went right", accepted );
         AddAcceptanceCriteria( childWorkItem, "Wrong", "Design allows for entry of what went wrong", accepted );
         AddAcceptanceCriteria( childWorkItem, "Improve", "Design allows for entry of what we will improve upon the next sprint", accepted );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Add Right", "Add list of things we did right", sbi, assigned, 6, 3 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 1 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", accepted );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Add Wrong", "Add list of things we did wrong", sbi, assigned, 7, 2 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 1 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Add Improvment List", "Add a list of items we need to improve upon", sbi, assigned, 3, 2 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 1 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Can Add", "Adding item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Update", "Changing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Can Delete", "Removing item is allowed", unverified );
         AddAcceptanceCriteria( childWorkItem, "Moves to Next Sprint", "Is viewable while setting up next sprint", unverified );
         CloseCriteriaList( childWorkItem );

         workItem = CreateWorkItem( session, "Problem Report Printing", "As a user, I want to be able to print hard copies of problem reports", pbi, complete, preps );
         childWorkItem = CreateChildTask( session, workItem, "No Line Feeds", "The current ASCII printing does not contain line feeds", bug, cancelled, 8, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Line Breaks", "Each line is on its own line", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Create Postscript", "Create a postscript file of the problem report", sbi, complete, 12, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Sections", "Each section contains a bold header explaining what it is", accepted );
         AddAcceptanceCriteria( childWorkItem, "Printer", "The file is sent to the printer", accepted );
         AddAcceptanceCriteria( childWorkItem, "Configuration", "The problem report prints according to the user configuration", accepted );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Configuration Screen", "Create a screen to allow users to configure the report format", sbi, complete, 5, 0 );
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

         workItem = CreateWorkItem( session, "Hummus Sandwich", "As a vegan, I want a tasty, tasty sandwich without any animal product in it", pbi, assigned, sandwiches );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == sandwiches.Id ).ElementAt( 0 );
         childWorkItem = CreateChildTask( session, workItem, "All Meat", "All of your sandwiches contain dead animals, nothing for vegans to eat", bug, assigned, 9, 3 );
         childWorkItem = CreateChildTask( session, workItem, "Make Hummus", "Make Hummus", sbi, assigned, 4, 2 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The hummus does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The hummus does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Chickpeas", "The primary ingredient is chickpeas", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The hummus tastes good", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Bread", "Make Vegan bread", sbi, complete, 11, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The bread does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The bread does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The bread tastes good", unverified );
         AddAcceptanceCriteria( childWorkItem, "Fluffy", "The bread is properly leavened and not flat", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Assemble", "Make the sandwich", sbi, complete, 2, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "No dead animals", "The sandwich does not contain the flesh of a dead animal", unverified );
         AddAcceptanceCriteria( childWorkItem, "No animal products", "The sandwich does not contain any animal by-product such as milk, egg, or cheese", unverified );
         AddAcceptanceCriteria( childWorkItem, "Tasty", "The sandwich tastes good", unverified );
         CloseCriteriaList( childWorkItem );

         workItem = CreateWorkItem( session, "Burndown Chart", "As a user, I want a quick and easy indication of the progress of work on a sprint", customerRequest, assigned, homeScrum );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 4 );
         childWorkItem = CreateChildTask( session, workItem, "Burndown Store", "Create a table that is used to store the burndown", sbi, newWorkItem, 3, 3 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 4 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "Information", "At a minimum, contains sprint, date, and remaining", unverified );
         CloseCriteriaList( childWorkItem );
         childWorkItem = CreateChildTask( session, workItem, "Generator", "Create a routine that generates the burndown", sbi, newWorkItem, 5, 5 );
         childWorkItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 4 );
         OpenCriteriaList();
         AddAcceptanceCriteria( childWorkItem, "All Active Sprints", "Routine includes all active sprints", unverified );
         AddAcceptanceCriteria( childWorkItem, "Accurate", "Routine accurately calculates the remaining value for each active sprint", unverified );
         AddAcceptanceCriteria( childWorkItem, "Store", "Routine stores the calculated value in the burndown table", unverified );
         CloseCriteriaList( childWorkItem );

         workItem = CreateWorkItem( session, "Quadratic Equations", "As a parent, I want to teach my child to sovle quadradic equations", customerRequest, cancelled, mathWar );
         childWorkItem = CreateChildTask( session, workItem, "Too Complex", "I think this is too complex for a game like this, and we should consider cancelling the request", issue, complete, 12, 0 );

         // 2 PBI's and 3 CR's without tasks
         //    ** New (3)
         //    ** Planning (2)
         workItem = CreateWorkItem( session, "Add Sprints", "As a manager, I want to organize backlog and tasks into workable timeframes.",
            pbi, newWorkItem, homeScrum );
         workItem = CreateWorkItem( session, "Hamburger", "As a hungry person, I want something made with chopped up dead cow.",
            pbi, newWorkItem, sandwiches );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == sandwiches.Id ).ElementAt( 0 );
         workItem = CreateWorkItem( session, "LDAP Server", "As a manager, I would like to have the ability to use LDAP as a user store.",
            customerRequest, newWorkItem, preps );
         workItem = CreateWorkItem( session, "Division", "As a parent, I want to teach my kid how to do division.",
            customerRequest, planning, mathWar );
         workItem = CreateWorkItem( session, "BLT", "As a hungry customer, I would like a sandwich made with Bacon.",
            customerRequest, planning, sandwiches );


         // Stand alone tasks
         // * Issues (4)
         // * Bugs (6)
         // * SBIs (5)
         // Various statuses, all SBI's either new or planning, two of each with Acceptance Criteria (Untested)
         workItem = CreateWorkItem( session, "No LDAP Server", "I cannot actually work on the LDAP server related stuff until I have a test server", issue, assigned, preps, 5, 4 );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == preps.Id ).ElementAt( 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Exists", "The LDAP Server exists", accepted );
         AddAcceptanceCriteria( workItem, "Accessible", "I can access the LDAP Server without error", rejected );
         AddAcceptanceCriteria( workItem, "Contains Data", "The LDAP server has test data I can use", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( session, "Modly Bread", "I cannot make a BLT if all of the bread has mold on it.", issue, assigned, sandwiches, 6, 3 );
         workItem = CreateWorkItem( session, "Remainders", "No one has defined how remainders should be dealt with.", issue, cancelled, mathWar, 2, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Playable", "The way remainders are to be handled is always playable by the user", unverified );
         AddAcceptanceCriteria( workItem, "Configurable", "The specification allows for user configuration", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( session, "Negative Numbers", "No one will define how they want negative numbers handled by the game", issue, newWorkItem, mathWar, 7, 7 );

         workItem = CreateWorkItem( session, "No Admin", "It is possible to set all users to not be admins, which makes it impossible to then administer the system", bug, complete, preps, 2, 0 );
         workItem = CreateWorkItem( session, "Bug in Soup", "Waiter, there is a bug in my soup.", bug, newWorkItem, sandwiches, 1, 1 );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == sandwiches.Id ).ElementAt( 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Removed", "The current bowl of soup is removed", accepted );
         AddAcceptanceCriteria( workItem, "New Soup", "A new bowl of soup is delivered to the table, not just the old soup with the bug removed", unverified );
         AddAcceptanceCriteria( workItem, "No Bug", "There is no bug in the new bowl of soup", unverified );
         AddAcceptanceCriteria( workItem, "Hot", "The new bowl of soup is hot", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( session, "Addition Error", "The program seems to randomly think that 2 + 2 = 8.", bug, assigned, mathWar, 2, 1 );
         workItem = CreateWorkItem( session, "Cannot assign sprint", "Attempt to assign an SBI to a sprint, the sprint does not stay selected.  It is not being saved in the database.", bug, assigned, homeScrum, 4, 3 );
         workItem = CreateWorkItem( session, "Hide not working", "The hiding of completed work items is having no effect.", bug, cancelled, homeScrum, 4, 0 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Hide", "Completed work items are hidden when hide is active", accepted );
         AddAcceptanceCriteria( workItem, "Show", "Completed work items are shown when hide is not active", accepted );
         AddAcceptanceCriteria( workItem, "No Server Hit", "The server is not hit at all during a show or a hide", accepted );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( session, "Bacon is Canadian", "The bacon for the BLT is Canadian.  That isn't really bacon, but more like ham.", bug, planning, sandwiches, 5, 5 );

         workItem = CreateWorkItem( session, "Add Last Active Admin Edit", "When a user is made not an admin, make sure they are not the last active admin user.", sbi, newWorkItem, preps, 6, 6 );
         workItem = CreateWorkItem( session, "Sprint Model", "Implement the domain model for sprints", sbi, newWorkItem, homeScrum, 9, 9 );
         workItem = CreateWorkItem( session, "Add backlog to sprint", "Modify the sprint screen to allow the addition of one or more backlog items", sbi, planning, homeScrum, 2, 2 );
         workItem.Sprint = Sprints.ModelData.Where( x => x.Project.Id == homeScrum.Id ).ElementAt( 3 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "All Tasks Added", "All tasks associated with the backlog item are automatically added to the sprint", unverified );
         AddAcceptanceCriteria( workItem, "Estimate Updated", "The estimate for the completion of the sprint is updated based on the estimates on the individual tasks", unverified );
         CloseCriteriaList( workItem );
         workItem = CreateWorkItem( session, "Add Mayo", "Spread mayo on the toast", sbi, planning, sandwiches, 3, 3 );
         workItem = CreateWorkItem( session, "Add Division Configuration", "Update the configuration screen to include a configuration section.  Save the configuration to the INI", sbi, planning, mathWar, 5, 5 );
         OpenCriteriaList();
         AddAcceptanceCriteria( workItem, "Remainder Setting", "Are remainders entered? toggle.", unverified );
         AddAcceptanceCriteria( workItem, "Difficulty Setting", "Difficulting setting (easy, medium, difficult)", unverified );
         AddAcceptanceCriteria( workItem, "Defaults", "Remainder defaults to Yes.  Difficulty defaults to medium", unverified );
         AddAcceptanceCriteria( workItem, "Saved to INI", "Whatever elements are set are saved to the INI, even if they match the default", unverified );
         CloseCriteriaList( workItem );

         workItem = new WorkItem()
         {
            Name = "Work Item #2",
            Description = "Description #2",
            WorkItemType = WorkItemTypes.ModelData[2],
            Status = WorkItemStatuses.ModelData[1],
            CreatedByUser = Users.ModelData[2],
            LastModifiedUserRid = Users.ModelData[0].Id,
            AssignedToUser = Users.ModelData[1],
            Project = Projects.ModelData[0]
         };
         session.Save( workItem );
         _workItems.Add( workItem );

         workItem = new WorkItem()
         {
            Name = "Work Item #3",
            Description = "Description #3",
            WorkItemType = WorkItemTypes.ModelData[0],
            Status = WorkItemStatuses.ModelData[2],
            CreatedByUser = Users.ModelData[1],
            LastModifiedUserRid = Users.ModelData[0].Id,
            Project = Projects.ModelData[2]
         };
         session.Save( workItem );
         _workItems.Add( workItem );
         workItem = new WorkItem()
         {
            Name = "Work Item #4",
            Description = "Description #4",
            WorkItemType = WorkItemTypes.ModelData[1],
            Status = WorkItemStatuses.ModelData[2],
            CreatedByUser = Users.ModelData[0],
            LastModifiedUserRid = Users.ModelData[1].Id,
            AssignedToUser = Users.ModelData[2],
            Project = Projects.ModelData[1]
         };
         session.Save( workItem );
         _workItems.Add( workItem );
         workItem = new WorkItem()
         {
            Name = "Work Item #5",
            Description = "Description #5",
            WorkItemType = WorkItemTypes.ModelData[0],
            Status = WorkItemStatuses.ModelData[1],
            CreatedByUser = Users.ModelData[0],
            LastModifiedUserRid = Users.ModelData[1].Id,
            Project = Projects.ModelData[2]
         };
         session.Save( workItem );
         _workItems.Add( workItem );
         workItem = new WorkItem()
         {
            Name = "Work Item #6",
            Description = "Description #6",
            WorkItemType = WorkItemTypes.ModelData[1],
            Status = WorkItemStatuses.ModelData[0],
            CreatedByUser = Users.ModelData[2],
            LastModifiedUserRid = Users.ModelData[1].Id,
            AssignedToUser = Users.ModelData[2],
            Project = Projects.ModelData[0]
         };
         session.Save( workItem );
         _workItems.Add( workItem );
         workItem = new WorkItem()
         {
            Name = "Work Item #7",
            Description = "PBI that is not open",
            WorkItemType = WorkItemTypes.ModelData.First( x => x.Category == WorkItemTypeCategory.BacklogItem ),
            Status = WorkItemStatuses.ModelData.First( x => x.Category == WorkItemStatusCategory.Complete ),
            CreatedByUser = Users.ModelData[2],
            LastModifiedUserRid = Users.ModelData[1].Id,
            AssignedToUser = Users.ModelData[2],
            Project = Projects.ModelData[0]
         };
         session.Save( workItem );
         _workItems.Add( workItem );
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

      private static WorkItem CreateWorkItem( ISession session, string name, string description, WorkItemType wit, WorkItemStatus status, Project project, int points = 1, int pointsRemaining = 0 )
      {
         var workItem = new WorkItem()
         {
            Name = name,
            Description = description,
            WorkItemType = wit,
            Status = status,
            CreatedByUser = GetRandomUser(),
            LastModifiedUserRid = GetRandomUser().Id,
            Project = project,
            Points = points,
            PointsRemaining = pointsRemaining
         };
         session.Save( workItem );

         workItem.Tasks = new List<WorkItem>();
         _workItems.Add( workItem );

         return workItem;
      }

      private static WorkItem CreateChildTask( ISession session, WorkItem backlogItem, string name, string description, WorkItemType wit, WorkItemStatus status, int points = 1, int pointsRemaining = 0 )
      {
         var workItem = new WorkItem()
         {
            ParentWorkItemRid = backlogItem.Id,
            Name = name,
            Description = description,
            WorkItemType = wit,
            Status = status,
            Project = backlogItem.Project,
            CreatedByUser = backlogItem.CreatedByUser,
            LastModifiedUserRid = backlogItem.LastModifiedUserRid,
            Points = points,
            PointsRemaining = pointsRemaining
         };
         session.Save( workItem );
         ((List<WorkItem>)backlogItem.Tasks).Add( workItem );
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
