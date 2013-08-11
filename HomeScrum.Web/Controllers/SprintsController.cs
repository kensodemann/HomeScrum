using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
    public class SprintsController : ReadWriteController<Sprint, SprintViewModel, SprintEditorViewModel>
    {
       public SprintsController( IPropertyNameTranslator<Sprint, SprintEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }
    }
}
