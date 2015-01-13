using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.Storage;
using DragonRising.Libraries;
using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace DragonRising
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      protected override void OnLoadCompleted(NavigationEventArgs e)
      {
         base.OnLoadCompleted(e);
      }

      protected override void OnStartup(StartupEventArgs e)
      {
         AlligenceManager.SetAlligenceManager(new SimpleAlligenceManager());

         var library = new SimpleLibrary();
         Library.SetLibrary(library);

         TileLibrary.Set(library.Tiles);
         EntityLibrary.SetLibrary(library.Entities);

         SaveManager.SetSaveManager(new JsonSaveManager());

         base.OnStartup(e);
      }
   }
}
