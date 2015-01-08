using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Alligences;

namespace DraconicEngine.GameWorld.Behaviors
{
   [Serializable]
   public class BasicMonsterBehavior : Behavior
   {
      public BasicMonsterBehavior()
      {

      }

      protected BasicMonsterBehavior(BasicMonsterBehavior original)
         : base(original)
      {
      }

      public override RogueAction PlanTurn(Entity owner)
      {
         RogueAction action = null;
         if (Scene.CurrentScene.IsVisible(owner.GetComponent<LocationComponent>().Location))
         {
            var player = Scene.CurrentScene.FocusEntity;
            if (player != null)
            {
               if (owner.IsAdjacent(player))
               {
                  action = Attack(player);
               }
               action = MoveTowards(owner, player.GetComponent<LocationComponent>().Location);
            }
         }

         return action ?? RogueAction.Idle;
      }

      RogueAction Attack(Entity player)
      {
         return new AttackEntityAction(player);
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
            var blockage = Scene.CurrentScene.IsBlocked(newLoc);

            if (blockage == Blockage.None)
            {
               return new MoveInDirectionAction(moveVec.ToDirection());
            }
            else if (blockage == Blockage.Entity)
            {
               var others = Scene.CurrentScene.EntityStore.GetEntitiesAt(newLoc);
               var blocker = others.SingleOrDefault(o => o.HasComponent<CreatureComponent>());
               if (blocker != null)
               {
                  var creatureComp = blocker.GetComponent<CreatureComponent>();
                  var myCreatureComp = owner.GetComponent<CreatureComponent>();
                     
                  if (AlligenceManager.Current.AreEnemies(creatureComp.Alligence, myCreatureComp.Alligence))
                  {
                     dir = moveVec.ToDirection();

                     return new AttackEntityAction(blocker);
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
