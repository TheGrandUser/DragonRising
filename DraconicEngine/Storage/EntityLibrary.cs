using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;

namespace DraconicEngine.Storage
{
   public interface IEntityLibrary
   {
      Dictionary<string, EntityTemplate> Templates { get; }

      EntityTemplate Get(string templateName);
   }

   public static class EntityLibrary
   {
      static IEntityLibrary current;
      public static IEntityLibrary Current => current;

      public static void SetLibrary(IEntityLibrary library) => current = library;
   }
}