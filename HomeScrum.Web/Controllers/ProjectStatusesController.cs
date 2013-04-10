using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class ProjectStatusesController : ReadWriteController<ProjectStatus>
   {
      [Inject]
      public ProjectStatusesController( IRepository<ProjectStatus> repository, IValidator<ProjectStatus> validator )
         : base( repository, validator ) { }

      //
      // GET: /ProjectStatuses/
      public override ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<ProjectStatus>, IEnumerable<ProjectStatusViewModel>>( items ) );
      }
   }
}
