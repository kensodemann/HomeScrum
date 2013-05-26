using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Controllers.Base
{
   public class SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> : ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : SystemDomainObject
      where ViewModelT : SystemDomainObjectViewModel
      where EditorViewModelT : SystemDomainObjectViewModel, new()
   {
      public SystemDataObjectController( IRepository<ModelT> mainRepository, IValidator<ModelT> validator, IPropertyNameTranslator<ModelT, EditorViewModelT> translator )
         : base( mainRepository, validator, translator ) { }
   }
}