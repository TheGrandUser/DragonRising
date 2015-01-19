using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Nodes
{
   [DoesNotHave(typeof(CreatureComponent))]
   [DoesNotHave(typeof(ItemComponent))]
   public class DrawNode : Node
   {
      public LocationComponent Location { get; set; }
      public DrawnComponent Drawn { get; set; }
   }
}
