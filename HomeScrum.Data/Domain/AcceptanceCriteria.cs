﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriteria : DomainObjectBase
   {
      [Required]
      public virtual AcceptanceCriteriaStatus Status { get; set; }

      [Required]
      public virtual WorkItem WorkItem { get; set; }
   }
}