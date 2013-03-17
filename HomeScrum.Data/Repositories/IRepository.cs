﻿using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Repositories
{
   public interface IRepository<T>
   {
      ICollection<T> GetAll();
      T Get( Guid id );

      void Add( T dataObject );
      void Update( T dataObject );
      void Delete( T dataObject );
   }
}