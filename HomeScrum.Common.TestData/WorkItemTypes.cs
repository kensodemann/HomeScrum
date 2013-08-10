using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItemTypes
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         if (AlreadyLoaded( sessionFactory ))
         {
            return;
         }

         CreateTestModelData( sessionFactory );
         LoadIntoDatabase( sessionFactory );
      }

      private static void LoadIntoDatabase( ISessionFactory sessionFactory )
      {
         var session = sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            foreach (var workItemType in ModelData)
               session.Save( workItemType );
            transaction.Commit();
         }
         session.Clear();
      }

      private static bool AlreadyLoaded( ISessionFactory sessionFactory )
      {
         var session = sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var count = session.Query<WorkItemType>().Count();
            transaction.Commit();

            return count > 0;
         }
      }

      public static WorkItemType[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory, bool initializeIds = false )
      {
         ModelData = new[]
         {
            new WorkItemType( sessionFactory )
            {
               Name="PBI",
               Description="Product Backlog Item",
               StatusCd='A',
               IsTask=false,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemType( sessionFactory )
            {
               Name="CR",
               Description="Customer Request",
               StatusCd='A',
               IsTask=false,
               IsPredefined=false,
               SortSequence=6
            },
            new WorkItemType( sessionFactory )
            {
               Name="SBI",
               Description="Sprint Backlog Item",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=1
            },          
            new WorkItemType( sessionFactory )
            {
               Name="Bug",
               Description="A problem with the software or design",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=2
            },
            new WorkItemType( sessionFactory )
            {
               Name="Issue",
               Description="A problem in the process that is blocking someone",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemType( sessionFactory )
            {
               Name="Design Goal",
               Description="The output of this task is a design item",
               StatusCd='A',
               IsTask=true,
               IsPredefined=false,
               SortSequence=4
            },
            new WorkItemType( sessionFactory )
            {
               Name="CFP",
               Description="Customer Funded PBI",
               StatusCd='I',
               IsTask=false,
               IsPredefined=false,
               SortSequence=7
            },
            new WorkItemType( sessionFactory )
            {
               Name="IFP",
               Description="Internally Funded PBI",
               StatusCd='I',
               IsTask=false,
               IsPredefined=false,
               SortSequence=8
            },
            new WorkItemType( sessionFactory )
            {
               Name="WO",
               Description="Work Order",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=9
            },
            new WorkItemType( sessionFactory )
            {
               Name="PL",
               Description="Problem Log",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=10
            },
            new WorkItemType( sessionFactory )
            {
               Name="Step",
               Description="A specific step required to complete and CFP or IFP",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=11
            }
         };

         if (initializeIds)
         {
            InitializeIds();
         }
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
