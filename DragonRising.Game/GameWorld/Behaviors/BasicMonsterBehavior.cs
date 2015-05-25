using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DragonRising.GameWorld.Alligences;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Behaviors;

namespace DragonRising.GameWorld.Behaviors
{
   [Serializable]
   public class BasicMonsterBehavior : Behavior
   {
      public BasicMonsterBehavior()
         : base("Basic Monster")
      {

      }

      protected BasicMonsterBehavior(BasicMonsterBehavior original)
         : base(original)
      {
      }

      public override RogueAction PlanTurn(Entity owner)
      {
         RogueAction action = null;
         if (World.Current.Scene.IsVisible(owner.GetComponent<LocationComponent>().Location))
         {
            var player = World.Current.Scene.FocusEntity;
            if (player != null)
            {
               if (owner.IsAdjacent(player))
               {
                  action = new AttackEntityAction(owner, player, None);
               }
               action = MoveTowards(owner, player.GetComponent<LocationComponent>().Location);
            }
         }

         return action ?? RogueAction.Idle;
      }
      
      RogueAction MoveTowards(Entity owner, Loc targetLocation)
      {
         var locComp = owner.GetComponent<LocationComponent>();
         var directionVec = targetLocation - locComp.Location;

         Vector[] moveAttempts = directionVec.PathFindAttempts().ToArray();

         Direction dir = Direction.None;

         foreach (var moveVec in moveAttempts)
         {
            var newLoc = locComp.Location + moveVec;
            var blockage = World.Current.Scene.IsBlocked(newLoc);

            if (blockage == Blockage.None)
            {
               return new MoveInDirectionAction(owner, moveVec.ToDirection());
            }
            else if (blockage == Blockage.Entity)
            {
               var others = World.Current.Scene.EntityStore.GetEntitiesAt(newLoc);
               var blocker = others.SingleOrDefault(o => o.HasComponent<CreatureComponent>());
               if (blocker != null)
               {
                  var creatureComp = blocker.GetComponent<CreatureComponent>();
                  var myCreatureComp = owner.GetComponent<CreatureComponent>();

                  if (AlligenceManager.Current.AreEnemies(creatureComp.Alligence, myCreatureComp.Alligence))
                  {
                     dir = moveVec.ToDirection();

                     return new AttackEntityAction(owner, blocker, None);
                  }
               }
            }
         }
         return RogueAction.Idle;
      }

      protected override Behavior CloneCore()
      {
         return new BasicMonsterBehavior(this);
      }
   }
}
