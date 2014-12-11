using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public static class Library
   {
      static ItemLibrary currentItemLibrary;
      public static ItemLibrary Items => currentItemLibrary;
      public static void SetItemLibrary(ItemLibrary library) => currentItemLibrary = library;
   }
}
