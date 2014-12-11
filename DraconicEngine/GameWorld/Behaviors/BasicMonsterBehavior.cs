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
   public class BasicMonsterBehavior : IBehavior
   {
      public RogueAction PlanTurn(Entity owner)
      {
         RogueAction action = null;
         if (Scene.CurrentScene.IsVisible(owner.Location))
         {
            var player = Scene.CurrentScene.FocusEntity;
            if (player != null)
            {
               if (owner.IsAdjacent(player))
               {
                  action = Attack(player);
               }
               action = MoveTowards(owner, player.Location);
            }
         }
         if (action == null)
         {
            return RogueAction.Idle;
         }

         return action;
      }

      private RogueAction Attack(Entity player)
      {
         return new AttackEntityAction(player);
      }

      RogueAction MoveTowards(Entity owner, Loc targetLocation)
      {
         var directionVec = targetLocation - owner.Location;

         Vector[] moveAttempts = directionVec.PathFindAttempts().ToArray();

         Direction dir = Direction.None;

         foreach (var moveVec in moveAttempts)
         {
            var newLoc = owner.Location + moveVec;
            var blockage = Scene.CurrentScene.IsBlocked(newLoc);

            if (blockage == Blockage.None)
            {
               return new MoveInDirectionAction(moveVec.ToDirection());
            }
            else if (blockage == Blockage.Entity)
            {
               var others = Scene.CurrentScene.EntityStore.GetEntitiesAt(newLoc);
               var creature = others.SingleOrDefault(o => o.HasComponent<CreatureComponent>());
               if (creature != null && creature.HasComponent<CombatantComponent>())
               {

                  if (creature.As<CreatureComponent, bool>(owner, (cr1, cr2) => AlligenceManager.Current.AreEnemies(cr2.Alligence, cr1.Alligence)))
                  {
                     dir = moveVec.ToDirection();

                     return new AttackEntityAction(creature);
                  }
               }
            }
         }
         return RogueAction.Idle;
      }

      public Item SelectInventoryItem(Entity owner) => null;

      public Loc? SelectTargetLocation(Entity owner, bool isLimitedToFoV = true) => null;

      public Entity SelectTargetCreature(Entity owner, int range = 0) => null;
   }
}
