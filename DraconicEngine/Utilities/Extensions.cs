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

      public static IList<T> InPlaceShuffle<T>(this IList<T> items, Random r)
      {
         for (int moveFrom = items.Count - 1; moveFrom > 0; moveFrom--)
         {
            var moveTo = r.Next(moveFrom);
            var temp = items[moveTo];
            items[moveTo] = items[moveFrom];
            items[moveFrom] = temp;
         }
         
         return items;
      }
   }
}
