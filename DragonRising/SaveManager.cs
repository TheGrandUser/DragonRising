using DraconicEngine;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.Storage;
using DragonRising.Properties;
using DragonRising.Storage;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising
{
   class JsonSaveManager : ISaveManager
   {
      JsonSerializer serializer = new JsonSerializer()
      {
         Formatting = Formatting.Indented,
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
         NullValueHandling = NullValueHandling.Ignore,
         DefaultValueHandling = DefaultValueHandling.Ignore,
         TypeNameHandling = TypeNameHandling.Auto,
         PreserveReferencesHandling = PreserveReferencesHandling.Objects,
         Context = new StreamingContext(StreamingContextStates.Persistence | StreamingContextStates.File, Library.Current),
         ReferenceResolver = new LibraryReferenceResolver()
      };

      public JsonSaveManager()
      {
         //serializer.Converters.Add(new SomeTypeConverter());
         serializer.Converters.Add(new EntityConverter());
         serializer.Converters.Add(new CharacterConverter());
         serializer.Converters.Add(new UsageConverter());
         serializer.Converters.Add(new BehaviorConverter());
      }

      public string LastSaveGame => File.Exists(ToFile(Settings.Default.LastSaveGame)) ? Settings.Default.LastSaveGame : string.Empty;

      string ToFile(string name)
      {
         var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var saveGameDir = Path.Combine(documents, "My Games", "Dragon Rising", "Saves");
         var filePath = Path.Combine(saveGameDir, name + ".sav");

         return filePath;
      }

      public IEnumerable<string> GetSaveGames()
      {
         var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var saveGamesDir = Path.Combine(documents, "My Games", "Dragon Rising", "Saves");

         if (Directory.Exists(saveGamesDir))
         {
            return Directory.EnumerateFiles(saveGamesDir, "*.sav")
               .Select(filePath => Path.GetFileNameWithoutExtension(filePath));
         }

         return Enumerable.Empty<string>();
      }

      class SceneInfo
      {
         public int mapWidth { get; set; }
         public int mapHeight { get; set; }
         public Entity[] entities { get; set; }
         public int focusEntityId { get; set; }
      }

      public async Task<Scene> LoadGame(string name)
      {
         var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var saveGameDir = Path.Combine(documents, "My Games", "Dragon Rising", "Saves");
         var filePath = Path.Combine(saveGameDir, name + ".sav");
         var mapFilePath = Path.Combine(saveGameDir, name + ".map");

         JObject doc;
         using (var streamReader = File.OpenText(filePath))
         {
            var raw = await streamReader.ReadToEndAsync();

            doc = JObject.Parse(raw);
         }

         var sceneInfo = serializer.Deserialize<SceneInfo>(doc.CreateReader());

         var width = sceneInfo.mapWidth;
         var height = sceneInfo.mapHeight;

         var tiles = Library.Current.Tiles;

         var map = new Tile[width * height];

         using (var mapFile = File.OpenRead(mapFilePath))
         {
            using (var compressor = new GZipStream(mapFile, CompressionMode.Decompress))
            {
               using (var binaryReader = new BinaryReader(compressor))
               {
                  for (int index = 0; index < map.Length; index++)
                  {
                     var id = binaryReader.ReadInt32();
                     var seen = binaryReader.ReadInt32();
                     map[index] = new Tile(id)
                     {
                        Visibility = (TileVisibility)seen
                     };
                  }
               }
            }
         }
         var scene = new Scene(map, width, height);

         foreach (var entity in sceneInfo.entities)
         {
            scene.EntityStore.Add(entity);
         }

         scene.FocusEntity = sceneInfo.entities[sceneInfo.focusEntityId];

         return scene;
      }

      public Task SaveGame(string name, Scene scene)
      {
         var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         var saveGamesDir = Path.Combine(documents, "My Games", "Dragon Rising", "Saves");

         if (!Directory.Exists(saveGamesDir))
         {
            Directory.CreateDirectory(saveGamesDir);
         }

         var filePath = Path.Combine(saveGamesDir, name + ".sav");
         var mapFilePath = Path.Combine(saveGamesDir, name + ".map");

         var entities = scene.EntityStore.AllEntities.ToArray();

         var sceneInfo = new SceneInfo()
         {
            mapWidth = scene.MapWidth,
            mapHeight = scene.MapHeight,
            entities = entities,
            focusEntityId = Array.IndexOf(entities, scene.FocusEntity),
         };

         //var doc = new JObject();
         //var writer = doc.CreateWriter();

         //serializer.Serialize(writer, sceneInfo);

         using (var mapFile = File.OpenWrite(mapFilePath))
         {
            using (var compressor = new GZipStream(mapFile, CompressionLevel.Optimal))
            {
               using (var binaryWriter = new BinaryWriter(compressor))
               {
                  for (int i = 0; i < scene.Map.Length; i++)
                  {
                     binaryWriter.Write(scene.Map[i].TileTypeId);
                     binaryWriter.Write((int)scene.Map[i].Visibility);
                  }
               }
            }
         }

         using (var file = File.OpenWrite(filePath))
         {
            //using (var compact = new GZipStream(file, CompressionLevel.Optimal))
            {
               //using (var streamWriter = new StreamWriter(compact))
               using (var streamWriter = new StreamWriter(file))
               {
                  //await streamWriter.WriteAsync(doc.ToString(Formatting.None));
                  serializer.Serialize(streamWriter, sceneInfo);
               }
            }
         }

         Settings.Default.LastSaveGame = name;
         Settings.Default.Save();

         return Task.FromResult(0);
      }
   }

   class LibraryReferenceResolver : IReferenceResolver
   {
      public LibraryReferenceResolver()
      {
         var allEntityTemplates =
            Library.Current.Entities.Templates.Concat(
            Library.Current.Items.Templates);
         //Library.Items
         //Behavior
         //var templates = library.EntityTemplates;
         foreach (var kvp in allEntityTemplates)
         {
            var libId = "lib-" + kvp.Key;
            mappings.Set(libId, kvp.Value);

            foreach (var componentTempate in kvp.Value.Components)
            {
               var componentTempateId = libId + "-comp-" + componentTempate.GetType().Name;

               mappings.Set(componentTempateId, componentTempate);
            }
         }
      }


      private int _referenceCount;
      private BidirectionalDictionary<string, object> mappings = new BidirectionalDictionary<string, object>();

      private BidirectionalDictionary<string, object> GetMappings(object context)
      {
         return this.mappings;
         //JsonSerializerInternalBase internalSerializer;

         //if (context is JsonSerializerInternalBase)
         //   internalSerializer = (JsonSerializerInternalBase)context;
         //else if (context is JsonSerializerProxy)
         //   internalSerializer = ((JsonSerializerProxy)context).GetInternalSerializer();
         //else
         //   throw new JsonException("The DefaultReferenceResolver can only be used internally.");

         //return internalSerializer.DefaultReferenceMappings;
      }

      public object ResolveReference(object context, string reference)
      {
         object value;
         GetMappings(context).TryGetByFirst(reference, out value);
         return value;
      }

      public string GetReference(object context, object value)
      {
         var mappings = GetMappings(context);

         string reference;
         if (!mappings.TryGetBySecond(value, out reference))
         {
            _referenceCount++;
            reference = _referenceCount.ToString(CultureInfo.InvariantCulture);
            mappings.Set(reference, value);
         }

         return reference;
      }

      public void AddReference(object context, string reference, object value)
      {
         GetMappings(context).Set(reference, value);
      }

      public bool IsReferenced(object context, object value)
      {
         string reference;
         return GetMappings(context).TryGetBySecond(value, out reference);
      }

   }

   class BidirectionalDictionary<TFirst, TSecond>
   {
      private readonly IDictionary<TFirst, TSecond> _firstToSecond;
      private readonly IDictionary<TSecond, TFirst> _secondToFirst;
      private readonly string _duplicateFirstErrorMessage;
      private readonly string _duplicateSecondErrorMessage;

      public BidirectionalDictionary()
          : this(EqualityComparer<TFirst>.Default, EqualityComparer<TSecond>.Default)
      {
      }

      public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer)
          : this(
              firstEqualityComparer,
              secondEqualityComparer,
              "Duplicate item already exists for '{0}'.",
              "Duplicate item already exists for '{0}'.")
      {
      }

      public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer,
          string duplicateFirstErrorMessage, string duplicateSecondErrorMessage)
      {
         _firstToSecond = new Dictionary<TFirst, TSecond>(firstEqualityComparer);
         _secondToFirst = new Dictionary<TSecond, TFirst>(secondEqualityComparer);
         _duplicateFirstErrorMessage = duplicateFirstErrorMessage;
         _duplicateSecondErrorMessage = duplicateSecondErrorMessage;
      }

      public void Set(TFirst first, TSecond second)
      {
         TFirst existingFirst;
         TSecond existingSecond;

         if (_firstToSecond.TryGetValue(first, out existingSecond))
         {
            if (!existingSecond.Equals(second))
               throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, _duplicateFirstErrorMessage, first));
         }

         if (_secondToFirst.TryGetValue(second, out existingFirst))
         {
            if (!existingFirst.Equals(first))
               throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, _duplicateSecondErrorMessage, second));
         }

         _firstToSecond.Add(first, second);
         _secondToFirst.Add(second, first);
      }

      public bool TryGetByFirst(TFirst first, out TSecond second)
      {
         return _firstToSecond.TryGetValue(first, out second);
      }

      public bool TryGetBySecond(TSecond second, out TFirst first)
      {
         return _secondToFirst.TryGetValue(second, out first);
      }
   }

   class SomeTypeConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType)
      {
         return objectType.IsConstructedGenericType && objectType.GetGenericTypeDefinition() == typeof(Some<>);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         var someType = typeof(Some<>);

         throw new NotImplementedException();
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var someType = value.GetType();
         var valueProperty = someType.GetProperty("Value");
         var realObject = valueProperty.GetValue(value);

         writer.WriteValue(realObject);
      }
   }

   class EntityConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType)
      {
         return typeof(Entity).IsAssignableFrom(objectType);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType == JsonToken.Null)
            return null;


         var entity = (existingValue as Entity) ??
            ((objectType == typeof(Entity)) ? new Entity() : (Entity)Activator.CreateInstance(objectType));

         CheckedRead(reader);

         while (reader.TokenType == JsonToken.PropertyName)
         {
            var propertyName = reader.Value.ToString();

            CheckedRead(reader);

            if (propertyName == "Name")
            {
               entity.Name = reader.Value.ToString();
            }
            else if (propertyName == "Components")
            {
               while (reader.TokenType == JsonToken.Comment)
               {
                  CheckedRead(reader);
               }

               var components = ReadComponents(reader, serializer);

               foreach (var component in components)
               {
                  entity.AddComponent(component);
               }
            }


            CheckedRead(reader);
         }

         return entity;
      }

      List<Component> ReadComponents(JsonReader reader, JsonSerializer serializer)
      {
         List<Component> components = new List<Component>();

         if (reader.TokenType != JsonToken.StartArray)
         {
            throw new JsonSerializationException("Unexpected end when reading Entity.");
         }

         while (reader.Read())
         {
            switch (reader.TokenType)
            {
               case JsonToken.Comment:
                  break;
               case JsonToken.EndArray:
                  return components;
               default:

                  var component = (Component)serializer.Deserialize(reader);

                  components.Add(component);

                  break;
            }
         }

         throw new JsonSerializationException("Unexpected end when reading Entity.");
      }

      private void CheckedRead(JsonReader reader)
      {
         if (!reader.Read())
            throw new JsonSerializationException("Unexpected end when reading Entity.");
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var entity = (Entity)value;

         writer.WriteStartObject();
         writer.WritePropertyName("Name");
         writer.WriteValue(entity.Name);
         writer.WritePropertyName("Components");
         writer.WriteStartArray();
         foreach (var component in entity.Components)
         {
            serializer.Serialize(writer, component, typeof(Component));
         }
         writer.WriteEndArray();
         writer.WriteEndObject();
      }
   }

   public class CharacterConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType)
      {
         return objectType == typeof(Character);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType != JsonToken.StartObject)
         {
            // Error
         }

         var obj = JObject.ReadFrom(reader);

         Glyph glyph = (Glyph)(obj["Glyph"].Value<int>());

         int color = obj["Fore"].Value<int>();


         RogueColor foreColor = new RogueColor(color);
         RogueColor? backColor = null;

         if (obj.Contains("Back"))
         {
            color = obj["Back"].Value<int>();
            backColor = new RogueColor(color);
         }

         return new Character(glyph, foreColor, backColor);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var c = (Character)value;

         writer.WriteStartObject();
         writer.WritePropertyName("Glyph");
         writer.WriteValue((int)c.Glyph);
         writer.WritePropertyName("Fore");
         writer.WriteValue(c.ForeColor.ToInt32());
         if (c.BackColor.HasValue)
         {
            writer.WritePropertyName("Back");
            writer.WriteValue(c.BackColor.Value.ToInt32());
         }
         writer.WriteEndObject();
      }
   }

   public class UsageConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType)
      {
         return typeof(IItemUsage).IsAssignableFrom(objectType);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType != JsonToken.String)
         {
            throw new JsonSerializationException("unexpected token type when deserializing an IItemUsage");
         }

         var name = reader.Value.ToString();
         var library = (ILibrary)serializer.Context.Context;

         return library.ItemUsages.Get(name);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var usage = (IItemUsage)value;
         var library = (ILibrary)serializer.Context.Context;
         writer.WriteValue(library.ItemUsages.NameForUsage(usage));
      }
   }

   public class BehaviorConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType)
      {
         return typeof(Behavior).IsAssignableFrom(objectType);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType != JsonToken.String)
         {
            throw new JsonSerializationException("unexpected token type when deserializing an IItemUsage");
         }

         var name = reader.Value.ToString();
         var library = (ILibrary)serializer.Context.Context;

         return library.Behaviors.Get(name);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var behavior = (Behavior)value;
         writer.WriteValue(behavior.Name);
      }
   }
}