using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Repositories
{
   public interface IDataObjectRepository<DataObjectType> where DataObjectType : BaseDataObject
   {
      ICollection<DataObjectType> GetAll();
      DataObjectType Get( Guid id );

      void Add( DataObjectType dataObject );
      void Update( DataObjectType dataObject );
      void Delete( DataObjectType dataObject );
   }
}
