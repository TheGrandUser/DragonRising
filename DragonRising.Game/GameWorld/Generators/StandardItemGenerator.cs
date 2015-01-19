using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.TempConstants;
using DragonRising.Storage;

namespace DragonRising.Generators
{
   public class StandardItemGenerator : IItemGenerator
   {
      Random random = new Random();

      public Entity GenerateItem()
      {
         Entity item = null;
         var value = random.NextDouble();

         if (value <= 0.7 && Library.Current.Items.Contains(HealingPotion))
         {
            var template = Library.Current.Items.Get(HealingPotion);
            item = template.Clone();
         }
         else if (value <= 0.85 && Library.Current.Items.Contains(ScrollOfLightningBolt))
         {
            var template = Library.Current.Items.Get(ScrollOfLightningBolt);
            item = template.Clone();
         }
         else if (value <= 0.90 && Library.Current.Items.Contains(ScrollOfFireball))
         {
            var template = Library.Current.Items.Get(ScrollOfFireball);
            item = template.Clone();
         }
         else if (Library.Current.Items.Contains(ScrollOfConfusion))
         {
            var template = Library.Current.Items.Get(ScrollOfConfusion);
            item = template.Clone();
         }

         return item;
      }
   }
}
