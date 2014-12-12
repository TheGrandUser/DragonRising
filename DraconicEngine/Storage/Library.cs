using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public static class Library
   {
      static IItemLibrary currentItemLibrary;
      public static IItemLibrary Items => currentItemLibrary;
      public static void SetItemLibrary(IItemLibrary library) => currentItemLibrary = library;
   }
}