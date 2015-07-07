﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Nodes
{
   class SeeableNode : Node
   {
      public DrawnComponent Drawn { get; set; }
   }
}
