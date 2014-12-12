using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers.Nodes
{
   public class NodeOutput
   {
   }

   public class NodeOutput<T> : NodeOutput
   {
      Subject<T> subject = new Subject<T>();
      
      public IObservable<T> Observable { get { return subject.AsObservable(); } }
      public void Pipe(T item)
      {
         this.subject.OnNext(item);
      }
      public void Pipe(IEnumerable<T> items)
      {
         foreach(var item in items)
         {
            this.subject.OnNext(item);
         }
      }
   }

   public class EntityNodeOutput : NodeOutput<Entity>
   {
   }

   public class CreatureNodeOutput : NodeOutput<Entity>
   {
   }

   public class ItemNodeOutput : NodeOutput<Entity>
   {
   }

   public class LocationNodeOutput : NodeOutput<Loc>
   {
   }

   public class NumberNodeOutput : NodeOutput<int>
   {
   }
}