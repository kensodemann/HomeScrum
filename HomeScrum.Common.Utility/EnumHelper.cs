using System;

namespace HomeScrum.Common.Utility
{
   public static class EnumHelper
   {
      public static String GetDescription( object value )
      {
         string description = value.ToString();
         var fieldInfo = value.GetType().GetField( value.ToString() );

         if (fieldInfo != null)
         {
            var attrs = fieldInfo.GetCustomAttributes( typeof( System.ComponentModel.DescriptionAttribute ), true );
            if (attrs != null && attrs.Length > 0)
            {
               description = ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
            }
         }

         return description;
      }
   }
}
