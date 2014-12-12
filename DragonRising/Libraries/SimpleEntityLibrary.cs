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
      Dictionary<string, EntityTemplate> templates = new Dictionary<string, EntityTemplate>();
      public Dictionary<string, EntityTemplate> Templates => templates;

      public SimpleEntityLibrary()
      {
         var alligence = AlligenceManager.Current.GetByName("Greenskins").Match(
            Some: a => a,
            None: () => { throw new Exception("No Greenskins alligence error!"); });

         Add(new EntityTemplate("Orc", new Character(Glyph.OLower, RogueColors.LightGreen),
            new CombatantComponentTemplate(hp: 10, defense: 0, power: 3),
            new DecisionComponentTemplate(),
            new BehaviorComponentTemplate(typeof(BasicMonsterBehavior)),
            new CreatureComponentTemplate(alligence))
         { Blocks = true });


         Add(new EntityTemplate("Troll", new Character(Glyph.TUpper, RogueColors.LightGreen),
            new CombatantComponentTemplate(hp: 16, defense: 1, power: 4),
            new DecisionComponentTemplate(),
            new BehaviorComponentTemplate(typeof(BasicMonsterBehavior)),
            new CreatureComponentTemplate(alligence))
         { Blocks = true });
      }

      private void Add(EntityTemplate entityTemplate)
      {
         this.templates.Add(entityTemplate.Name, entityTemplate);
      }

      public EntityTemplate Get(string templateName)
      {
         return templates[templateName];
      }
   }
}
