using NHibernate;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class SystemDomainObject : DomainObjectBase
   {
      public SystemDomainObject( ISessionFactory sessionFactory )
         : base( sessionFactory ) { }

      public virtual char StatusCd { get; set; }

      public virtual bool IsPredefined { get; set; }

      [Required]
      public virtual int SortSequence { get; set; }
   }
}
