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
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItem in ModelData)
               session.Save( workItem );
            transaction.Commit();
         }
      }

      public static WorkItem[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         BuildBaseWorkItems();
         AddAcceptanceCriteria();

         if (initializeIds)
         {
            InitializeIds();
         }
      }

      private static void BuildBaseWorkItems()
      {
         ModelData = new[]
         {
            new WorkItem()
            {
               Name = "Work Item #1",
               Description = "Description #1",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[0],
               CreatedByUser = Users.ModelData[0],
               LastModifiedUserRid = Users.ModelData[0].Id,
               Project = Projects.ModelData[1]
            },
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
            },
            new WorkItem()
            {
               Name = "Work Item #3",
               Description = "Description #3",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[2],
               CreatedByUser = Users.ModelData[1],
               LastModifiedUserRid = Users.ModelData[0].Id,
               Project = Projects.ModelData[2]
            },
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
            },
            new WorkItem()
            {
               Name = "Work Item #5",
               Description = "Description #5",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[1],
               CreatedByUser = Users.ModelData[0],
               LastModifiedUserRid = Users.ModelData[1].Id,
               Project = Projects.ModelData[2]
            },
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
            },
            new WorkItem()
            {
               Name = "Work Item #7",
               Description = "PBI that is not open",
               WorkItemType = WorkItemTypes.ModelData.First(x => !x.IsTask),
               Status = WorkItemStatuses.ModelData.First(x => !x.IsOpenStatus),
               CreatedByUser = Users.ModelData[2],
               LastModifiedUserRid = Users.ModelData[1].Id,
               AssignedToUser = Users.ModelData[2],
               Project = Projects.ModelData[0]
            }
         };

         ModelData.Last().ParentWorkItem = ModelData.First();
      }

      private static void AddAcceptanceCriteria()
      {
         ModelData[0].AcceptanceCriteria = new[]
         {
            new AcceptanceCriteria()
            {
               Name = "Has all items",
               Description = "All Items Exist",
               Status = AcceptanceCriteriaStatuses.ModelData[0],
               WorkItem = ModelData[0]
            },
            new AcceptanceCriteria()
            {
               Name = "Everything Works",
               Description = "Make sure everything actually works",
               Status = AcceptanceCriteriaStatuses.ModelData[1],
               WorkItem = ModelData[0]
            }
         };

         ModelData[2].AcceptanceCriteria = new[]
         {
            new AcceptanceCriteria()
            {
               Name = "Has been done correctly",
               Description = "This item will only be accepted if it is done correctly",
               Status = AcceptanceCriteriaStatuses.ModelData[2],
               WorkItem = ModelData[2]
            }
         };

         ModelData[3].AcceptanceCriteria = new[]
         {
            new AcceptanceCriteria()
            {
               Name = "Criteria #1",
               Description = "This is criteria #1",
               Status = AcceptanceCriteriaStatuses.ModelData[0],
               WorkItem = ModelData[3]
            },
            new AcceptanceCriteria()
            {
               Name = "Criteria #2",
               Description = "This is criteria #2",
               Status = AcceptanceCriteriaStatuses.ModelData[2],
               WorkItem = ModelData[3]
            },
            new AcceptanceCriteria()
            {
               Name = "Criteria #3",
               Description = "This is criteria #3",
               Status = AcceptanceCriteriaStatuses.ModelData[0],
               WorkItem = ModelData[3]
            }
         };

         ModelData[5].AcceptanceCriteria = new[]
         {
            new AcceptanceCriteria()
            {
               Name = "Criteria #1",
               Description = "This is criteria #1",
               Status = AcceptanceCriteriaStatuses.ModelData[1],
               WorkItem = ModelData[5]
            },
            new AcceptanceCriteria()
            {
               Name = "Criteria #2",
               Description = "This is criteria #2",
               Status = AcceptanceCriteriaStatuses.ModelData[0],
               WorkItem = ModelData[5]
            }
         };
      }

      private static void InitializeIds()
      {
         foreach (var model in ModelData)
         {
            model.Id = Guid.NewGuid();
         }
      }
   }
}
