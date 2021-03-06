﻿using HomeScrum.Data.Domain;
using HomeScrum.Common.Test.Utility;
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
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<WorkItemType>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

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
               Category=WorkItemTypeCategory.BacklogItem,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemType( sessionFactory )
            {
               Name="CR",
               Description="Customer Request",
               StatusCd='A',
               Category=WorkItemTypeCategory.BacklogItem,
               IsPredefined=false,
               SortSequence=6
            },
            new WorkItemType( sessionFactory )
            {
               Name="SBI",
               Description="Sprint Backlog Item",
               StatusCd='A',
               Category=WorkItemTypeCategory.Task,
               IsPredefined=true,
               SortSequence=1
            },          
            new WorkItemType( sessionFactory )
            {
               Name="Bug",
               Description="A problem with the software or design",
               StatusCd='A',
               Category=WorkItemTypeCategory.Issue,
               IsPredefined=true,
               SortSequence=2
            },
            new WorkItemType( sessionFactory )
            {
               Name="Issue",
               Description="A problem in the process that is blocking someone",
               StatusCd='A',
               Category=WorkItemTypeCategory.Issue,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemType( sessionFactory )
            {
               Name="Design Goal",
               Description="The output of this task is a design item",
               StatusCd='A',
               Category=WorkItemTypeCategory.Task,
               IsPredefined=false,
               SortSequence=4
            },
            new WorkItemType( sessionFactory )
            {
               Name="CFP",
               Description="Customer Funded PBI",
               StatusCd='I',
               Category=WorkItemTypeCategory.BacklogItem,
               IsPredefined=false,
               SortSequence=7
            },
            new WorkItemType( sessionFactory )
            {
               Name="IFP",
               Description="Internally Funded PBI",
               StatusCd='I',
               Category=WorkItemTypeCategory.BacklogItem,
               IsPredefined=false,
               SortSequence=8
            },
            new WorkItemType( sessionFactory )
            {
               Name="WO",
               Description="Work Order",
               StatusCd='I',
               Category=WorkItemTypeCategory.Task,
               IsPredefined=false,
               SortSequence=9
            },
            new WorkItemType( sessionFactory )
            {
               Name="PL",
               Description="Problem Log",
               StatusCd='I',
               Category=WorkItemTypeCategory.Issue,
               IsPredefined=false,
               SortSequence=10
            },
            new WorkItemType( sessionFactory )
            {
               Name="Step",
               Description="A specific step required to complete and CFP or IFP",
               StatusCd='I',
               Category=WorkItemTypeCategory.Task,
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
