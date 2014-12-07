using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public class Entity
   {
      Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
      public IEnumerable<IComponent> Components => this.components.Values;

      Subject<IComponent> componentAdded = new Subject<IComponent>();
      Subject<IComponent> componentRemoved = new Subject<IComponent>();

      readonly Dictionary<Type, Type> aliases = new Dictionary<Type, Type>();

      public Character Character { get; set; }

      public int X { get; set; }
      public int Y { get; set; }

      public string Name { get; set; }
      public bool Blocks { get; set; }

      public bool IsDisposed { get { return false; } }

      public Entity(string name, Glyph glyph, RogueColor color, bool blocks = false)
      {
         this.Name = name;
         this.Character = new Character(glyph, color);
         this.Blocks = blocks;
      }
      public IObservable<IComponent> ComponentAdded => componentAdded;
      public IObservable<IComponent> ComponentRemoved => componentRemoved;
      public void Draw(ITerminal terminal, Vector viewOffset)
      {
         var displayX = X - viewOffset.X;
         var displayY = Y - viewOffset.Y;

         if (displayX >= 0 && displayX < terminal.Size.X &&
             displayY >= 0 && displayY < terminal.Size.Y)
         {
            terminal.Set(displayX, displayY, this.Character);
         }
      }

      public void Clear(ITerminal terminal, Vector viewOffset)
      {
         var displayX = X - viewOffset.X;
         var displayY = Y - viewOffset.Y;

         if (displayX >= 0 && displayX < terminal.Size.X &&
             displayY >= 0 && displayY < terminal.Size.Y)
         {
            terminal.Set(displayX, displayY, Character.Space);
         }
      }

      public Loc Location
      {
         get { return new Loc(X, Y); }
         set
         {
            X = value.X;
            Y = value.Y;
         }
      }

      public void AddComponent(Type type, IComponent component)
      {
         components.Add(type, component);

         componentAdded.OnNext(component);

         var compType = component.GetType();
         if (compType != type)
         {
            this.AddComponentAlias(type, compType);
         }

         component.Owner = this;
         OnComponentAdded(component);
      }

      protected virtual void OnComponentAdded(IComponent component) { }

      public void AddComponentAlias(Type registeredType, Type alias)
      {
         if (this.components.ContainsKey(registeredType))
         {
            aliases[alias] = registeredType;
         }
      }

      public bool HasComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         return this.components.ContainsKey(type) || (this.aliases.ContainsKey(type) && this.components.ContainsKey(this.aliases[type]));
      }

      protected void SetComponent<TComp>(ref TComp component, TComp value)
         where TComp : IComponent
      {
         if (!object.Equals(component, value))
         {
            if (this.HasComponent(typeof(TComp)))
            {
               this.RemoveComponent(typeof(TComp));
            }
            component = value;
            AddComponent(typeof(TComp), value);
         }
      }

      public IComponent GetComponent(Type type)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         IComponent component;
         if (components.TryGetValue(type, out component))
         {
            return component;
         }
         else if (this.aliases.ContainsKey(type))
         {
            var alias = this.aliases[type];
            if (components.TryGetValue(alias, out component))
            {
               return component;
            }
         }

         throw new ArgumentException(string.Format("object {0} does not have a component of type {1}", this, type.Name));
      }

      public bool TryGetComponent(Type type, out IComponent component)
      {
         if (this.IsDisposed)
         {
            throw new ObjectDisposedException("CompositeObject");
         }
         var result = components.TryGetValue(type, out component);
         if (!result && this.aliases.ContainsKey(type))
         {
            var alias = this.aliases[type];
            result = components.TryGetValue(alias, out component);
         }
         return result;
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
            var allAliases = this.aliases.Where(kvp => kvp.Value == type).Select(kvp => kvp.Key).ToArray();
            foreach (var alias in allAliases)
            {
               this.aliases.Remove(alias);
            }
            components.Remove(type);
            componentRemoved.OnNext(component);
            if (component.Owner == this)
            {
               component.Owner = null;
            }
            OnComponentRemoved(component);
         }
      }

      protected virtual void OnComponentRemoved(IComponent component)
      {
      }
   }

   public static class EntityExtensions
   {
      public static Vector DistanceTo(this Entity self, Entity other)
      {
         return other.Location - self.Location;
      }

      public static void AddComponent<TComponent>(this Entity self, TComponent component)
          where TComponent : IComponent
      {
         self.AddComponent(typeof(TComponent), component);
      }

      public static void AddComponent<TInterface, TComponent>(this Entity self, TComponent component)
         where TInterface : IComponent
         where TComponent : TInterface
      {
         self.AddComponent(typeof(TInterface), component);
      }

      public static void AddComponentAlias<TComponent>(this Entity self, TComponent component)
          where TComponent : IComponent
      {
         self.AddComponent(typeof(TComponent), component);
      }

      public static bool HasComponent<T>(this Entity self)
         where T : IComponent
      {
         return self.HasComponent(typeof(T));
      }

      public static T GetComponent<T>(this Entity self)
         where T : IComponent
      {
         return (T)self.GetComponent(typeof(T));
      }

      public static T GetComponentOrDefault<T>(this Entity self)
         where T : IComponent
      {
         if (self.HasComponent(typeof(T)))
         {
            return (T)self.GetComponent(typeof(T));
         }
         return default(T);
      }

      public static bool TryGetComponent<T>(this Entity self, out T component)
         where T : IComponent
      {
         IComponent comp;
         var result = self.TryGetComponent(typeof(T), out comp);
         component = (T)comp;
         return result;
      }

      public static void RemoveComponent<T>(this Entity self)
         where T : IComponent
      {
         self.RemoveComponent(typeof(T));
      }
   }
}
