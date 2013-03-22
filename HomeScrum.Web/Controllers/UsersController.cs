using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace HomeScrum.Web.Controllers
{
   public class UsersController : DataObjectBaseController<User, String>
   {
      [Inject]
      public UsersController( IRepository<User, String> repository, IValidator<User> validator )
         : base( repository, validator ) { }
   }
}
