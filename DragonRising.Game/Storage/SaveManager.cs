﻿using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DragonRising.GameWorld;

namespace DragonRising.Storage
{
   public interface ISaveManager
   {
      Task SaveGame(string name, World world);
      Task<World> LoadGame(string name);
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
