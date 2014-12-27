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

      public override void Do(Entity initiator, ImmutableList<Fulfilment> fulfilments)
      {
         var fulfillment = fulfilments.Single();

         var locationFulfillment = fulfillment as FulfilmentLocation;
         var target = locationFulfillment.Value;

         var rangeSquared = 3 * 3;

         var entitiesToDamage = Scene.CurrentScene.EntityStore.AllCreaturesSpecialFirst
            .Where(entity => entity.HasComponent<CombatantComponent>() && (entity.Location - target).LengthSquared <= rangeSquared)
            .ToList();

         foreach(var entity in entitiesToDamage)
         {
            var damageEffect = new DamageEffect(initiator);
         }

         base.Do(initiator, fulfilments);
      }
   }
}
