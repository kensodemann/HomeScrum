using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.Repositories
{
   public interface IWorkItemRepository : IRepository<WorkItem>
   {
      ICollection<WorkItem> GetAllProductBacklog();
      ICollection<WorkItem> GetOpenProductBacklog();
   }
}
