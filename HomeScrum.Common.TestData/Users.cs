using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;

namespace HomeScrum.Common.TestData
{
   public static class Users
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<User>())
         {
            CreateTestModelData( );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static User[] ModelData { get; private set; }

      private static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new User()
            {
               UserName = "kws",
               LastName = "Smith",
               FirstName = "Kevin",
               MiddleName="William",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "j_a",
               LastName = "Anderson",
               FirstName = "Judy",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "q__",
               FirstName = "Quintin",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "iai",
               FirstName = "I",
               MiddleName = "Am",
               LastName= "Inactive",
               StatusCd = 'I'
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
