using System;
using System.Web.Mvc;

namespace HomeScrum.Web.Attributes
{
#if DEBUG
   public class ReleaseRequireHttpsAttribute : Attribute
   {
       // no-op
   }
#else
   public class ReleaseRequireHttpsAttribute : RequireHttpsAttribute
   {
       // does the same thing as RequireHttpsAttribute
   }
#endif
}