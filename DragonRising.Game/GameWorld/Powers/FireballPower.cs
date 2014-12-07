using DraconicEngine.Powers;
using DraconicEngine.Powers.Effects;
using DraconicEngine.Powers.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Powers
{
   class FireballPower : Power
   {
      public FireballPower()
      {
         var chooseLocationNode = new ChooseLocationNode();
         var getCreaturesWithin = new GetEntitiesWithinNode() { Radius = 3, LineOfEffect = true };
         var damageValue = new NumberConstantNode() { Value = 12 };
         var damageEffect = new DamageEffect();

         var locationLink = new NodeConnection()
         {
            Output = chooseLocationNode.LocationOutput,
            Input = getCreaturesWithin.LocationInput,
         };


      }
   }
}
