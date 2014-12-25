using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class CreatureDrawNode : Node
   {
      public LocationComponent Location { get; set; }
      public CreatureComponent Creature { get; set; }
      public DrawnComponent Drawn { get; set; }
   }
}
