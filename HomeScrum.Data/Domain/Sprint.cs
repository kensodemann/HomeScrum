﻿using NHibernate;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class Sprint : DomainObjectBase
   {
      [Required]
      public virtual SprintStatus Status { get; set; }

      [Required]
      public virtual Project Project { get; set; }

      public virtual string Goal { get; set; }

      [Required]
      public virtual Guid LastModifiedUserRid { get; set; }

      [Required]
      public virtual User CreatedByUser { get; set; }

      public virtual DateTime? StartDate { get; set; }

      public virtual DateTime? EndDate { get; set; }

      [Required]
      [Range(1, 32767)]
      public virtual int Capacity { get; set; }

      public virtual ICollection<SprintCalendarEntry> Calendar { get; set; }

      #region Non-POCO stuff
      public Sprint()
         : this( null ) { }

      [Inject]
      public Sprint( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Sprint";
      }
      #endregion
   }
}
