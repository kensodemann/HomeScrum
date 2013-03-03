using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers.Base;
using Ninject;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : DataObjectBaseController<AcceptanceCriteriaStatus>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IDataObjectRepository<AcceptanceCriteriaStatus> repository )
         : base( repository ) { }
   }
}
