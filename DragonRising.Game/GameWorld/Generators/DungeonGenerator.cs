using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.Utilities;
using DragonRising.GameWorld.Components;

namespace DragonRising.Generators
{
   public class DungeonGenerator : IMapGenerator
   {
      static readonly int RoomMaxSize = 10;
      static readonly int RoomMinSize = 6;
      static readonly int MaxRooms = 30;
      
      IPopulationGenerator populationGenerator;
      IItemGenerator itemGenerator;

      List<Tuple<int, int>> itemsPerRoomByLevel;
      List<Tuple<int, int>> monstersPerRoomByLevel;

      public DungeonGenerator(IPopulationGenerator populationGenerator, IItemGenerator itemGenerator)
      {
         this.populationGenerator = populationGenerator;
         this.itemGenerator = itemGenerator;

         this.itemsPerRoomByLevel = new List<Tuple<int, int>>
         {
            tuple(1,1),
            tuple(2,4),
         };

         this.monstersPerRoomByLevel = new List<Tuple<int, int>>
         {
            tuple(2,1),
            tuple(3,4),
            tuple(5,6),
         };
      }

      public Loc MakeMap(Scene scene)
      {
         MapBuilder builder = new MapBuilder(scene);

         var rooms = new List<TerminalRect>();
         var mapWidth = scene.MapWidth;
         var mapHeight = scene.MapHeight;
         var random = RogueGame.Current.GameRandom;

         foreach (var _ in Enumerable.Range(0, MaxRooms))
         {
            var w = random.Next(RoomMinSize, RoomMaxSize + 1);
            var h = random.Next(RoomMinSize, RoomMaxSize + 1);
            var x = random.Next(0, mapWidth - w - 1);
            var y = random.Next(0, mapHeight - h - 1);

            var newRoom = new TerminalRect(x, y, w, h);

            if (!rooms.Any(r => TerminalRect.Intersects(newRoom, r)))
            {
               builder.CreateRoom(newRoom);
               if (rooms.Count > 0)
               {
                  var prevRoom = rooms.Last();

                  var prevCenter = prevRoom.Center;
                  var currentCenter = newRoom.Center;

                  if (random.Next(2) == 0)
                  {
                     builder.CreateHTunnel(prevCenter.X, currentCenter.X, prevCenter.Y);
                     builder.CreateVTunnel(prevCenter.Y, currentCenter.Y, currentCenter.X);
                  }
                  else
                  {
                     builder.CreateVTunnel(prevCenter.Y, currentCenter.Y, prevCenter.X);
                     builder.CreateHTunnel(prevCenter.X, currentCenter.X, currentCenter.Y);
                  }

                  PlaceEntities(scene, newRoom);
               }

               rooms.Add(newRoom);
            }
         }

         var stairs = new Entity("Stairs",
            new DrawnComponent()
            {
               SeenCharacter = new Character(Glyph.LessThan, RogueColors.White),
               ExploredCharacter = new Character(Glyph.LessThan, RogueColors.LightGray)
            },
            new LocationComponent() { Location = rooms.Last().Center });

         scene.EntityStore.AddEntity(stairs);
         scene.Stairs = stairs;

         return rooms.First().Center;
      }

      void PlaceEntities(Scene scene, TerminalRect room)
      {
         DoGeneration(scene, room, this.populationGenerator.GenerarateMonster, monstersPerRoomByLevel);

         DoGeneration(scene, room, this.itemGenerator.GenerateItem, itemsPerRoomByLevel);
      }

      private void DoGeneration(Scene scene, TerminalRect room, Func<int, Entity> generator, IEnumerable<Tuple<int, int>> countPerLevel)
      {
         var count = ItemSelection.FromDungeonLevel(scene.Level, countPerLevel).Match(
            Some: maxCount => RogueGame.Current.GameRandom.Next(maxCount + 1),
            None: () => 0);
         
         for (int i = 0; i < count; i++)
         {
            var location = GetRandomLocationInRoom(room);

            if (scene.IsBlocked(location) == Blockage.None)
            {
               var entity = generator(scene.Level);

               entity.SetLocation(location);

               scene.EntityStore.AddEntity(entity);
            }
         }
      }

      public Loc GetRandomLocationInRoom(TerminalRect room)
      {
         var x = RogueGame.Current.GameRandom.Next(room.Left + 1, room.Right);
         var y = RogueGame.Current.GameRandom.Next(room.Top + 1, room.Bottom);

         return new Loc(x, y);
      }
   }
}
