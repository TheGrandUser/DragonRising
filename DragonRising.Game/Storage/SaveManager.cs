using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;

namespace DragonRising.Storage
{
   public interface ISaveManager
   {
      Task SaveGame(string name, Scene scene);
      Task<Scene> LoadGame(string name);
      IEnumerable<string> GetSaveGames();
      string LastSaveGame { get; }
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
