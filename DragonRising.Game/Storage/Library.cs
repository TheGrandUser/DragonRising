using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Storage
{
   public interface ILibrary
   {
      IItemLibrary Items { get; }
      IBehaviorLibrary Behaviors { get; }
      IEntityLibrary Entities { get; }
      ITileLibrary Tiles { get; }
      IPowerLibrary Powers { get; }
   }

   public static class Library
   {
      static ILibrary current;
      public static ILibrary Current => current;
      public static void SetLibrary(ILibrary library) => current = library;
   }
}
