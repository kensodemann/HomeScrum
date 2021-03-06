﻿using HomeScrum.Data.Validation;
using NHibernate;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class DomainObjectBase : ValidatableObject
   {
      public DomainObjectBase( ISessionFactory sessionFactory )
         : base( sessionFactory ) { }

      public virtual Guid Id { get; set; }

      [Required]
      [MaxLength( 50 )]
      public virtual string Name { get; set; }

      public virtual string Description { get; set; }
   }
}
