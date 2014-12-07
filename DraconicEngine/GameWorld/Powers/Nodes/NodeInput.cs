
using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers.Nodes
{
   public class NodeInput
   {
      public bool Optional { get; set; }
   }

   public class NumberNodeInput : NodeInput
   {
      public int Value { get; set; }
      public int Default { get; set; }
   }

   public class EntityNodeInput : NodeInput
   {
      public IEnumerable<Entity> Value { get; set; }
   }

   public class CreatureNodeInput : NodeInput
   {
      public IEnumerable<Entity> Value { get; set; }
   }

   public class LocationNodeInput : NodeInput
   {
      public IEnumerable<Loc> Value { get; set; }
   }

   public class NodeInput<T> : NodeInput
   {
      public T Value { get; set; }
      public T Default { get; set; }
   }
}