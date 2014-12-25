using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   [DoesNotHave(typeof(CreatureComponent))]
   [DoesNotHave(typeof(ItemComponent))]
   public class DrawNode : Node
   {
      public LocationComponent Location { get; set; }
      public DrawnComponent Drawn { get; set; }
   }
}
