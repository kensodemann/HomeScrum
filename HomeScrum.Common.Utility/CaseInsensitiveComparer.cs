using System;
using System.Collections.Generic;

namespace HomeScrum.Common.Utility
{
   public class CaseInsensitiveComparer : IComparer<string>
   {
      public int Compare( string x, string y )
      {
         return string.Compare( x, y, StringComparison.OrdinalIgnoreCase );
      }
   }
}
