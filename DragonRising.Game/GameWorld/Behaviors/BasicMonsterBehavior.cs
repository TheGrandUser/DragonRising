using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Alligences;
using DragonRising.GameWorld.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

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

      public override ActionTaken PlanTurn(Entity owner)
      {
         ActionTaken action = null;
         if (World.Current.Scene.IsVisible(owner.Location))
         {
            var player = World.Current.Scene.FocusEntity;
            if (player != null)
            {
               if (owner.IsAdjacent(player))
               {
                  action = new AttackEntityAction(owner, player, None);
               }
               action = MoveTowards(owner, player.Location);
            }
         }

         return action ?? ActionTaken.Idle;
      }
      
      ActionTaken MoveTowards(Entity owner, Loc targetLocation)
      {
         var directionVec = targetLocation - owner.Location;

         Vector[] moveAttempts = directionVec.PathFindAttempts().ToArray();

         Direction dir = Direction.None;

         foreach (var moveVec in moveAttempts)
         {
            var newLoc = owner.Location + moveVec;
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
         return ActionTaken.Idle;
      }

      protected override Behavior CloneCore()
      {
         return new BasicMonsterBehavior(this);
      }
   }
}
