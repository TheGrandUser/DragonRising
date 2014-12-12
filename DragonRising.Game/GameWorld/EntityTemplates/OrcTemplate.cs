using DraconicEngine;
using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.EntityTemplates
{
   //class OrcTemplate : EntityTemplate
   //{
   //   public OrcTemplate()
   //   {

   //   }

   //   protected override Entity CreateEntityCore()
   //   {
   //      var greenskins = AlligenceManager.Current.GetOrAddByName("greenskins");
         
   //      var monster = new Entity("Orc", Glyph.OLower, RogueColors.LightGreen, blocks: true);

   //      monster.AddComponent(new BehaviorComponent(new BasicMonsterBehavior()));
   //      monster.AddComponent(new CombatantComponent(hp: 10, defense: 0, power: 3));
   //      monster.AddComponent(new CreatureComponent() { Alligence = greenskins });
   //      monster.AddComponent(new DecisionComponent());

   //      //inventory
   //      //timer

   //      return monster;
   //   }
   //}
}