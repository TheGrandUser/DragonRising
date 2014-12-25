using DraconicEngine;
using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Generators;
using DraconicEngine.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Libraries
{
   class SimpleEntityLibrary : IEntityLibrary
   {
      Dictionary<string, Entity> templates = new Dictionary<string, Entity>();
      public Dictionary<string, Entity> Templates => templates;

      public SimpleEntityLibrary()
      {
         var alligence = AlligenceManager.Current.GetByName("Greenskins").Match(
            Some: a => a,
            None: () => { throw new Exception("No Greenskins alligence error!"); });

         Add(new Entity("Orc",
            new LocationComponent() { Blocks = true },
            new DrawnComponent() { SeenCharacter = new Character(Glyph.OLower, RogueColors.LightGreen) },
            new CombatantComponent(hp: 10, defense: 0, power: 3),
            new BehaviorComponent(new BasicMonsterBehavior()),
            new CreatureComponent(alligence, 6)));


         Add(new Entity("Troll",
            new LocationComponent() { Blocks = true },
            new DrawnComponent() { SeenCharacter = new Character(Glyph.TUpper, RogueColors.LightGreen) },
            new CombatantComponent(hp: 16, defense: 1, power: 4),
            new BehaviorComponent(new BasicMonsterBehavior()),
            new CreatureComponent(alligence, 5)));
      }

      private void Add(Entity entityTemplate)
      {
         this.templates.Add(entityTemplate.Name, entityTemplate);
      }

      public Entity Get(string templateName)
      {
         return templates[templateName];
      }
   }
}
