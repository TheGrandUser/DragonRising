using DraconicEngine.GameWorld.Effects;
using DraconicEngine.Powers;
using DraconicEngine.Powers.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using System.Collections.Immutable;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DragonRising.Powers
{
   class FireballPower : Power
   {
      public FireballPower()
      {
         var chooseLocationNode = new ChooseLocationNode();
         var getCreaturesWithin = new GetEntitiesWithinNode() { Radius = 3, LineOfEffect = true };
         var damageValue = new NumberConstantNode() { Value = 12 };

         var locationLink = new NodeConnection()
         {
            Output = chooseLocationNode.LocationOutput,
            Input = getCreaturesWithin.LocationInput,
         };


      }

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         var locationFulfillment = fulfillment as LocationFulfillment;
         var target = locationFulfillment.Location;

         var rangeSquared = 3 * 3;

         var entitiesToDamage = Scene.CurrentScene.EntityStore.AllCreaturesSpecialFirst
            .Where(entity => entity.HasComponent<CombatantComponent>() && (entity.Location - target).LengthSquared <= rangeSquared)
            .ToList();

         foreach(var entity in entitiesToDamage)
         {
            var damageEffect = new DamageEffect(initiator);
         }

         base.Do(initiator, fulfillment);
      }
   }
}
