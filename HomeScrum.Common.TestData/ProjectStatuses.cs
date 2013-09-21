using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;

namespace HomeScrum.Common.TestData
{
   public class ProjectStatuses
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<ProjectStatus>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static ProjectStatus[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory, bool initializeIds = false )
      {
         ModelData = new[]
         {
            new ProjectStatus( sessionFactory )
            {
               Name="Open",
               Description="Active Project",
               StatusCd='A',
               Category=ProjectStatusCategory.Active,
               IsPredefined=true,
               SortSequence=1
            },
            new ProjectStatus( sessionFactory )
            {
               Name="Inactive",
               Description="No longer active",
               StatusCd='A',
               Category=ProjectStatusCategory.Inactive,
               IsPredefined=false,
               SortSequence=2
            },
            new ProjectStatus( sessionFactory )
            {
               Name="Closed",
               Description="The project is closed",
               StatusCd='A',
               Category=ProjectStatusCategory.Complete,
               IsPredefined=true,
               SortSequence=3
            },
            new ProjectStatus( sessionFactory )
            {
               Name="Waiting",
               Description="Awaiting Customer Approval",
               StatusCd='I',
               Category=ProjectStatusCategory.Inactive,
               IsPredefined=false,
               SortSequence=4
            },
            new ProjectStatus( sessionFactory )
            {
               Name="Deleted",
               Description="The project no longer exists",
               StatusCd='I',
               Category=ProjectStatusCategory.Inactive,
               IsPredefined=false,
               SortSequence=5
            },
            new ProjectStatus( sessionFactory )
            {
               Name="Active",
               Description="The project is active",
               StatusCd='I',
               Category=ProjectStatusCategory.Active,
               IsPredefined=false,
               SortSequence=6
            },
            new ProjectStatus( sessionFactory )
            {
               Name="On Hold",
               Description="The project is waiting for something",
               StatusCd='I',
               Category=ProjectStatusCategory.Active,
               IsPredefined=false,
               SortSequence=7
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
