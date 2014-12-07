using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Generators;

namespace DragonRising.Generators
{
   public class DungeonGenerator : IMapGenerator
   {
      static readonly int RoomMaxSize = 10;
      static readonly int RoomMinSize = 6;
      static readonly int MaxRooms = 30;
      static readonly int MaxRoomMonsters = 3;
      static readonly int MaxRoomItems = 3;

      Random random = new Random();
      IPopulationGenerator populationGenerator;
      IItemGenerator itemGenerator;

      public DungeonGenerator(IPopulationGenerator populationGenerator, IItemGenerator itemGenerator)
      {
         this.populationGenerator = populationGenerator;
         this.itemGenerator = itemGenerator;
      }

      public Loc MakeMap(Scene scene)
      {
         MapBuilder builder = new MapBuilder(scene);

         var rooms = new List<TerminalRect>();
         var mapWidth = scene.MapWidth;
         var mapHeight = scene.MapHeight;

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

         return rooms.First().Center;
      }

      void PlaceEntities(Scene scene, TerminalRect room)
      {
         DoGeneration(scene, room, this.populationGenerator.GenerarateMonster, MaxRoomMonsters);

         DoGeneration(scene, room, this.itemGenerator.GenerateEntityItem, MaxRoomItems);
      }

      private void DoGeneration<TEntity>(Scene scene, TerminalRect room, Func<TEntity> generator, int maxCount)
         where TEntity : Entity
      {
         var count = this.random.Next(maxCount + 1);

         for (int i = 0; i < count; i++)
         {
            var location = GetRandomLocationInRoom(room);

            if (scene.IsBlocked(location) == Blockage.None)
            {
               var entity = generator();

               entity.Location = location;

               scene.EntityStore.Add(entity);
            }
         }
      }

      public Loc GetRandomLocationInRoom(TerminalRect room)
      {
         var x = this.random.Next(room.Left + 1, room.Right);
         var y = this.random.Next(room.Top + 1, room.Bottom);

         return new Loc(x, y);
      }
   }
}
