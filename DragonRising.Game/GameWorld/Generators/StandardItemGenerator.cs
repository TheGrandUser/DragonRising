using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using static DragonRising.TempConstants;
using DragonRising.Storage;
using DraconicEngine.Utilities;
using static DraconicEngine.Utilities.ItemSelection;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DragonRising.Generators
{
   public class StandardItemGenerator : IItemGenerator
   {
      Dictionary<Entity, Either<double, IEnumerable<Tuple<double, int>>>> items;
      List<Tuple<int, int>> itemsPerRoomByLevel;

      public StandardItemGenerator()
      {
         this.itemsPerRoomByLevel = new List<Tuple<int, int>>
         {
            tuple(1,1),
            tuple(2,4),
         };

         items = new Dictionary<Entity, Either<double, IEnumerable<Tuple<double, int>>>>()
         {
            { Library.Current.Items.Get(HealingPotion), Make(0.35) },
            { Library.Current.Items.Get(ScrollOfLightningBolt), Make(tuple(0.25, 4)) },
            { Library.Current.Items.Get(ScrollOfFireball), Make(tuple(0.25, 6)) },
            { Library.Current.Items.Get(ScrollOfConfusion), Make(tuple(0.10, 2)) },
         };
      }

      public Entity GenerateItem(int level)
      {
         var template = ItemSelection.RandomChoice(level, items);

         return template.Clone();
      }
   }
}
