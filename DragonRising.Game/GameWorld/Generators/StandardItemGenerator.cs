using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using DraconicEngine.Generators;
using DraconicEngine;
using DragonRising.Entities.Items;
using DraconicEngine.Storage;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.TempConstants;

namespace DragonRising.Generators
{
   public class StandardItemGenerator : IItemGenerator
   {
      Random random = new Random();

      public Entity GenerateItem()
      {
         Entity item = null;
         var value = random.NextDouble();

         if (value <= 0.7 && Library.Items.Contains(HealingPotion))
         {
            var template = Library.Items.Get(HealingPotion);
            item = template.Create();
         }
         else if (value <= 0.85 && Library.Items.Contains(ScrollOfLightningBolt))
         {
            var template = Library.Items.Get(ScrollOfLightningBolt);
            item = template.Create();
            //item = CreateScrollOfLightningBolt();
         }
         else if (value <= 0.90 && Library.Items.Contains(ScrollOfFireball))
         {
            var template = Library.Items.Get(ScrollOfFireball);
            item = template.Create();
            //item = CreateScrollOfFireball();
         }
         else if (Library.Items.Contains(ScrollOfConfusion))
         {
            var template = Library.Items.Get(ScrollOfConfusion);
            item = template.Create();
            //item = CreateScrollOfConfusion();
         }

         return item;
      }
   }
}
