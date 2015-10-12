using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals;
using LanguageExt;
using static LanguageExt.Prelude;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DraconicEngine.EntitySystem
{
   [DebuggerDisplay("Entity {Name}")]
   [JsonObject(MemberSerialization.OptIn)]
   public sealed class Entity
   {
      [JsonProperty]
      Dictionary<string, Component> components = new Dictionary<string, Component>();
      [JsonProperty]
      Dictionary<string, CharacterStat> stats = new Dictionary<string, CharacterStat>();

      [NonSerialized]
      Subject<Component> componentAdded = new Subject<Component>();
      [NonSerialized]
      Subject<Component> componentRemoved = new Subject<Component>();

      public Entity()
      {
      }

      public Entity(string name, ComponentSet components = null, StatSet stats = null)
      {
         this.Name = name;

         foreach (var stat in stats?.Stats ?? Enumerable.Empty<CharacterStat>())
         {
            this.AddStat(stat);
         }
         foreach (var component in components?.Components ?? Enumerable.Empty<Component>())
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

      [JsonProperty]
      public Loc Location { get; set; } = Loc.Zero;

      [JsonProperty]
      public bool Blocks { get; set; } = false;

      public void AddComponent(Component component)
      {
         components.Add(component.GetType().Name, component);
         component.Owner = this;

         componentAdded.OnNext(component);
      }

      public bool HasComponent(Type type) => this.components.ContainsKey(type.Name);

      public Component GetComponent(Type type)
      {
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
         var newEntity = new Entity()
         {
            Name = Name,
            Blocks = Blocks,
            Location = fresh ? Location : Loc.Zero
         };

         foreach (var stat in this.stats)
         {
            newEntity.stats.Add(stat.Key, stat.Value);
         }

         foreach (var kvp in this.components)
         {
            var newComp = kvp.Value.Clone(fresh);
            newComp.Owner = newEntity;
            newEntity.components.Add(kvp.Key, newComp);
         }

         return newEntity;
      }

      public void AddStat(CharacterStat stat) => stats.Add(stat.Name, stat);
      public bool HasStat(string name) => stats.ContainsKey(name);

      public CharacterStat GetStat(string name) => stats[name];
      public CharacterStat<T> GetStat<T>(string name)
      {
         if (stats.ContainsKey(name))
         {
            return (CharacterStat<T>)stats[name];
         }
         else
         {
            var stat = new CharacterStat<T>(name);
            stats.Add(name, stat);

            return stat;
         }
      }

      void PostSerialize()
      {
      }
   }

   public static class EntityExtensions
   {
      public static bool HasComponent<T>(this Entity self) where T : Component => self.HasComponent(typeof(T));

      public static T GetComponent<T>(this Entity self) where T : Component => (T)self.GetComponent(typeof(T));

      public static T GetComponentOrDefault<T>(this Entity self)
         where T : Component
      {
         if (self.HasComponent(typeof(T)))
         {
            return (T)self.GetComponent(typeof(T));
         }
         return default(T);
      }

      public static Option<T> TryGetComponent<T>(this Entity self) where T : Component => self.TryGetComponent(typeof(T)).Map(comp => (T)comp);

      public static void RemoveComponent<T>(this Entity self) where T : Component => self.RemoveComponent(typeof(T));

      public static CharacterStat<T> AddStat<T>(this Entity self, string name, T value = default(T))
      {
         var stat = new CharacterStat<T>(name) { Value = value };
         self.AddStat(stat);
         return stat;
      }
   }

   [DebuggerDisplay("Stat {Name}")]
   [JsonObject(MemberSerialization.OptIn)]
   public abstract class CharacterStat : ISerializable
   {
      [JsonProperty]
      public string Name { get; }

      protected CharacterStat(string name) { Name = name; }

      protected CharacterStat(SerializationInfo information, StreamingContext context) { Name = information.GetString("Name"); }

      public virtual void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("Name", Name);

      public abstract object GetValue();

      public static CharacterStat<T> Make<T>(string name, T value) => new CharacterStat<T>(name, value);
   }

   public class CharacterStat<T> : CharacterStat
   {
      [NonSerialized]
      T value;

      [JsonProperty]
      public T Value
      {
         get { return value; }
         set
         {
            if (!this.value.Equals(value))
            {
               this.value = value;
               //changed!
            }
         }
      }

      public CharacterStat(string name) : base(name) { }

      public CharacterStat(string name, T value) : base(name) { this.value = value; }

      public CharacterStat(SerializationInfo information, StreamingContext context)
         : base(information, context)
      {
         value = (T)information.GetValue("Value", typeof(T));
      }

      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("Value", Value);
      }

      public override object GetValue() => value;

      public override string ToString() => value.ToString();

      public static implicit operator T(CharacterStat<T> self) => self.value;

   }

   public class ComponentSet
   {
      public ComponentSet(params Component[] components)
      {
         Components = components.ToList();
      }

      public List<Component> Components { get; private set; }
   }

   public class StatSet
   {
      public StatSet(params CharacterStat[] stats)
      {
         Stats = stats.ToList();
      }

      public List<CharacterStat> Stats { get; private set; }
   }
}
