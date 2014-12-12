using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising
{
   public class JsonSaveManger : ISaveManager
   {
      public IEnumerable<string> GetSaveGames()
      {
         throw new NotImplementedException();
      }

      public Task LoadGAme(string filePath)
      {
         throw new NotImplementedException();
      }

      public Task SaveGame(string filePath)
      {
         throw new NotImplementedException();
      }

      void SaveEntity(Entity entity)
      {
         //var serializer = new DataContractJsonSerializer(typeof(Entity),
         //   new Type[] { typeof(Creature), typeof(Item) });

         //System.Xml.Serialization.
      }
   }
}
