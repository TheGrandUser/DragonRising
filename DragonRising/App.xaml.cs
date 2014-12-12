using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.Storage;
using DragonRising.Libraries;
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
         TileLibrary.Set(new SimpleTileLibrary());
         AlligenceManager.SetAlligenceManager(new SimpleAlligenceManager());
         Library.SetItemLibrary(new SimpleItemLibrary());
         EntityLibrary.SetLibrary(new SimpleEntityLibrary());

         base.OnStartup(e);
      }
   }
}
