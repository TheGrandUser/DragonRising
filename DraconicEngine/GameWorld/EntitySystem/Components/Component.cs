using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   [Serializable]
   public abstract class Component
   {
      public Entity Owner { get; set; }

      public Component Clone(bool fresh = true)
      {
         return CloneCore(fresh);
      }

      protected Component()
      {
      }

      protected Component(Component original, bool fresh)
      {
         // Nothing to copy yet, will be a differetn Entity Owner
      }
      protected abstract Component CloneCore(bool fresh);
   }

   public static class ComponentUtility
   {
      public static void As<TComponent>(this Entity entity, Action<TComponent> ifComponent)
         where TComponent : Component
      {
         if (entity.HasComponent<TComponent>())
         {
            var component = entity.GetComponent<TComponent>();
            ifComponent(component);
         }
      }

      public static R As<TComponent, R>(this Entity entity, Func<TComponent, R> ifComponent, R withoutComponent = default(R))
         where TComponent : Component
      {
         if (entity.HasComponent<TComponent>())
         {
            var component = entity.GetComponent<TComponent>();
            return ifComponent(component);
         }
         return withoutComponent;
      }

      public static Task<R> AsAsync<TComponent, R>(this Entity entity, Func<TComponent, Task<R>> ifComponent, R withoutComponent = default(R))
         where TComponent : Component
      {
         if (entity.HasComponent<TComponent>())
         {
            var component = entity.GetComponent<TComponent>();
            return ifComponent(component);
         }
         return Task.FromResult(withoutComponent);
      }

      public static void As<TComponent>(this Entity entity1, Entity entity2, Action<TComponent, TComponent> ifComponent)
         where TComponent : Component
      {
         if (entity1.HasComponent<TComponent>() && entity2.HasComponent<TComponent>())
         {
            var component1 = entity1.GetComponent<TComponent>();
            var component2 = entity2.GetComponent<TComponent>();
            ifComponent(component1, component2);
         }
      }

      public static R As<TComponent, R>(this Entity entity1, Entity entity2, Func<TComponent, TComponent, R> ifComponent, R withoutComponent = default(R))
         where TComponent : Component
      {
         if (entity1.HasComponent<TComponent>() && entity2.HasComponent<TComponent>())
         {
            var component1 = entity1.GetComponent<TComponent>();
            var component2 = entity2.GetComponent<TComponent>();
            return ifComponent(component1, component2);
         }
         return withoutComponent;
      }

      public static void As<T1, T2>(this Entity entity1, Entity entity2, Action<T1, T2> ifComponent)
         where T1 : Component
         where T2 : Component
      {
         if (entity1.HasComponent<T1>() && entity2.HasComponent<T2>())
         {
            var component1 = entity1.GetComponent<T1>();
            var component2 = entity2.GetComponent<T2>();
            ifComponent(component1, component2);
         }
      }

      public static R As<T1, T2, R>(this Entity entity1, Entity entity2, Func<T1, T2, R> ifComponent, R withoutComponent = default(R))
         where T1 : Component
         where T2 : Component
      {
         if (entity1.HasComponent<T1>() && entity2.HasComponent<T2>())
         {
            var component1 = entity1.GetComponent<T1>();
            var component2 = entity2.GetComponent<T2>();
            return ifComponent(component1, component2);
         }
         return withoutComponent;
      }
   }
}
