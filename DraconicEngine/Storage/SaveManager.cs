using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public interface ISaveManager
   {
      //System.Xml.
   }

   public class JsonSaveManger : ISaveManager
   {
      void SaveEntity(Entity entity)
      {
         //var serializer = new DataContractJsonSerializer(typeof(Entity),
         //   new Type[] { typeof(Creature), typeof(Item) });

         //System.Xml.Serialization.
      }
   }

   public static class SaveManager
   {
      static ISaveManager current;

      public static ISaveManager Current { get { return current; } }

      public static void SetSaveManager(ISaveManager saveManager)
      {
         current = saveManager;
      }
   }
}
