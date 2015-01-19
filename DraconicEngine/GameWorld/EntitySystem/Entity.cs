using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals;
using LanguageExt;
using LanguageExt.Prelude;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   [DebuggerDisplay("Entity {Name}")]
   [JsonObject(MemberSerialization.OptIn)]
   public sealed class Entity
   {
      [JsonProperty]
      Dictionary<string, Component> components = new Dictionary<string, Component>();

      [NonSerialized]
      Subject<Component> componentAdded = new Subject<Component>();
      [NonSerialized]
      Subject<Component> componentRemoved = new Subject<Component>();

      #region Action Resolvers

      public T GetActionResolver<T>()
         where T : class, IActionResolver
      {
         return GetDefaultResolver<T>();
      }

      static Dictionary<Type, IActionResolver> defaultResolvers = new Dictionary<Type, IActionResolver>();

      static T GetDefaultResolver<T>()
         where T : class, IActionResolver
      {
         if (defaultResolvers.ContainsKey(typeof(T)))
         {
            return defaultResolvers[typeof(T)] as T;
         }
         var assembly = typeof(T).Assembly;
         var defaultResolverTypes = assembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t) &&
         t.CustomAttributes.Any(cad => cad.AttributeType == typeof(DefaultResolverAttribute))).ToList();

         if (defaultResolverTypes.Count > 1)
         {
            throw new Exception("Too many defaults");
         }
         else if (defaultResolverTypes.Count < 1)
         {
            throw new Exception("No default");
         }

         T resolver = assembly.CreateInstance(defaultResolverTypes.Single().FullName) as T;

         defaultResolvers[typeof(T)] = resolver;

         return resolver;
      }

      #endregion

      public Entity()
      {
      }

      public Entity(string name, params Component[] components)
      {
         this.Name = name;
         foreach(var component in components)
         {
            this.components.Add(component.GetType().Name, component);
            component.Owner = this;
         }
      }
      [JsonIgnore]
      public IEnumerable<Component> Components => this.components.Values;
      [JsonIgnore]
      public IObservable<Component> ComponentAdded => componentAdded;
      [JsonIgnore]
      public IObservable<Component> ComponentRemoved => componentRemoved;

      [JsonProperty]
      public string Name { get; set; }

      public bool IsDisposed { get { return false; } }

      public void AddComponent(Component component)
      {
         components.Add(component.GetType().Name, component);
         component.Owner = this;

         componentAdded.OnNext(component);
      }

      public bool HasComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         return this.components.ContainsKey(type.Name);
      }

      public Component GetComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         Component component;
         if (components.TryGetValue(type.Name, out component))
         {
            Debug.Assert(component.Owner == this, "A component's Owner is not the entity");
            return component;
         }

         throw new ArgumentException(string.Format("object {0} does not have a component of type {1}", this, type.Name));
      }

      public Option<Component> TryGetComponent(Type type)
      {

         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         Component component;
         if (components.TryGetValue(type.Name, out component))
         {
            Debug.Assert(component.Owner == this, "A component's Owner is not the entity");
            return component;
         }

         return None;
      }

      public void RemoveComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         if (components.ContainsKey(type.Name))
         {
            var component = components[type.Name];

            components.Remove(type.Name);
            componentRemoved.OnNext(component);
            if (component.Owner == this)
            {
               component.Owner = null;
            }
         }
      }

      public Entity Clone(bool fresh = true)
      {
         var newEntity = new Entity() { Name = this.Name };

         foreach(var kvp in this.components)
         {
            var newComp = kvp.Value.Clone(fresh);
            newComp.Owner = newEntity;
            newEntity.components.Add(kvp.Key, newComp);
         }

         return newEntity;
      }

      void PostSerialize()
      {

      }
   }

   public static class EntityExtensions
   {
      public static bool HasComponent<T>(this Entity self)
         where T : Component
      {
         return self.HasComponent(typeof(T));
      }

      public static T GetComponent<T>(this Entity self)
         where T : Component
      {
         return (T)self.GetComponent(typeof(T));
      }

      public static T GetComponentOrDefault<T>(this Entity self)
         where T : Component
      {
         if (self.HasComponent(typeof(T)))
         {
            return (T)self.GetComponent(typeof(T));
         }
         return default(T);
      }

      public static Option<T> TryGetComponent<T>(this Entity self)
         where T : Component
      {
         var result = self.TryGetComponent(typeof(T));

         return result.Match(
            Some: comp => (T)comp,
            None: () => (T)null);
      }

      public static void RemoveComponent<T>(this Entity self)
         where T : Component
      {
         self.RemoveComponent(typeof(T));
      }
   }
}
