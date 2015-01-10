using DraconicEngine;
using DraconicEngine.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising
{
   class SaveManager : ISaveManager
   {
      public IEnumerable<string> GetSaveGames()
      {
         var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var saveGamesDir = Path.Combine(documents, "My Games", "Dragon Rising", "Saves");
         
         if (Directory.Exists(saveGamesDir))
         {
            return Directory.EnumerateFiles(saveGamesDir, "*.sav");
         }

         return Enumerable.Empty<string>();
      }

      public Task<Scene> LoadGAme(string filePath)
      {

         throw new NotImplementedException();
      }

      public Task SaveGame(string filePath, Scene scene)
      {
         throw new NotImplementedException();
      }
   }
}
