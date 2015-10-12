using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Nodes
{
   public class BehaviorNode : Node
   {
      public BehaviorComponent Behavior { get; set; }
   }
}