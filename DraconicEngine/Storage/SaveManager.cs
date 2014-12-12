﻿using DraconicEngine.GameWorld.EntitySystem;
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
      Task SaveGame(string filePath);
      Task LoadGAme(string filePath);
      IEnumerable<string> GetSaveGames();
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
