using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals;
using LanguageExt;
using LanguageExt.Prelude;
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
   public sealed class Entity
   {
      Dictionary<Type, Component> components = new Dictionary<Type, Component>();

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
            this.components.Add(component.GetType(), component);
            component.Owner = this;
         }
      }
      public IEnumerable<Component> Components => this.components.Values;
      public IObservable<Component> ComponentAdded => componentAdded;
      public IObservable<Component> ComponentRemoved => componentRemoved;

      public string Name { get; set; }

      public bool IsDisposed { get { return false; } }

      //public void Draw(ITerminal terminal, Vector viewOffset)
      //{
      //   var display = Location - viewOffset;

      //   if (display.X >= 0 && display.X < terminal.Size.X &&
      //       display.Y >= 0 && display.Y < terminal.Size.Y)
      //   {
      //      terminal.Set(display, this.Character);
      //   }
      //}

      //public Character Character
      //{
      //   get { return this.GetComponentOrDefault<DrawnComponent>()?.SeenCharacter ?? new Character(Glyph.Space, RogueColors.White); }
      //   set { this.As<DrawnComponent>(comp => comp.SeenCharacter = value); }
      //}

      public Loc Location
      {
         get { return this.GetComponentOrDefault<LocationComponent>()?.Location ?? new Loc(-1, -1); }
         set { this.As<LocationComponent>(comp => comp.Location = value); }
      }
      public bool Blocks
      {
         get { return this.GetComponentOrDefault<LocationComponent>()?.Blocks ?? false; }
         set { this.As<LocationComponent>(comp => comp.Blocks = value); }
      }

      public void AddComponent(Type type, Component component)
      {
         components.Add(type, component);
         component.Owner = this;

         componentAdded.OnNext(component);
      }

      public bool HasComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         return this.components.ContainsKey(type);
      }

      public Component GetComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         Component component;
         if (components.TryGetValue(type, out component))
         {
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
         if (components.TryGetValue(type, out component))
         {
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
         if (components.ContainsKey(type))
         {
            var component = components[type];

            components.Remove(type);
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

            newEntity.components.Add(kvp.Key, newComp);
         }

         return newEntity;
      }
   }

   public static class EntityExtensions
   {
      public static Vector DistanceTo(this Entity self, Entity other)
      {
         return other.Location - self.Location;
      }

      public static void AddComponent<TComponent>(this Entity self, TComponent component)
          where TComponent : Component
      {
         self.AddComponent(typeof(TComponent), component);
      }

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
