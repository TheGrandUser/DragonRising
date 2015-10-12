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
      
      public ITileLibrary Tiles { get; }
      
      public IBehaviorLibrary Behaviors { get; }
      public ISpellLibrary Spells { get; }

      public SimpleLibrary()
      {
         this.Behaviors = new SimpleBehaviorLibrary();
         var entities = new SimpleEntityLibrary();

         this.Entities = entities;
         this.Items = new SimpleItemLibrary();
         this.Spells = new SimpleSpellLibrary();
         this.Tiles = new SimpleTileLibrary();

         entities.Initialize(this.Behaviors);
      }
   }
}
