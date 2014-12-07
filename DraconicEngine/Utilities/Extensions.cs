using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
   public static class StringExtensions
   {
      public static string[] Split(this string str, StringSplitOptions options, params char[] seperator)
      {
         return str.Split(seperator, options);
      }
   }
}
