﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class User
   {
      public virtual string UserId { get; set; }
      public virtual string FirstName { get; set; }
      public virtual string MiddleName { get; set; }
      public virtual string LastName { get; set; }
      public virtual char StatusCd { get; set; }
   }
}
