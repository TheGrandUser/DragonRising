using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvsAnLib;
using System.Text.RegularExpressions;

namespace DraconicEngine.Utilities
{
   public static class StringPresentationFormatter
   {
      static readonly string aAnPattern = @"(/[aA])\s(\w+?)\b";

      public static string MakePresentable(string input)
      {
         if (input.Contains(@"//"))
         {
            var parts = input.Split(new[] { @"//" }, StringSplitOptions.None);

            return string.Join(@"/", parts.Select(p => MakePresentable(p)));
         }

         var result = Regex.Replace(input, aAnPattern,
              match =>
              {
                 var article = match.Groups[1].Value;
                 var word = match.Groups[2].Value;

                 var inflection = AvsAn.Query(word);
                 if (char.IsUpper(article[1]))
                 {
                    return inflection.Article == "a" ? "A " + word : "An " + word;
                 }

                 return inflection.Article + " " + word;
              });

         return result;
      }
   }
}
