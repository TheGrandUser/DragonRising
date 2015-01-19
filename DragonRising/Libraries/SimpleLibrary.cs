using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Libraries
{
   class SimpleLibrary : ILibrary
   {
      public IEntityLibrary Entities { get; }

      public IItemLibrary Items { get; }

      public IPowerLibrary Powers { get; }

      public ITileLibrary Tiles { get; }

      public IItemUsageLibrary ItemUsages { get; }

      public IBehaviorLibrary Behaviors { get; }

      public SimpleLibrary()
      {
         this.Behaviors = new SimpleBehaviorLibrary();
         var entities = new SimpleEntityLibrary();

         this.Entities = entities;
         this.Items = new SimpleItemLibrary();
         this.Powers = new SimplePowerLibrary();
         this.Tiles = new SimpleTileLibrary();
         this.ItemUsages = new SimpleItemUsagesLibrary();

         entities.Initialize(this.Behaviors);
      }
   }
}
