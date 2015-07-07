using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Alligences;
using DragonRising.GameWorld.Behaviors;
using DragonRising.GameWorld.Components;
using DragonRising.Storage;
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
      }

      internal void Initialize(IBehaviorLibrary behaviors)
      {
         Add(new Entity("Orc",
            new ComponentSet(
               new DrawnComponent() { SeenCharacter = new Character(Glyph.OLower, RogueColors.LightGreen) },
               new CombatantComponent(hp: 10, defense: 0, power: 3),
               new BehaviorComponent(behaviors.Get("Basic Monster")),
               new CreatureComponent(6)),
            new StatSet(CharacterStat.Make(CoreStats.XP, 35)))
         { Blocks = true });


         Add(new Entity("Troll",
            new ComponentSet(
               new DrawnComponent() { SeenCharacter = new Character(Glyph.TUpper, RogueColors.LightGreen) },
               new CombatantComponent(hp: 16, defense: 1, power: 4),
               new BehaviorComponent(behaviors.Get("Basic Monster")),
               new CreatureComponent(5)),
            new StatSet(CharacterStat.Make(CoreStats.XP, 100)))
         { Blocks = true });
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
