using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SystemDomainObject : DomainObjectBase
   {
      public SystemDomainObject()
         : base() { }

      public SystemDomainObject( SystemDomainObject model )
         : base( model )
      {
         this.StatusCd = model.StatusCd;
         this.IsPredefined = model.IsPredefined;
      }

      public virtual char StatusCd { get; private set; }

      [Display( Name = "AllowUse", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool AllowUse
      {
         get { return StatusCd == 'A'; }
         set { StatusCd = value ? 'A' : 'I'; }
      }

      public virtual bool IsPredefined { get; set; }
   }
}
